using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext db;

    public OrderController(AppDbContext appDbContext)
    {
        db = appDbContext;
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] Order order)
    {
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return Ok();
    }
};
