using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Dto;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(AppDbContext appDbContext) : ControllerBase
{
    private readonly AppDbContext db = appDbContext;

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] CreateProductDto product)
    {
        Product newProduct = new() { Name = product.Name };
        db.Products.Add(newProduct);
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await db.Products.ToListAsync();
    }
}
