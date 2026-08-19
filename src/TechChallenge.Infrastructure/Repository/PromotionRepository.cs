using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infrastructure.Repository;

public class PromotionRepository : IPromotionRepository
{
    private readonly ApplicationDbContext _context;

    public PromotionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync()
    {
        return await _context.Promotions
            .Include(promotion => promotion.Games)
            .ToListAsync();
    }

    public async Task<Promotion?> GetByIdAsync(int id)
    {
        return await _context.Promotions
            .Include(promotion => promotion.Games)
            .FirstOrDefaultAsync(
                promotion => promotion.Id == id
            );
    }

    public async Task<Promotion> AddAsync(Promotion promotion)
    {
        await _context.Promotions.AddAsync(promotion);
        await _context.SaveChangesAsync();
        return promotion;
    }

    public async Task UpdateAsync(Promotion promotion)
    {
        _context.Promotions.Update(promotion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Promotion promotion)
    {
        _context.Promotions.Remove(promotion);

        await _context.SaveChangesAsync();
    }
}