using Desafio.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Retorna todos os produtos disponíveis.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        return Ok(new List<Products>
        {
            new Products(1, "Produto 1", 10.99m, "Descrição do Produto 1", "Categoria A"),
            new Products(2, "Produto 2", 15.49m, "Descrição do Produto 2", "Categoria B"),
            new Products(3, "Produto 3", 7.99m, "Descrição do Produto 3", "Categoria A"),
            new Products(4, "Produto 4", 12.75m, "Descrição do Produto 4", "Categoria C"),
            new Products(5, "Produto 5", 9.50m, "Descrição do Produto 5", "Categoria B")
        });
    }

    /// <summary>
    /// Retorna um produto específico pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductsById(int id)
    {
        return Ok(new Products(id, $"Produto {id}", 10.99m + id, $"Descrição do Produto {id}", $"Categoria {(char)('A' + (id % 3))}"));
    }


    /// <summary>
    /// Cria um novo produto.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateProduct()
    {
        // Lógica para criar um novo produto (exemplo)
        return CreatedAtAction(nameof(GetProductsById), new { id = 6 }, new Products(6, "Produto 6", 11.99m, "Descrição do Produto 6", "Categoria A"));
    }

    /// <summary>
    /// Atualiza um produto existente pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id)
    {
        // Lógica para atualizar um produto existente (exemplo)
        return Ok("Atualizado com Sucesso");
    }

    /// <summary>
    /// Deleta um produto existente pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        // Lógica para deletar um produto existente (exemplo)
        return Ok("Deletado com Sucesso");
    }
}
