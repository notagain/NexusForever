using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PathEpisode : ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public PathEpisodeEntry Entry { get; set; }
        public uint Id { get; }
        public ulong CharacterId { get; set; }

        public bool RewardReceived
        {
            get => rewardReceived;
            set
            {
                if(value != rewardReceived)
                {
                    rewardReceived = value;
                    saveMask |= PathEpisodeSaveMask.RewardChange;
                }
            }
        }

        private bool rewardReceived;

        private PathEpisodeSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="PathEpisode"/> from an existing database model.
        /// </summary>
        public PathEpisode(CharacterPathEpisode model)
        {
            CharacterId = model.Id;
            Id = model.EpisodeId;
            Entry = GameTableManager.PathEpisode.GetEntry(model.EpisodeId);
            RewardReceived = model.RewardReceived;

            saveMask = PathEpisodeSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="PathEpisode"/> from an <see cref="PathEpisodeEntry"/> template.
        /// </summary>
        public PathEpisode(ulong owner, PathEpisodeEntry entry, bool rewardReceived = false)
        {
            Id = entry.Id;
            CharacterId = owner;
            Entry = entry;
            RewardReceived = rewardReceived;

            saveMask = PathEpisodeSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == PathEpisodeSaveMask.None)
                return;

            if ((saveMask & PathEpisodeSaveMask.Create) != 0)
            {
                // Currency doesn't exist in database, all infomation must be saved
                context.Add(new CharacterPathEpisode
                {
                    Id = CharacterId,
                    EpisodeId = Entry.Id,
                    RewardReceived = RewardReceived,
                });
            }
            //else
            //{
            //    // Currency already exists in database, save only data that has been modified
            //    var model = new CharacterCurrency
            //    {
            //        Id = CharacterId,
            //        CurrencyId = (byte)Entry.Id,
            //    };

            //    // could probably clean this up with reflection, works for the time being
            //    EntityEntry<CharacterCurrency> entity = context.Attach(model);
            //    if ((saveMask & CurrencySaveMask.Amount) != 0)
            //    {
            //        model.Amount = Amount;
            //        entity.Property(p => p.Amount).IsModified = true;
            //    }
            //}

            saveMask = PathEpisodeSaveMask.None;
        }
    }
}
