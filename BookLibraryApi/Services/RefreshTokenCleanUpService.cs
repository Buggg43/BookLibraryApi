
using BookLibraryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Services
{
    public class RefreshTokenCleanUpService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public RefreshTokenCleanUpService(IServiceScopeFactory scopeFactory) 
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                var toRemoveTokens = await dbContext.RefreshTokens.Where(b => (DateTime.UtcNow - b.Expires) > TimeSpan.FromDays(30)).ToListAsync();

                dbContext.RefreshTokens.RemoveRange(toRemoveTokens);

                var expiredTokens = await dbContext.RefreshTokens.Where(b => b.Expires < DateTime.UtcNow && !b.isRevoked).ToListAsync();

                foreach (var token in expiredTokens)
                {
                    token.isRevoked = true;
                }
                await dbContext.SaveChangesAsync();
                


                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            }
        }
    }
}
