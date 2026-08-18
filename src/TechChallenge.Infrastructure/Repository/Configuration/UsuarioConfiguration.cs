using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entity;
using TechChallenge.Domain.ValueObject;

namespace TechChallenge.Infrastructure.Repository.Configuration;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnType("INT").UseIdentityColumn();
        builder.Property(u => u.DataCriacao).HasColumnType("DATETIME").IsRequired();
        builder.Property(u => u.Nome).HasColumnType("VARCHAR(100)").IsRequired();

        // Objetos de Valor viram uma única coluna: gravamos o texto e, na leitura,
        // o próprio Value Object é reconstruído.
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Endereco,
                endereco => Email.Criar(endereco))
            .HasColumnType("VARCHAR(200)")
            .IsRequired();

        builder.Property(u => u.Senha)
            .HasConversion(
                senha => senha.Hash,
                hash => Senha.APartirDoHash(hash))
            .HasColumnName("SenhaHash")
            .HasColumnType("VARCHAR(200)")
            .IsRequired();

        // O e-mail é a credencial de login: não pode repetir.
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Perfil).HasColumnType("INT").IsRequired();

        // A biblioteca é exposta como somente leitura, então o EF precisa gravar
        // e ler pelo campo privado _biblioteca em vez de pela propriedade.
        builder.Metadata
            .FindNavigation(nameof(Usuario.Biblioteca))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
