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

    public async Task<UserResponse> CreateAsync(
        RegisterUserRequest request
    )
    {
        var existingLogin =
            await _userRepository.GetByLoginAsync(request.Login);

        if (existingLogin != null)
        {
            throw new InvalidOperationException("Login já cadastrado.");
        }

        var existingEmail =
            await _userRepository.GetByEmailAsync(request.Email);

        if (existingEmail != null)
        {
            throw new InvalidOperationException("E-mail já cadastrado.");
        }

        var user = new User(
            request.Name,
            PerfilEnum.Client,
            request.Email,
            request.Password,
            request.Login
        );

        var createdUser =
            await _userRepository.AddAsync(user);

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

        // Comparação simples (texto puro)
        return user.Password == password ? user : null;
    }
}