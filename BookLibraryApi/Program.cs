using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BookLibraryApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BookLibraryApi.Services;
using System.Text;
using Microsoft.AspNetCore.Identity;
using BookLibraryApi.Models;
using FluentValidation;
using MediatR.Extensions.FluentValidation.AspNetCore;
using FluentValidation.AspNetCore;
using BookLibraryApi.Middleware;
using BookLibraryApi.Mapper;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookLibraryApi
{
    public partial class Program
    {
        private static readonly InMemoryDatabaseRoot _testDbRoot = new(); // ✅ współdzielona baza w testach

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (!builder.Environment.EnvironmentName.Equals("IntegrationTesting", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddSwaggerGen();
            }

            builder.Services.AddControllers();
            builder.Services.AddScoped<PasswordHasher<User>>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"] ?? "";
            }, typeof(Program).Assembly);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "this_is_a_super_secret_test_key_123"))
                    };
                });

            builder.Services.AddHostedService<RefreshTokenCleanUpService>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            if (builder.Environment.EnvironmentName == "IntegrationTesting")
            {
                builder.Services.AddDbContext<LibraryDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb", _testDbRoot));
            }
            else
            {
                builder.Services.AddDbContext<LibraryDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            var app = builder.Build();

            if (!app.Environment.EnvironmentName.Equals("IntegrationTesting", StringComparison.OrdinalIgnoreCase))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            app.MapControllers();
            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
