namespace RoguesGambitServer.Api.Dtos
{
    public sealed class ErrorResponse
    {
        public string ErrorCode { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
