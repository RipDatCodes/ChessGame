using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RoguesGambitServer.Api.Dtos;
using RoguesGambitServer.Core.Errors;
using RoguesGambitServer.Core.Exceptions;

namespace RoguesGambitServer.Api.Middleware
{
    public sealed class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (LobbyException ex)
            {
                await HandleLobbyExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception.");
                await HandleGenericExceptionAsync(context);
            }
        }

        private static async Task HandleLobbyExceptionAsync(HttpContext context, LobbyException ex)
        {
            var (statusCode, code) = MapLobbyErrorCode(ex.ErrorCode);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse
            {
                ErrorCode = code,
                Message = ex.Message
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }

        private static async Task HandleGenericExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse
            {
                ErrorCode = "InternalError",
                Message = "An unexpected error occurred."
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }

        private static (HttpStatusCode StatusCode, string Code) MapLobbyErrorCode(LobbyErrorCode errorCode)
        {
            return errorCode switch
            {
                LobbyErrorCode.LobbyNotFound => (HttpStatusCode.NotFound, nameof(LobbyErrorCode.LobbyNotFound)),
                LobbyErrorCode.NotHost => (HttpStatusCode.Forbidden, nameof(LobbyErrorCode.NotHost)),
                LobbyErrorCode.LobbyFull => (HttpStatusCode.Conflict, nameof(LobbyErrorCode.LobbyFull)),
                LobbyErrorCode.LobbyNotJoinable => (HttpStatusCode.Conflict, nameof(LobbyErrorCode.LobbyNotJoinable)),
                LobbyErrorCode.InvalidRequest => (HttpStatusCode.BadRequest, nameof(LobbyErrorCode.InvalidRequest)),
                _ => (HttpStatusCode.InternalServerError, "Unknown")
            };
        }
    }
}
