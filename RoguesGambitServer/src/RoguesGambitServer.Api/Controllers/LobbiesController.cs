using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoguesGambitServer.Api.Dtos;
using RoguesGambitServer.Api.Mapping;
using RoguesGambitServer.Core.Interfaces;
using RoguesGambitServer.Core.Models;

namespace RoguesGambitServer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class LobbiesController : ControllerBase
    {
        private readonly ILobbyService _lobbyService;

        public LobbiesController(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateLobbyResponse>> CreateLobby(
            [FromBody] CreateLobbyRequest request,
            CancellationToken cancellationToken)
        {
            var visibility = ParseVisibility(request.Visibility);

            var lobby = await _lobbyService.CreateLobbyAsync(
                request.PlayerId,
                request.DisplayName,
                visibility,
                request.GameSettings.ToModel(),
                request.ConnectionInfo.ToModel(),
                request.MaxPlayers,
                cancellationToken);

            var response = new CreateLobbyResponse
            {
                Lobby = lobby.ToLobbyDto()
            };

            return CreatedAtAction(nameof(GetLobby), new { lobbyId = lobby.LobbyId }, response);
        }

        [HttpPost("{lobbyId:guid}/heartbeat")]
        public async Task<IActionResult> Heartbeat(
            Guid lobbyId,
            [FromBody] HeartbeatRequest request,
            CancellationToken cancellationToken)
        {
            await _lobbyService.HeartbeatAsync(lobbyId, request.PlayerId, cancellationToken);
            return Ok(new { Success = true });
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<LobbySummaryDto>>> ListLobbies(
            [FromQuery] string? status,
            [FromQuery] string? visibility,
            CancellationToken cancellationToken)
        {
            LobbyStatus? statusFilter = null;
            LobbyVisibility? visibilityFilter = null;

            if (!string.IsNullOrWhiteSpace(status))
            {
                statusFilter = Enum.Parse<LobbyStatus>(status, ignoreCase: true);
            }

            if (!string.IsNullOrWhiteSpace(visibility))
            {
                visibilityFilter = Enum.Parse<LobbyVisibility>(visibility, ignoreCase: true);
            }

            var lobbies = await _lobbyService.ListLobbiesAsync(
                statusFilter,
                visibilityFilter,
                cancellationToken);

            var summaries = new List<LobbySummaryDto>(lobbies.Count);
            foreach (var lobby in lobbies)
            {
                summaries.Add(lobby.ToLobbySummaryDto());
            }

            return Ok(summaries);
        }

        [HttpGet("{lobbyId:guid}")]
        public async Task<ActionResult<LobbyDto>> GetLobby(
            Guid lobbyId,
            CancellationToken cancellationToken)
        {
            var lobby = await _lobbyService.GetLobbyAsync(lobbyId, cancellationToken);
            return Ok(lobby.ToLobbyDto());
        }

        [HttpPost("{lobbyId:guid}/join")]
        public async Task<ActionResult<JoinLobbyResponse>> JoinLobby(
            Guid lobbyId,
            [FromBody] JoinLobbyRequest request,
            CancellationToken cancellationToken)
        {
            var lobby = await _lobbyService.JoinLobbyAsync(
                lobbyId,
                request.PlayerId,
                request.DisplayName,
                cancellationToken);

            var response = new JoinLobbyResponse
            {
                LobbyId = lobby.LobbyId,
                ConnectionInfo = lobby.ConnectionInfo.ToDto(),
                Lobby = lobby.ToLobbyDto()
            };

            return Ok(response);
        }

        [HttpPost("{lobbyId:guid}/leave")]
        public async Task<IActionResult> LeaveLobby(
            Guid lobbyId,
            [FromBody] LeaveLobbyRequest request,
            CancellationToken cancellationToken)
        {
            await _lobbyService.LeaveLobbyAsync(
                lobbyId,
                request.PlayerId,
                cancellationToken);

            return Ok(new { Success = true });
        }

        [HttpPost("{lobbyId:guid}/status")]
        public async Task<ActionResult<LobbyDto>> UpdateLobbyStatus(
            Guid lobbyId,
            [FromBody] UpdateLobbyStatusRequest request,
            CancellationToken cancellationToken)
        {
            var newStatus = Enum.Parse<LobbyStatus>(request.Status, ignoreCase: true);

            var lobby = await _lobbyService.UpdateLobbyStatusAsync(
                lobbyId,
                request.PlayerId,
                newStatus,
                cancellationToken);

            return Ok(lobby.ToLobbyDto());
        }

        [HttpDelete("{lobbyId:guid}")]
        public async Task<IActionResult> CloseLobby(
            Guid lobbyId,
            [FromBody] LeaveLobbyRequest request,
            CancellationToken cancellationToken)
        {
            await _lobbyService.CloseLobbyAsync(
                lobbyId,
                request.PlayerId,
                cancellationToken);

            return Ok(new { Success = true });
        }

        private static LobbyVisibility ParseVisibility(string visibility)
        {
            if (string.IsNullOrWhiteSpace(visibility))
            {
                return LobbyVisibility.Public;
            }

            return Enum.Parse<LobbyVisibility>(visibility, ignoreCase: true);
        }
    }
}
