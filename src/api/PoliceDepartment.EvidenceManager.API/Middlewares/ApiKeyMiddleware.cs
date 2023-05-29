namespace PoliceDepartment.EvidenceManager.API.Middlewares
{
    public sealed class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;

        public ApiKeyMiddleware(RequestDelegate next,
                                IConfiguration configuration)
        {
            _next = next;
            _apiKey = configuration["ApiKey"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.ToString().ToLower().Contains("swagger") &&
                context.Request.Headers["X-API-KEY"] != _apiKey)
            {
                throw new UnauthorizedAccessException("Access not permited: Invalid API-KEY");
            }

            await _next(context);
        }
    }
}
