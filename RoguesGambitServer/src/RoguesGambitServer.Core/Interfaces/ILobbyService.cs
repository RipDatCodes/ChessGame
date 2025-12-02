using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RoguesGambitServer.Core.Models;

namespace RoguesGambitServer.Core.Interfaces
{
    /// <summary>
    /// Domain-level API for lobby operations.
    /// The Web API layer calls into this interface and maps results to HTTP/JSON.
    /// </summary>
    public interface ILobbyService
    {
        Task<Lobby> CreateLobbyAsync(
            string playerId,
            string displayName,
            LobbyVisibility visibility,
            GameSettings gameSettings,
            ConnectionInfo connectionInfo,
            int maxPlayers,
            CancellationToken cancellationToken);

        Task HeartbeatAsync(
            Guid lobbyId,
            string playerId,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Lobby>> ListLobbiesAsync(
            LobbyStatus? status,
            LobbyVisibility? visibility,
            CancellationToken cancellationToken);

        Task<Lobby> GetLobbyAsync(
            Guid lobbyId,
            CancellationToken cancellationToken);

        Task<Lobby> JoinLobbyAsync(
            Guid lobbyId,
            string playerId,
            string displayName,
            CancellationToken cancellationToken);

        Task LeaveLobbyAsync(
            Guid lobbyId,
            string playerId,
            CancellationToken cancellationToken);

        Task<Lobby> UpdateLobbyStatusAsync(
            Guid lobbyId,
            string playerId,
            LobbyStatus newStatus,
            CancellationToken cancellationToken);

        Task CloseLobbyAsync(
            Guid lobbyId,
            string playerId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Removes any lobbies whose last heartbeat is older than the given timeout.
        /// Intended to be called by a background cleanup job.
        /// Returns the number of lobbies removed.
        /// </summary>
        Task<int> CleanupExpiredLobbiesAsync(
            TimeSpan heartbeatTimeout,
            CancellationToken cancellationToken);
    }
}
