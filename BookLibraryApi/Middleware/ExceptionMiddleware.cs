using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BookLibraryApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) 
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var error = new 
                {
                    status = 500,
                    message = "Unresolved exception."
                };
                var json = JsonSerializer.Serialize(error);

                await context.Response.WriteAsync(json);
            }
            
        }
    }
}
