using Microsoft.EntityFrameworkCore;
using NetworkManager.Domain.Aggregates.Users;

namespace NetworkMangar.Infrastructure.Users;
public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddAsync(User user)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.AsNoTracking().ToListAsync(); 
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FindAsync(id);
    }

    public async Task UpdateTrafficUsageAsync(string uuid, long upload, long download)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);

        if (user != null)
        {
            user.UploadUsage += upload;
            user.DownloadUsage += download;

            if ((user.UploadUsage + user.DownloadUsage) >= user.MonthlyLimit)
            {
                user.IsActive = false; 
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(User user)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public void UpdateUsage(string userId, long up, long down)
    {
        throw new NotImplementedException();
    }

    public bool CheckAndDisableExpiredUsers()
    {
        throw new NotImplementedException();
    }
}
