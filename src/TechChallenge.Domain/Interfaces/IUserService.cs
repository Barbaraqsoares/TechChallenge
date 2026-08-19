using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Models.User;

namespace TechChallenge.Domain.Interfaces;

public interface IUserService
{
    Task<UserResponse> CreateAsync(RegisterUserRequest request);
    Task<User?> AuthenticateAsync(
        string login,
        string password
    );
}