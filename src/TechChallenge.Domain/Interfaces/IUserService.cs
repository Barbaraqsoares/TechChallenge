using TechChallenge.Domain.Entity;

namespace TechChallenge.Domain.Interfaces;

public interface IUserService
{
    Task<User> CreateAsync(User user);

    Task<User?> AuthenticateAsync(string login, string password);
}