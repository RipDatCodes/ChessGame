using System.Linq;
using RoguesGambitServer.Api.Dtos;
using RoguesGambitServer.Core.Models;
using CoreConnectionInfo = RoguesGambitServer.Core.Models.ConnectionInfo;

namespace RoguesGambitServer.Api.Mapping
{
    public static class LobbyMappingExtensions
    {
        public static LobbyDto ToLobbyDto(this Lobby lobby)
        {
            return new LobbyDto
            {
                LobbyId = lobby.LobbyId,
                HostPlayerId = lobby.HostPlayerId,
                HostDisplayName = lobby.HostDisplayName,
                Status = lobby.Status.ToString(),
                Visibility = lobby.Visibility.ToString(),
                GameSettings = lobby.GameSettings.ToDto(),
                ConnectionInfo = lobby.ConnectionInfo.ToDto(),
                Players = lobby.Players
                    .Select(p => new PlayerSummaryDto
                    {
                        PlayerId = p.PlayerId,
                        DisplayName = p.DisplayName
                    })
                    .ToList(),
                MaxPlayers = lobby.MaxPlayers,
                CreatedAtUtc = lobby.CreatedAtUtc,
                LastHeartbeatAtUtc = lobby.LastHeartbeatAtUtc
            };
        }

        public static LobbySummaryDto ToLobbySummaryDto(this Lobby lobby)
        {
            return new LobbySummaryDto
            {
                LobbyId = lobby.LobbyId,
                HostDisplayName = lobby.HostDisplayName,
                Status = lobby.Status.ToString(),
                Visibility = lobby.Visibility.ToString(),
                GameSettings = lobby.GameSettings.ToDto(),
                CurrentPlayerCount = lobby.Players.Count,
                MaxPlayers = lobby.MaxPlayers,
                CreatedAtUtc = lobby.CreatedAtUtc
            };
        }

        public static GameSettingsDto ToDto(this GameSettings model)
        {
            return new GameSettingsDto
            {
                TimeControl = model.TimeControl,
                Variant = model.Variant,
                Rated = model.Rated
            };
        }

        public static ConnectionInfoDto ToDto(this CoreConnectionInfo model)
        {
            return new ConnectionInfoDto
            {
                Mode = model.Mode,
                Address = model.Address,
                Metadata = model.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }

        public static GameSettings ToModel(this GameSettingsDto dto)
        {
            return new GameSettings
            {
                TimeControl = dto.TimeControl,
                Variant = dto.Variant,
                Rated = dto.Rated
            };
        }

        public static CoreConnectionInfo ToModel(this ConnectionInfoDto dto)
        {
            return new CoreConnectionInfo
            {
                Mode = dto.Mode,
                Address = dto.Address,
                Metadata = dto.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
    }
}
