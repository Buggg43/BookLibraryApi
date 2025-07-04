using Microsoft.EntityFrameworkCore;
using BookLibraryApi.Data;
using Microsoft.AspNetCore.Hosting.Server;
using BookLibraryApi.Middlewear;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BookLibraryApi.Services;
using System.Text;
using Microsoft.AspNetCore.Identity;
using BookLibraryApi.Models;

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
            builder.Services.AddScoped<PasswordHasher<User>>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))

                };
            });
            builder.Services.AddHostedService<RefreshTokenCleanUpService>();



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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddlewear>();
            app.MapControllers();
            app.UseHttpsRedirection();







            app.Run();
        }
    }
}
