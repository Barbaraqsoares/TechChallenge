using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infrastructure.Repository;

public class JogoRepository : EFRepository<Jogo>, IJogoRepository
{
    public JogoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IList<Jogo>> ObterEmPromocao() =>
        await _context.Jogos
            .AsNoTracking()
            .Where(jogo => jogo.PercentualDesconto > 0)
            .OrderByDescending(jogo => jogo.PercentualDesconto)
            .ToListAsync();

    public async Task<IList<Jogo>> ObterPorGenero(string genero) =>
        await _context.Jogos
            .AsNoTracking()
            .Where(jogo => jogo.Genero == genero)
            .OrderBy(jogo => jogo.Titulo)
            .ToListAsync();
}
