using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
}
