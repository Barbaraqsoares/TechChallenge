using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> CreateAsync(User user)
    {
        var existingLogin = await _userRepository.GetByLoginAsync(user.Login);

        if (existingLogin != null)
        {
            throw new InvalidOperationException("Login já cadastrado.");
        }

        var existingEmail = await _userRepository.GetByEmailAsync(user.Email);

        if (existingEmail != null)
        {
            throw new InvalidOperationException("E-mail já cadastrado.");
        }

        // Armazenar senha em texto (sem hashing) conforme pedido: usar Password quando fornecida
        var plain = !string.IsNullOrEmpty(user.Password) ? user.Password : user.Password;
        user.Password = plain ?? string.Empty;
        user.Password = null;

        return await _userRepository.AddAsync(user);
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