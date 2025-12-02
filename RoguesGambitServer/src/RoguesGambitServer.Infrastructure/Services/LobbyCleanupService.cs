using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoguesGambitServer.Core.Interfaces;
using RoguesGambitServer.Infrastructure.Configuration;

namespace RoguesGambitServer.Infrastructure.Services
{
    /// <summary>
    /// Background service that periodically removes expired lobbies
    /// based on the configured heartbeat timeout.
    /// </summary>
    public sealed class LobbyCleanupService : BackgroundService
    {
        private readonly ILobbyService _lobbyService;
        private readonly IOptions<LobbyOptions> _options;
        private readonly ILogger<LobbyCleanupService> _logger;

        public LobbyCleanupService(
            ILobbyService lobbyService,
            IOptions<LobbyOptions> options,
            ILogger<LobbyCleanupService> logger)
        {
            _lobbyService = lobbyService;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromSeconds(_options.Value.CleanupIntervalSeconds);
            var timeout = TimeSpan.FromSeconds(_options.Value.HeartbeatTimeoutSeconds);

            _logger.LogInformation(
                "Lobby cleanup service started. Interval={IntervalSeconds}s, Timeout={TimeoutSeconds}s",
                _options.Value.CleanupIntervalSeconds,
                _options.Value.HeartbeatTimeoutSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var removed = await _lobbyService.CleanupExpiredLobbiesAsync(timeout, stoppingToken)
                        .ConfigureAwait(false);

                    if (removed > 0)
                    {
                        _logger.LogInformation("Lobby cleanup removed {Count} expired lobbies.", removed);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running lobby cleanup.");
                }

                try
                {
                    await Task.Delay(interval, stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation("Lobby cleanup service stopped.");
        }
    }
}
