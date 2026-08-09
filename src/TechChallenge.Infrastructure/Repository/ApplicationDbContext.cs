using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
}
