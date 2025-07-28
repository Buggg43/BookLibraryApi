using BookLibraryApi;
using BookLibraryApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly InMemoryDatabaseRoot _dbRoot = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "this_is_a_super_secret_test_key_123",
                ["Jwt:Issuer"] = "test_issuer",
                ["Jwt:ExpireMinutes"] = "60"
            };
            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb", _dbRoot);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            db.Database.EnsureCreated(); 
        });

        return base.CreateHost(builder);
    }
}
