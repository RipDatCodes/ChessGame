using System;
using RoguesGambitServer.Core.Interfaces;

namespace RoguesGambitServer.Infrastructure.Time
{
    /// <summary>
    /// Default clock implementation using DateTime.UtcNow.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
