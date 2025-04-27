using CustomerSupportAssistant.Domain.Entities;

namespace CustomerSupportAssistant.Persistence.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
}
