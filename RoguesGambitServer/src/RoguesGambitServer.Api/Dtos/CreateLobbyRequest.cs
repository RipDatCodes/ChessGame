namespace RoguesGambitServer.Api.Dtos
{
    public sealed class CreateLobbyRequest
    {
        public string PlayerId { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Visibility { get; set; } = "Public";

        public GameSettingsDto GameSettings { get; set; } = new();

        public ConnectionInfoDto ConnectionInfo { get; set; } = new();

        public int MaxPlayers { get; set; } = 2;
    }
}
