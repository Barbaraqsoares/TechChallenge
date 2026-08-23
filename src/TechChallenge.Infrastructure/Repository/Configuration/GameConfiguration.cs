using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository.Configuration
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");
  
            builder.Property(p => p.Id).HasColumnType("int").UseIdentityColumn(); 
            builder.Property(p => p.Name).IsRequired().HasColumnType("nvarchar(100)");
            builder.Property(p => p.Description).HasColumnType("nvarchar(max)");
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.IsActive).HasColumnType("bit");
            builder.Property(p => p.IsMultiplayer).HasColumnType("bit");
            builder.Property(p => p.CreatedAt).HasColumnType("datetime2");
            builder.Property(p => p.UpdateAt).HasColumnType("datetime2");
         

        }

        
    }
}
