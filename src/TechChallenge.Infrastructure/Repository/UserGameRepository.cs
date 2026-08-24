using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infrastructure.Repository;

public class UserGameRepository : IUserGameRepository
{
    private readonly ApplicationDbContext _context;

    public UserGameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserGame?> GetByUserAndGameAsync(int userId, int gameId)
    {
        return await _context.UserGames.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == gameId);
    }

    public async Task<List<UserGame>> GetByUserIdAsync(int userId)
    {
        return await _context.UserGames.Include(ug => ug.Game).Where(ug => ug.UserId == userId).ToListAsync();
    }

    public async Task<UserGame> AddAsync(UserGame userGame)
    {
        _context.UserGames.Add(userGame);
        await _context.SaveChangesAsync();

        return userGame;
    }
}