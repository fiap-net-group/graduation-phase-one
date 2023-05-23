using Newtonsoft.Json;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using System.Net;

namespace PoliceDepartment.EvidenceManager.API.Middlewares
{
    public sealed class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;

        public ErrorHandlerMiddleware(RequestDelegate next,
                                      ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Businness error caught by exception", ("exception", ex));

                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error caught by middleware", ex);

                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, BusinessException exception)
        {
            var code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new BaseResponse().AsError(exception.Message));

            return ErrorResponse(context, result, code);
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            var code = HttpStatusCode.InternalServerError;

            return ErrorResponse(context, code);
        }

        private static Task ErrorResponse(HttpContext context, HttpStatusCode code)
        {
            var result = JsonConvert.SerializeObject(new BaseResponse().AsError("Unexpected error"));

            return ErrorResponse(context, result, code);
        }

        private static Task ErrorResponse(HttpContext context, string result, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
