using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models.User;

namespace TechChallenge.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> CreateAsync(RegisterUserRequest request)
    {
        var existingLogin = await _userRepository.GetByLoginAsync(request.Login);

        if (existingLogin != null)
            throw new InvalidOperationException("Login já cadastrado.");

        var existingEmail = await _userRepository.GetByEmailAsync(request.Email);

        if (existingEmail != null)
            throw new InvalidOperationException("E-mail já cadastrado.");

        var user = new User(
            request.Name,
            request.Email,
            request.Login,
            request.Password,
            PerfilEnum.Client
        );

        var createdUser = await _userRepository.AddAsync(user);

        return new UserResponse
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Email = createdUser.Email,
            Login = createdUser.Login,
            Perfil = createdUser.Perfil,
            CreatedAt = createdUser.CreatedAt
        };
    }

    public async Task<User?> AuthenticateAsync(string login, string password)
    {
        var user = await _userRepository.GetByLoginAsync(login);

        if (user == null)
            return null;

        var passwordIsValid =BCrypt.Net.BCrypt.Verify(password, user.Password);

        return passwordIsValid ? user: null;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Login = user.Login,
            Perfil = user.Perfil,
            CreatedAt = user.CreatedAt
        });
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return null;

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Login = user.Login,
            Perfil = user.Perfil,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return false;

        await _userRepository.DeleteAsync(user);

        return true;
    }
}