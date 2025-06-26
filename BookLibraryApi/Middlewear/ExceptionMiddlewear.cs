using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BookLibraryApi.Middlewear
{
    public class ExceptionMiddlewear
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddlewear> _logger;
        public ExceptionMiddlewear(RequestDelegate next, ILogger<ExceptionMiddlewear> logger) 
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
            catch(Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var error = new {message = "Wewnętrzny błąd serwera.", details = ex.Message};

                _logger.LogError(ex, "Wewnętrzny błąd serwera.");
                
                await context.Response.WriteAsJsonAsync(error);
            }
            
        }
    }
}
