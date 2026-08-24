using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Entity;

namespace TechChallenge.Infrastructure.Repository.Configurations;

public class UserGameConfiguration : IEntityTypeConfiguration<UserGame>
{
    public void Configure(EntityTypeBuilder<UserGame> builder)
    {
        builder.HasKey(ug => ug.Id);
        builder.HasOne(ug => ug.User).WithMany().HasForeignKey(ug => ug.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ug => ug.Game).WithMany().HasForeignKey(ug => ug.GameId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(ug => new { ug.UserId, ug.GameId }).IsUnique();
    }
}