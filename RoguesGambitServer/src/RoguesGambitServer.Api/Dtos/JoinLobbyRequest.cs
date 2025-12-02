namespace RoguesGambitServer.Api.Dtos
{
    public sealed class JoinLobbyRequest
    {
        public string PlayerId { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
    }
}
