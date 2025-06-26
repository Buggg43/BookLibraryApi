using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BookLibraryApi.Middlewear
{
    public class ExceptionMiddlewear
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddlewear(RequestDelegate next) 
        {
            _next = next;
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

                var error = new {message = "Wewnętrzny błąd serwera", details = ex.Message};
                
                await context.Response.WriteAsJsonAsync(error);
            }
            
        }
    }
}
