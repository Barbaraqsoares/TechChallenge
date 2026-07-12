namespace Desafio.Domain.Repositories;

public record Products(
    int Id,
    string Name,
    decimal Price,
    string Description,
    string Category
);