using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Models.User;

namespace TechChallenge.Domain.Interfaces;

public interface IUserService
{
    Task<UserResponse> CreateAsync(RegisterUserRequest request);
    Task<User?> AuthenticateAsync(string login,string password);
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    //Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request);
}