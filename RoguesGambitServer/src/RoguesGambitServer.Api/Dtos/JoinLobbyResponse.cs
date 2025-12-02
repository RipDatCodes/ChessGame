using System;

namespace RoguesGambitServer.Api.Dtos
{
    public sealed class JoinLobbyResponse
    {
        public Guid LobbyId { get; set; }

        public ConnectionInfoDto ConnectionInfo { get; set; } = new();

        public LobbyDto Lobby { get; set; } = new();
    }
}
