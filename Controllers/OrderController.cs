using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Dto;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(AppDbContext appDbContext) : ControllerBase
{
    private readonly AppDbContext db = appDbContext;

    [HttpPost]
    [ProducesResponseType<OrderPopulatedDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<OrderPopulatedDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var user = await db.Users.FindAsync(dto.UserId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        List<Product> products;
        if (dto.ProductIds != null && dto.ProductIds.Any())
        {
            products = await db.Products.Where(p => dto.ProductIds.Contains(p.Id)).ToListAsync();
            if (products.Count != dto.ProductIds.Count)
            {
                return NotFound("Some of the products are not found");
            }
        }
        else
        {
            products = new List<Product>();
        }

        var order = new Order
        {
            Name = dto.Name,
            User = user,
            Products = products
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var result = await GetOrder(order.Id);

        if (result.Result is NotFoundResult)
        {
            return NotFound();
        }

        var okResult = result.Result as OkObjectResult;
        if (okResult?.Value is OrderPopulatedDto orderPopulatedDto)
        {
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderPopulatedDto);
        }

        return StatusCode(500, "An error occurred while retrieving the created order.");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderPopulatedDto>> GetOrder(int id)
    {
        var order = await db
            .Orders.Include(o => o.User)
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        OrderPopulatedDto response =
            new()
            {
                Name = order.Name,
                Id = order.Id,
                Products = order
                    ?.Products?.Select(p => new ProductDto { Id = p.Id, Name = p.Name })
                    .ToList(),
            };

        if (order?.User != null)
        {
            response.User = new UserDto()
            {
                Id = order.User.Id,
                Email = order.User.Email,
                Username = order.User.Username,
            };
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<OrderPopulatedDto>> GetAllOrders()
    {
        return Ok();
    }
};
