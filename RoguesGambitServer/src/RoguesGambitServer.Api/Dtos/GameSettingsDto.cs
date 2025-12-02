namespace RoguesGambitServer.Api.Dtos
{
    public sealed class GameSettingsDto
    {
        public string TimeControl { get; set; } = "10+0";

        public string Variant { get; set; } = "standard";

        public bool Rated { get; set; }
    }
}
