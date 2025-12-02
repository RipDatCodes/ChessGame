using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RoguesGambitServer.Core.Models;

namespace RoguesGambitServer.Core.Interfaces
{
    /// <summary>
    /// Storage abstraction for lobby persistence.
    /// Initial implementation will be in-memory, but this interface
    /// allows swapping in Redis or a database later without touching domain logic.
    /// </summary>
    public interface ILobbyRepository
    {
        Task<Lobby> CreateAsync(Lobby lobby, CancellationToken cancellationToken);

        Task<Lobby?> GetByIdAsync(Guid lobbyId, CancellationToken cancellationToken);

        Task UpdateAsync(Lobby lobby, CancellationToken cancellationToken);

        Task DeleteAsync(Guid lobbyId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns lobbies filtered by optional status and visibility.
        /// For public lobby list use status = WaitingForOpponent and visibility = Public.
        /// </summary>
        Task<IReadOnlyList<Lobby>> QueryAsync(
            LobbyStatus? status,
            LobbyVisibility? visibility,
            CancellationToken cancellationToken);

        /// <summary>
        /// Returns all lobbies, typically used only by background cleanup logic.
        /// </summary>
        Task<IReadOnlyList<Lobby>> GetAllAsync(CancellationToken cancellationToken);
    }
}
