namespace NetworkManager.Domain.Aggregates.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);

    //List<UserConfig> GetActiveUsers();
    void UpdateUsage(string userId, long up, long down);
    bool CheckAndDisableExpiredUsers();
}
