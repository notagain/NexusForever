using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PathMissionManager : ISaveCharacter
        //, IEnumerable<Currency>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Player player;
        private readonly Dictionary<uint, PathEpisode> episodes = new Dictionary<uint, PathEpisode>();
        private readonly Dictionary<uint, PathMission> missions = new Dictionary<uint, PathMission>();

        /// <summary>
        /// Create a new <see cref="PathMissionManager"/> from <see cref="Player"/> database model.
        /// </summary>
        public PathMissionManager(Player owner, Character model)
        {
            player = owner;

            foreach (var characterPathEpisode in model.CharacterPathEpisode)
                episodes.Add(characterPathEpisode.EpisodeId, new PathEpisode(characterPathEpisode));
            foreach (var characterPathMission in model.CharacterPathMission)
                missions.Add(characterPathMission.MissionId, new PathMission(characterPathMission));

            log.Debug($"Episodes: {episodes.Count} | Missions: {missions.Count}");

            //TODO: Check that all missions for each episode are accounted for and add missing ones. Need to do this in case DB crashes mid-write.
        }

        /// <summary>
        /// Create a new <see cref="CharacterPathEpisode"/>.
        /// </summary>
        public PathEpisode PathEpisodeCreate(uint episodeId)
        {
            PathEpisodeEntry pathEpisodeEntry = GameTableManager.PathEpisode.GetEntry(episodeId);
            if (pathEpisodeEntry == null)
                return null;

            return PathEpisodeCreate(pathEpisodeEntry);
        }

        /// <summary>
        /// Create a new <see cref="CharacterPathEpisode"/>.
        /// </summary>
        public PathEpisode PathEpisodeCreate(PathEpisodeEntry pathEpisodeEntry)
        {
            if (pathEpisodeEntry == null)
                return null;

            if (episodes.ContainsKey(pathEpisodeEntry.Id))
                throw new ArgumentException($"Path Episode {pathEpisodeEntry.Id} is already added to the player!");

            PathEpisode pathEpisode = new PathEpisode(
                player.CharacterId,
                pathEpisodeEntry,
                false
            );
            episodes.Add(pathEpisodeEntry.Id, pathEpisode);
            CreateAllMissionsForEpisode(pathEpisode.Id);
            return pathEpisode;
        }

        /// <summary>
        /// Create a new <see cref="CharacterPathMission"/>.
        /// </summary>
        public PathMission PathMissionCreate(uint missionId)
        {
            PathMissionEntry pathMissionEntry = GameTableManager.PathMission.GetEntry(missionId);
            if (pathMissionEntry == null)
                return null;

            return PathMissionCreate(pathMissionEntry);
        }

        /// <summary>
        /// Create a new <see cref="CharacterPathMission"/>.
        /// </summary>
        public PathMission PathMissionCreate(PathMissionEntry pathMissionEntry)
        {
            if (pathMissionEntry == null)
                return null;

            if (episodes.ContainsKey(pathMissionEntry.Id))
                throw new ArgumentException($"Path Episode {pathMissionEntry.Id} is already added to the player!");

            PathMission pathMission = new PathMission(
                player.CharacterId,
                pathMissionEntry,
                false
            );
            missions.Add(pathMissionEntry.Id, pathMission);
            return pathMission;
        }

        /// <summary>
        /// Create all <see cref="CharacterPathMission"/> for a <see cref="PathEpisode"/>
        /// </summary>
        /// <param name="episodeId"></param>
        public void CreateAllMissionsForEpisode(uint episodeId)
        {
            if(episodeId <= 0)
                throw new ArgumentException($"Path Episode ID must be greater than 0.");

            PathMissionEntry[] pathMissionEntries = GetEpisodeMissions(episodeId);

            foreach (PathMissionEntry pathMission in pathMissionEntries)
                PathMissionCreate(pathMission);
        }

        /// <summary>
        /// Returns all <see cref="PathMissionEntry"/> for a given <see cref="PathEpisodeEntry"/>
        /// </summary>
        /// <param name="pathEpisodeId"></param>
        /// <returns></returns>
        public PathMissionEntry[] GetEpisodeMissions(uint pathEpisodeId)
        {
            if (pathEpisodeId <= 0)
                return null;

            PathMissionEntry[] episodeMissions = Array.FindAll(GameTableManager.PathMission.Entries, x => x.PathEpisodeId == pathEpisodeId);

            return episodeMissions;
        }

        /// <summary>
        /// Initiates necessary methods for when a player loads into the game. Must be called after entity has been created.
        /// </summary>
        public void Load()
        {
            SetEpisodeProgress();
            SetCurrentZoneEpisode();
        }

        /// <summary>
        /// Sets the episode progress and sends to the player
        /// </summary>
        public void SetEpisodeProgress()
        {
            foreach (KeyValuePair<uint, PathEpisode> pathEpisode in episodes)
            {
                List<PathMission> pathMissions = new List<PathMission>();
                PathMissionEntry[] pathMissionEntries = GetEpisodeMissions(pathEpisode.Value.Id);
                foreach (PathMissionEntry pathMission in pathMissionEntries)
                {
                    PathMission matchingMission = missions.FirstOrDefault(x => x.Value.Id == pathMission.Id).Value;
                    log.Debug($"Mission - ID: {matchingMission.Id}, Completed: {matchingMission.Completed}, Progress: {matchingMission.Progress}, State: {matchingMission.State}");
                    pathMissions.Add(matchingMission);
                }

                SendServerPathEpisodeProgress(pathEpisode.Value.Id, pathMissions.ToArray());
            }
        }

        /// <summary>
        /// Sets the current episode based on zone and sends to the player
        /// </summary>
        public void SetCurrentZoneEpisode()
        {
            PathEpisodeEntry currentMapEpisode = GetEpisodeForMap();
            if (currentMapEpisode != null)
                SendServerPathCurrentEpisode(currentMapEpisode.Id);

            if (episodes.FirstOrDefault(x => x.Value.Id == currentMapEpisode.Id).Value == null)
                PathEpisodeCreate(currentMapEpisode.Id);
        }

        /// <summary>
        /// Get the matching <see cref="PathEpisodeEntry"/> for map the player's currently on
        /// </summary>
        /// <returns></returns>
        private PathEpisodeEntry GetEpisodeForMap()
        {
            // TODO: Use Zone ID & World ID when we can track zone
            uint worldId = player.Map.Entry.Id;
            PathEpisodeEntry matchedEpisode = Array.Find(GameTableManager.PathEpisode.Entries, x => x.WorldId == worldId && x.PathTypeEnum == (uint)player.Path);

            return matchedEpisode;
        }

        public void ActivateMission(uint missionId)
        {
            PathMissionEntry pathMissionEntry = GameTableManager.PathMission.GetEntry(missionId);
            if (pathMissionEntry == null)
                throw new ArgumentException($"Mission ID {missionId} did not match any PathMissionEntry");

            ActivateMission(pathMissionEntry);
        }

        public void ActivateMission(PathMissionEntry pathMissionEntry)
        {
            List<PathMission> missionsToSend = new List<PathMission>();

            KeyValuePair<uint, PathMission> matchingMission = missions.FirstOrDefault(x => x.Value.Id == pathMissionEntry.Id);
            if (matchingMission.Value != null)
            {
                missionsToSend.Add(matchingMission.Value);
                SendServerPathMissionActivate(missionsToSend.ToArray());
            }
        }

        /// <summary>
        /// Unlocks a <see cref="PathMissionEntry"/> for the Player
        /// </summary>
        /// <param name="missionId"></param>
        public void UnlockMission(uint missionId)
        {
            PathMissionEntry pathMissionEntry = GameTableManager.PathMission.GetEntry(missionId);
            if (pathMissionEntry == null)
                throw new ArgumentException($"Mission ID {missionId} did not match any PathMissionEntry");

            UnlockMission(pathMissionEntry);
        }

        /// <summary>
        /// Unlocks a <see cref="PathMissionEntry"/> for the Player
        /// </summary>
        /// <param name="pathMissionEntry"></param>
        public void UnlockMission(PathMissionEntry pathMissionEntry)
        {
            KeyValuePair<uint, PathMission> matchingMission = missions.FirstOrDefault(x => x.Value.Id == pathMissionEntry.Id);
            if (matchingMission.Value != null)
            {
                matchingMission.Value.State = 1;
                missions[matchingMission.Key] = matchingMission.Value;

                SendServerPathMissionUpdae(matchingMission.Value);
            }
        }

        public void UpdateMission(uint missionId, uint progress, uint state)
        {
            PathMissionEntry pathMissionEntry = GameTableManager.PathMission.GetEntry(missionId);
            if (pathMissionEntry == null)
                throw new ArgumentException($"Mission ID {missionId} did not match any PathMissionEntry");

            UpdateMission(pathMissionEntry, progress, state);
        }

        public void UpdateMission(PathMissionEntry pathMissionEntry, uint progress, uint state)
        {

            KeyValuePair<uint, PathMission> matchingMission = missions.FirstOrDefault(x => x.Value.Id == pathMissionEntry.Id);
            if (matchingMission.Value != null)
            {
                matchingMission.Value.Progress = Math.Clamp(matchingMission.Value.Progress += progress, 0, 100);
                if (matchingMission.Value.Progress >= 100)
                {
                    matchingMission.Value.Completed = true;
                    matchingMission.Value.Progress = 0;
                }

                matchingMission.Value.State = state;
                missions[matchingMission.Key] = matchingMission.Value;

                SendServerPathMissionUpdae(matchingMission.Value);
            }
        }

        /// <summary>
        ///Update <see cref="CharacterCurrency"/> with supplied amount.
        /// </summary>
        //private void CurrencyAmountUpdate(Currency currency, ulong amount)
        //{
        //    if (currency == null)
        //        throw new ArgumentNullException();

        //    currency.Amount = amount;

        //    player.Session.EnqueueMessageEncrypted(new ServerPlayerCurrencyChanged
        //    {
        //        CurrencyId = (byte)currency.Id,
        //        Amount = currency.Amount,
        //    });
        //}

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        //public void CurrencyAddAmount(byte currencyId, ulong amount)
        //{
        //    CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
        //    if (currencyEntry == null)
        //        throw new ArgumentNullException();

        //    CurrencyAddAmount(currencyEntry, amount);
        //}

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        //public void CurrencyAddAmount(CurrencyTypeEntry currencyEntry, ulong amount)
        //{
        //    if (currencyEntry == null)
        //        throw new ArgumentNullException();

        //    if (!currencies.TryGetValue((byte)currencyEntry.Id, out Currency currency))
        //        CurrencyCreate(currencyEntry, (ulong)amount);
        //    else
        //    {
        //        amount += currency.Amount;
        //        if (currency.Entry.CapAmount > 0)
        //            amount = Math.Min(amount + currency.Amount, currency.Entry.CapAmount);
        //        CurrencyAmountUpdate(currency, amount);
        //    }
        //}

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        //public void CurrencySubtractAmount(byte currencyId, ulong amount)
        //{
        //    CurrencyTypeEntry currencyEntry = GameTableManager.CurrencyType.GetEntry(currencyId);
        //    if (currencyEntry == null)
        //        throw new ArgumentNullException();

        //    CurrencySubtractAmount(currencyEntry, amount);
        //}

        /// <summary>
        /// Create a new <see cref="CharacterCurrency"/>.
        /// </summary>
        //public void CurrencySubtractAmount(CurrencyTypeEntry currencyEntry, ulong amount)
        //{
        //    if (currencyEntry == null)
        //        throw new ArgumentNullException();

        //    if (!currencies.TryGetValue((byte)currencyEntry.Id, out Currency currency))
        //        throw new ArgumentException($"Cannot create currency {currencyEntry.Id} with a negative amount!");
        //    if (currency.Amount < amount)
        //        throw new ArgumentException($"Trying to remove more currency {currencyEntry.Id} than the player has!");
        //    CurrencyAmountUpdate(currency, currency.Amount - amount);
        //}

        //public Currency GetCurrency(uint currencyId)
        //{
        //    return GetCurrency((byte)currencyId);
        //}

        //public Currency GetCurrency(byte currencyId)
        //{
        //    if (!currencies.TryGetValue(currencyId, out Currency currency))
        //        return CurrencyCreate(currencyId);
        //    return currency;
        //}

        public void Save(CharacterContext context)
        {
            log.Debug($"PathMissionManager.Save called");
            foreach (PathEpisode pathEpisode in episodes.Values)
                pathEpisode.Save(context);

            foreach (PathMission pathMission in missions.Values)
                pathMission.Save(context);
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        //public IEnumerator<Currency> GetEnumerator()
        //{
        //    return currencies.Values.GetEnumerator();
        //}

        private void SendServerPathEpisodeProgress(uint episodeId, PathMission[] pathMissions)
        {
            List<ServerPathEpisodeProgress.Mission> missionProgress = new List<ServerPathEpisodeProgress.Mission>();

            foreach(PathMission pathMission in pathMissions)
                missionProgress.Add(new ServerPathEpisodeProgress.Mission
                {
                    MissionId = pathMission.Id,
                    Completed = pathMission.Completed,
                    Userdata = pathMission.Progress,
                    Statedata = pathMission.State
                });

            player.Session.EnqueueMessageEncrypted(new ServerPathEpisodeProgress
            {
                EpisodeId = (ushort)episodeId,
                Missions = missionProgress
            });
        }

        private void SendServerPathCurrentEpisode(uint episodeId)
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathCurrentEpisode
            {
                EpisodeId = (ushort)episodeId
            });
        }

        private void SendServerPathMissionActivate(PathMission[] pathMissions, byte reason = 1, uint giver = 0)
        {
            List<ServerPathMissionActivate.Mission> missionList = new List<ServerPathMissionActivate.Mission>();

            foreach (PathMission pathMission in pathMissions)
            {
                //log.Debug($"Activating {pathMission.Id}, {pathMission.Completed}, {pathMission.Progress}, {pathMission.State}");
                missionList.Add(new ServerPathMissionActivate.Mission
                {
                    MissionId = pathMission.Id,
                    Completed = pathMission.Completed,
                    Userdata = pathMission.Progress,
                    Statedata = pathMission.State,
                    Reason = reason,
                    Giver = giver
                });
            }

            player.Session.EnqueueMessageEncrypted(new ServerPathMissionActivate
            {
                Missions = missionList       
            });
        }

        private void SendServerPathMissionUpdae(PathMission pathMission)
        {
            player.Session.EnqueueMessageEncrypted(new ServerPathMissionUpdate
            {
                MissionId = (ushort)pathMission.Id,
                Completed = pathMission.Completed,
                Userdata = pathMission.Progress,
                Statedata = pathMission.State
            });
        }
    }
}
