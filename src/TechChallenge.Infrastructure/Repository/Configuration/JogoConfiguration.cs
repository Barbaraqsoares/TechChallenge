using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository.Configuration;

public class JogoConfiguration : IEntityTypeConfiguration<Jogo>
{
    public void Configure(EntityTypeBuilder<Jogo> builder)
    {
        builder.ToTable("Jogo");
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Id).HasColumnType("INT").UseIdentityColumn();
        builder.Property(j => j.DataCriacao).HasColumnType("DATETIME").IsRequired();
        builder.Property(j => j.Titulo).HasColumnType("VARCHAR(200)").IsRequired();
        builder.Property(j => j.Descricao).HasColumnType("VARCHAR(2000)");
        builder.Property(j => j.Genero).HasColumnType("VARCHAR(100)");
        builder.Property(j => j.DataLancamento).HasColumnType("DATETIME").IsRequired();

        // DECIMAL(10,2) evita o arredondamento indevido que o tipo padrão causaria
        // em valores monetários.
        builder.Property(j => j.Preco).HasColumnType("DECIMAL(10,2)").IsRequired();
        builder.Property(j => j.PercentualDesconto).HasColumnType("DECIMAL(5,2)").IsRequired();

        builder.HasIndex(j => j.Titulo);

        builder.Metadata
            .FindNavigation(nameof(Jogo.Aquisicoes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
