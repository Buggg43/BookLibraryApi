using Microsoft.EntityFrameworkCore;
using BookLibraryApi.Data;
using Microsoft.AspNetCore.Hosting.Server;
using BookLibraryApi.Middlewear;

namespace BookLibraryApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.  
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi  
            builder.Services.AddOpenApi();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Fix for CS1009: Unrecognized escape sequence  


            builder.Services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseSqlServer(@"Server=.\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;");

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.  
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddlewear>();
            app.MapControllers();
            app.UseHttpsRedirection();
            

            

                


            app.Run();
        }
    }
}
