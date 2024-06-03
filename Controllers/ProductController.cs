using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly AppDbContext db;

    public ProductController(AppDbContext appDbContext)
    {
        db = appDbContext;
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] CreateProduct product)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await db.Products.ToListAsync();
    }
}
