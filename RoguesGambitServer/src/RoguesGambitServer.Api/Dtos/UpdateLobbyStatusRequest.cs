namespace RoguesGambitServer.Api.Dtos
{
    public sealed class UpdateLobbyStatusRequest
    {
        public string PlayerId { get; set; } = string.Empty;

        // Expected values: "WaitingForOpponent", "InProgress", "Completed"
        public string Status { get; set; } = string.Empty;
    }
}
