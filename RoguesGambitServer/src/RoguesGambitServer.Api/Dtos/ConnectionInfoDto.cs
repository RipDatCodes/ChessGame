using System.Collections.Generic;

namespace RoguesGambitServer.Api.Dtos
{
    public sealed class ConnectionInfoDto
    {
        public string Mode { get; set; } = "direct";

        public string Address { get; set; } = string.Empty;

        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
