using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.ValueObject;

namespace TechChallenge.Infrastructure.Repository;

public class UsuarioRepository : EFRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> ObterPorEmail(string email)
    {
        // O Email é gravado em uma única coluna por um conversor, então a consulta
        // precisa comparar o Objeto de Valor inteiro: navegar até .Endereco dentro
        // do LINQ não é traduzível para SQL.
        var procurado = Email.Criar(email);

        return await _context.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Email == procurado);
    }

    public async Task<Usuario?> ObterComBiblioteca(int id) =>
        // Eager loading: traz o usuário, sua biblioteca e o jogo de cada item em
        // uma única consulta, evitando o problema N+1.
        await _context.Usuarios
            .Include(usuario => usuario.Biblioteca)
                .ThenInclude(aquisicao => aquisicao.Jogo)
            .FirstOrDefaultAsync(usuario => usuario.Id == id);

    public async Task<bool> ExisteComEmail(string email)
    {
        var procurado = Email.Criar(email);

        return await _context.Usuarios
            .AnyAsync(usuario => usuario.Email == procurado);
    }
}
