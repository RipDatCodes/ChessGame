using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoguesGambitServer.Core.Errors;
using RoguesGambitServer.Core.Exceptions;
using RoguesGambitServer.Core.Interfaces;
using RoguesGambitServer.Core.Models;

namespace RoguesGambitServer.Core.Services
{
    /// <summary>
    /// Domain-level implementation of lobby operations.
    /// </summary>
    public sealed class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _repository;
        private readonly IClock _clock;

        public LobbyService(ILobbyRepository repository, IClock clock)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public async Task<Lobby> CreateLobbyAsync(
            string playerId,
            string displayName,
            LobbyVisibility visibility,
            GameSettings gameSettings,
            ConnectionInfo connectionInfo,
            int maxPlayers,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "PlayerId is required.");
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "DisplayName is required.");
            }

            if (gameSettings == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "GameSettings is required.");
            }

            if (connectionInfo == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "ConnectionInfo is required.");
            }

            if (maxPlayers < 1)
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "MaxPlayers must be at least 1.");
            }

            var now = _clock.UtcNow;

            var lobby = new Lobby
            {
                LobbyId = Guid.NewGuid(),
                HostPlayerId = playerId,
                HostDisplayName = displayName,
                Visibility = visibility,
                Status = LobbyStatus.WaitingForOpponent,
                GameSettings = gameSettings,
                ConnectionInfo = connectionInfo,
                MaxPlayers = maxPlayers,
                CreatedAtUtc = now,
                LastHeartbeatAtUtc = now
            };

            lobby.Players.Add(new PlayerSummary
            {
                PlayerId = playerId,
                DisplayName = displayName
            });

            return await _repository.CreateAsync(lobby, cancellationToken).ConfigureAwait(false);
        }

        public async Task HeartbeatAsync(
            Guid lobbyId,
            string playerId,
            CancellationToken cancellationToken)
        {
            var lobby = await _repository.GetByIdAsync(lobbyId, cancellationToken).ConfigureAwait(false);

            if (lobby == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyNotFound,
                    "Lobby not found.");
            }

            if (!string.Equals(lobby.HostPlayerId, playerId, StringComparison.Ordinal))
            {
                throw new LobbyException(
                    LobbyErrorCode.NotHost,
                    "Only the host may send heartbeat signals for a lobby.");
            }

            lobby.LastHeartbeatAtUtc = _clock.UtcNow;

            await _repository.UpdateAsync(lobby, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<Lobby>> ListLobbiesAsync(
            LobbyStatus? status,
            LobbyVisibility? visibility,
            CancellationToken cancellationToken)
        {
            // For public lobby browsing you will typically call this with:
            // status = LobbyStatus.WaitingForOpponent, visibility = LobbyVisibility.Public
            var lobbies = await _repository.QueryAsync(status, visibility, cancellationToken).ConfigureAwait(false);

            return lobbies;
        }

        public async Task<Lobby> GetLobbyAsync(
            Guid lobbyId,
            CancellationToken cancellationToken)
        {
            var lobby = await _repository.GetByIdAsync(lobbyId, cancellationToken).ConfigureAwait(false);

            if (lobby == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyNotFound,
                    "Lobby not found.");
            }

            return lobby;
        }

        public async Task<Lobby> JoinLobbyAsync(
            Guid lobbyId,
            string playerId,
            string displayName,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "PlayerId is required.");
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "DisplayName is required.");
            }

            var lobby = await _repository.GetByIdAsync(lobbyId, cancellationToken).ConfigureAwait(false);

            if (lobby == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyNotFound,
                    "Lobby not found.");
            }

            if (lobby.Status != LobbyStatus.WaitingForOpponent)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyNotJoinable,
                    "Lobby is not in a joinable state.");
            }

            if (lobby.Players.Count >= lobby.MaxPlayers)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyFull,
                    "Lobby is full.");
            }

            // If the player is already in the lobby, just return it as-is.
            if (!lobby.Players.Any(p => string.Equals(p.PlayerId, playerId, StringComparison.Ordinal)))
            {
                lobby.Players.Add(new PlayerSummary
                {
                    PlayerId = playerId,
                    DisplayName = displayName
                });
            }

            // For chess, once the second player joins we may consider the lobby "in progress".
            if (lobby.Players.Count >= lobby.MaxPlayers)
            {
                lobby.Status = LobbyStatus.InProgress;
            }

            await _repository.UpdateAsync(lobby, cancellationToken).ConfigureAwait(false);

            return lobby;
        }

        public async Task LeaveLobbyAsync(
            Guid lobbyId,
            string playerId,
            CancellationToken cancellationToken)
        {
            var lobby = await _repository.GetByIdAsync(lobbyId, cancellationToken).ConfigureAwait(false);

            if (lobby == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyNotFound,
                    "Lobby not found.");
            }

            if (string.Equals(lobby.HostPlayerId, playerId, StringComparison.Ordinal))
            {
                // Host leaving is equivalent to closing the lobby.
                await _repository.DeleteAsync(lobbyId, cancellationToken).ConfigureAwait(false);
                return;
            }

            var removed = lobby
                .Players
                .RemoveAll(p => string.Equals(p.PlayerId, playerId, StringComparison.Ordinal));

            if (removed > 0)
            {
                await _repository.UpdateAsync(lobby, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<Lobby> UpdateLobbyStatusAsync(
            Guid lobbyId,
            string playerId,
            LobbyStatus newStatus,
            CancellationToken cancellationToken)
        {
            var lobby = await _repository.GetByIdAsync(lobbyId, cancellationToken).ConfigureAwait(false);

            if (lobby == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyNotFound,
                    "Lobby not found.");
            }

            if (!string.Equals(lobby.HostPlayerId, playerId, StringComparison.Ordinal))
            {
                throw new LobbyException(
                    LobbyErrorCode.NotHost,
                    "Only the host may update lobby status.");
            }

            lobby.Status = newStatus;

            await _repository.UpdateAsync(lobby, cancellationToken).ConfigureAwait(false);

            return lobby;
        }

        public async Task CloseLobbyAsync(
            Guid lobbyId,
            string playerId,
            CancellationToken cancellationToken)
        {
            var lobby = await _repository.GetByIdAsync(lobbyId, cancellationToken).ConfigureAwait(false);

            if (lobby == null)
            {
                throw new LobbyException(
                    LobbyErrorCode.LobbyNotFound,
                    "Lobby not found.");
            }

            if (!string.Equals(lobby.HostPlayerId, playerId, StringComparison.Ordinal))
            {
                throw new LobbyException(
                    LobbyErrorCode.NotHost,
                    "Only the host may close a lobby.");
            }

            await _repository.DeleteAsync(lobbyId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CleanupExpiredLobbiesAsync(
            TimeSpan heartbeatTimeout,
            CancellationToken cancellationToken)
        {
            if (heartbeatTimeout <= TimeSpan.Zero)
            {
                throw new LobbyException(
                    LobbyErrorCode.InvalidRequest,
                    "Heartbeat timeout must be greater than zero.");
            }

            var now = _clock.UtcNow;

            var allLobbies = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);

            var expired = allLobbies
                .Where(l => now - l.LastHeartbeatAtUtc > heartbeatTimeout)
                .ToList();

            foreach (var lobby in expired)
            {
                await _repository.DeleteAsync(lobby.LobbyId, cancellationToken).ConfigureAwait(false);
            }

            return expired.Count;
        }
    }
}
