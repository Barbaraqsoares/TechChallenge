using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository.Configuration;

public class UsuarioJogoConfiguration : IEntityTypeConfiguration<UsuarioJogo>
{
    public void Configure(EntityTypeBuilder<UsuarioJogo> builder)
    {
        builder.ToTable("UsuarioJogo");
        builder.HasKey(uj => uj.Id);

        builder.Property(uj => uj.Id).HasColumnType("INT").UseIdentityColumn();
        builder.Property(uj => uj.DataCriacao).HasColumnType("DATETIME").IsRequired();
        builder.Property(uj => uj.UsuarioId).HasColumnType("INT").IsRequired();
        builder.Property(uj => uj.JogoId).HasColumnType("INT").IsRequired();
        builder.Property(uj => uj.PrecoPago).HasColumnType("DECIMAL(10,2)").IsRequired();
        builder.Property(uj => uj.DataAquisicao).HasColumnType("DATETIME").IsRequired();

        builder.HasOne(uj => uj.Usuario)
            .WithMany(u => u.Biblioteca)
            .HasForeignKey(uj => uj.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uj => uj.Jogo)
            .WithMany(j => j.Aquisicoes)
            .HasForeignKey(uj => uj.JogoId)
            // Restrict impede excluir um jogo que alguém já comprou: o histórico
            // da biblioteca precisa continuar íntegro.
            .OnDelete(DeleteBehavior.Restrict);

        // Reforça no banco a regra que a entidade já garante: o mesmo jogo não
        // entra duas vezes na biblioteca do mesmo usuário.
        builder.HasIndex(uj => new { uj.UsuarioId, uj.JogoId }).IsUnique();
    }
}
