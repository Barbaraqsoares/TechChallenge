using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infrastructure.Repository;

/// <summary>
/// Implementação genérica do repositório sobre o Entity Framework.
/// Cada repositório específico herda daqui e ganha o CRUD pronto, escrevendo
/// apenas as consultas próprias da sua entidade.
/// </summary>
public class EFRepository<T> : IRepository<T> where T : EntityBase
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public EFRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IList<T>> ObterTodos() =>
        await _dbSet.AsNoTracking().ToListAsync();

    public virtual async Task<T?> ObterPorId(int id) =>
        await _dbSet.FirstOrDefaultAsync(entidade => entidade.Id == id);

    public virtual async Task Cadastrar(T entidade)
    {
        await _dbSet.AddAsync(entidade);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Alterar(T entidade)
    {
        _dbSet.Update(entidade);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Deletar(int id)
    {
        var entidade = await ObterPorId(id);

        if (entidade is null)
        {
            return;
        }

        _dbSet.Remove(entidade);
        await _context.SaveChangesAsync();
    }
}
