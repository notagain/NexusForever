using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Path")]
    public class PathMissionManagerCommandHandler : CommandCategory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public PathMissionManagerCommandHandler()
            : base(true, "pm", "pathmission")
        {
        }

        [SubCommandHandler("episodeadd", "episodeId - Creates episode for player")]
        public Task AddPathMissionManagerEpisodeSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
            {
                context.SendErrorAsync($"Parameter mising.");
                return Task.CompletedTask;
            }
                

            uint episodeId = 0;
            if (parameters.Length > 0)
                episodeId = uint.Parse(parameters[0]);

            if(episodeId > 0)
            {
                PathEpisode pathEpisode = context.Session.Player.PathMissionManager.PathEpisodeCreate(episodeId);
                context.SendMessageAsync($"Executing PathEpisodeCreate with episode {pathEpisode.Id}");
            }
            else
            {
                context.SendErrorAsync($"Unknown episode: {episodeId}");
            }

            return Task.CompletedTask;
        }

        [SubCommandHandler("missionactivate", "missionId - Unlocks mission for player")]
        public Task AddPathMissionManagerMissionActivateSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
            {
                context.SendErrorAsync($"Parameter mising.");
                return Task.CompletedTask;
            }

            uint missionId = 0;
            if (parameters.Length > 0)
                missionId = uint.Parse(parameters[0]);

            if (missionId > 0)
            {
                context.Session.Player.PathMissionManager.ActivateMission(missionId);
            }
            else
            {
                context.SendErrorAsync($"Unknown episode: {missionId}");
            }

            return Task.CompletedTask;
        }

        [SubCommandHandler("missionunlock", "missionId - Unlocks mission for player")]
        public Task AddPathMissionManagerMissionUnlockSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
            {
                context.SendErrorAsync($"Parameter mising.");
                return Task.CompletedTask;
            }

            uint missionId = 0;
            if (parameters.Length > 0)
                missionId = uint.Parse(parameters[0]);

            if (missionId > 0)
            {
                context.Session.Player.PathMissionManager.UnlockMission(missionId);
            }
            else
            {
                context.SendErrorAsync($"Unknown episode: {missionId}");
            }

            return Task.CompletedTask;
        }

        [SubCommandHandler("missionupdate", "missionId progress state - Unlocks mission for player")]
        public Task AddPathMissionManagerMissionUpdateSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 2)
            {
                context.SendErrorAsync($"Parameter mising.");
                return Task.CompletedTask;
            }

            uint missionId = 0;
            if (parameters.Length > 0)
                missionId = uint.Parse(parameters[0]);

            uint progress = 0;
            if (parameters.Length > 1)
                progress = uint.Parse(parameters[1]);

            uint state = 0;
            if (parameters.Length > 2)
                state = uint.Parse(parameters[2]);

            if (missionId > 0)
            {
                context.Session.Player.PathMissionManager.UpdateMission(missionId, progress, state);
            }
            else
            {
                context.SendErrorAsync($"Unknown episode: {missionId}");
            }

            return Task.CompletedTask;
        }
    }
}
