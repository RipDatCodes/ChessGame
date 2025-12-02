using System;

namespace RoguesGambitServer.Core.Interfaces
{
    /// <summary>
    /// Abstraction over system time to make domain logic testable.
    /// </summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
