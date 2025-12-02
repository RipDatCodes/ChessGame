namespace RoguesGambitServer.Api.Dtos
{
    public sealed class CreateLobbyResponse
    {
        public LobbyDto Lobby { get; set; } = new();
    }
}
