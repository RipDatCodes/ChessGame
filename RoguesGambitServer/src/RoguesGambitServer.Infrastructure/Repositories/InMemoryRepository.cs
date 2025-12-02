using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoguesGambitServer.Core.Interfaces;
using RoguesGambitServer.Core.Models;

namespace RoguesGambitServer.Infrastructure.Repositories
{
    /// <summary>
    /// Thread-safe in-memory implementation of ILobbyRepository.
    /// Suitable for development and small-scale hosting.
    /// </summary>
    public sealed class InMemoryLobbyRepository : ILobbyRepository
    {
        private readonly ConcurrentDictionary<Guid, Lobby> _lobbies;

        public InMemoryLobbyRepository()
        {
            _lobbies = new ConcurrentDictionary<Guid, Lobby>();
        }

        public Task<Lobby> CreateAsync(Lobby lobby, CancellationToken cancellationToken)
        {
            if (lobby == null)
            {
                throw new ArgumentNullException(nameof(lobby));
            }

            // We assume LobbyId is already set by the service.
            _lobbies[lobby.LobbyId] = lobby;
            return Task.FromResult(lobby);
        }

        public Task<Lobby?> GetByIdAsync(Guid lobbyId, CancellationToken cancellationToken)
        {
            _lobbies.TryGetValue(lobbyId, out var lobby);
            return Task.FromResult(lobby);
        }

        public Task UpdateAsync(Lobby lobby, CancellationToken cancellationToken)
        {
            if (lobby == null)
            {
                throw new ArgumentNullException(nameof(lobby));
            }

            _lobbies[lobby.LobbyId] = lobby;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid lobbyId, CancellationToken cancellationToken)
        {
            _lobbies.TryRemove(lobbyId, out _);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Lobby>> QueryAsync(
            LobbyStatus? status,
            LobbyVisibility? visibility,
            CancellationToken cancellationToken)
        {
            IEnumerable<Lobby> query = _lobbies.Values;

            if (status.HasValue)
            {
                query = query.Where(l => l.Status == status.Value);
            }

            if (visibility.HasValue)
            {
                query = query.Where(l => l.Visibility == visibility.Value);
            }

            var result = query
                .OrderBy(l => l.CreatedAtUtc)
                .ToList()
                .AsReadOnly();

            return Task.FromResult<IReadOnlyList<Lobby>>(result);
        }

        public Task<IReadOnlyList<Lobby>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = _lobbies.Values
                .OrderBy(l => l.CreatedAtUtc)
                .ToList()
                .AsReadOnly();

            return Task.FromResult<IReadOnlyList<Lobby>>(result);
        }
    }
}
