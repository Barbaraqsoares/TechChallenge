using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnType("int").ValueGeneratedNever().UseIdentityColumn();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(t => t.Email).IsRequired().HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(t => t.Login).HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("varchar(50)")
                .HasColumnName("Password");
            builder.Property(t => t.Perfil).IsRequired();
            builder.Property(t => t.CreatedAt).HasColumnType("DATETIME");
            builder.Property(t => t.UpdatedAt).HasColumnType("DATETIME");
        }
    }
}
