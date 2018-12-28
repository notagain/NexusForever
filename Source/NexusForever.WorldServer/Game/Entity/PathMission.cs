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
    public class PathMission : ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public PathMissionEntry Entry { get; set; }
        public uint Id { get; }
        public ulong CharacterId { get; set; }

        public bool Completed
        {
            get => completed;
            set
            {
                if(value != completed)
                {
                    completed = value;
                    saveMask |= PathMissionSaveMask.Complete;
                }
            }
        }

        private bool completed;

        public uint Progress
        {
            get => progress;
            set
            {
                if (value != progress)
                {
                    progress = value;
                    saveMask |= PathMissionSaveMask.Progress;
                }
            }
        }

        private uint progress;

        public uint State
        {
            get => state;
            set
            {
                if (value != state)
                {
                    state = value;
                    saveMask |= PathMissionSaveMask.State;
                }
            }
        }

        private uint state;

        private PathMissionSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="PathMission"/> from an existing database model.
        /// </summary>
        public PathMission(CharacterPathMission model)
        {
            CharacterId = model.Id;
            Id = model.MissionId;
            Entry = GameTableManager.PathMission.GetEntry(model.MissionId);
            Completed = model.Completed;
            Progress = model.Progress;
            State = model.State;

            saveMask = PathMissionSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="PathMission"/> from an <see cref="PathMissionEntry"/> template.
        /// </summary>
        public PathMission(ulong owner, PathMissionEntry entry, bool completed = false, uint progress = 0, uint state = 0)
        {
            Id = entry.Id;
            CharacterId = owner;
            Entry = entry;
            Completed = completed;
            Progress = progress;
            State = state;

            saveMask = PathMissionSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == PathMissionSaveMask.None)
                return;

            if ((saveMask & PathMissionSaveMask.Create) != 0)
            {
                // Currency doesn't exist in database, all infomation must be saved
                context.Add(new CharacterPathMission
                {
                    Id = CharacterId,
                    MissionId = Entry.Id,
                    Completed = Completed,
                    Progress = Progress,
                    State = State
                });
            }
            else
            {
                // Currency already exists in database, save only data that has been modified
                var model = new CharacterPathMission
                {
                    Id = CharacterId,
                    MissionId = Entry.Id
                };

                // could probably clean this up with reflection, works for the time being
                EntityEntry<CharacterPathMission> entity = context.Attach(model);
                if ((saveMask & PathMissionSaveMask.State) != 0)
                {
                    model.State = state;
                    entity.Property(p => p.State).IsModified = true;
                }
            }

            saveMask = PathMissionSaveMask.None;
        }
    }
}
