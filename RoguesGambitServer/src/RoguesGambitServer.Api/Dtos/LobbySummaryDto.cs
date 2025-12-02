using System;

namespace RoguesGambitServer.Api.Dtos
{
    public sealed class LobbySummaryDto
    {
        public Guid LobbyId { get; set; }

        public string HostDisplayName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Visibility { get; set; } = string.Empty;

        public GameSettingsDto GameSettings { get; set; } = new();

        public int CurrentPlayerCount { get; set; }

        public int MaxPlayers { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
