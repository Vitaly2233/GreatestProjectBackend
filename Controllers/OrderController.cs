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
        var orders = await db.Orders.Include(o => o.User).Include(o => o.Products).ToListAsync();

        var orderPopulatedDtos = orders.Select(o =>
        {
            var result = new OrderPopulatedDto() { Name = o.Name, Id = o.Id };

            if (o.Products != null)
            {
                result.Products = o
                    .Products.Select(o => new ProductDto() { Name = o.Name })
                    .ToList();
            }
            if (o.User != null)
            {
                result.User = new UserDto()
                {
                    Id = o.User.Id,
                    Email = o.User.Email,
                    Username = o.User.Username,
                };
            }

            return result;
        });

        return Ok(orderPopulatedDtos);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateOrder(UpdateOrderDto order)
    {
        var foundOrder = await db.Orders.FindAsync(order.Id);

        if (foundOrder == null)
            return NotFound();

        foundOrder.Name = order.Name;

        db.Orders.Update(foundOrder);
        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/change-relations")]
    public async Task<ActionResult> ChangeOrderRelations(int id, ChangeOrderRelationsDto dto)
    {
        // Start all tasks concurrently
        var orderTask = db.Orders.FindAsync(id).AsTask();
        var userTask =
            dto.UserId != null
                ? db.Users.FindAsync(dto.UserId.Value).AsTask()
                : Task.FromResult<User?>(null);
        var productTask =
            dto.ProductIds != null
                ? db.Products.Where(p => dto.ProductIds.Contains(p.Id)).ToListAsync()
                : Task.FromResult(new List<Product>());

        // Wait for all tasks to complete
        await Task.WhenAll(orderTask, userTask, productTask);

        var foundOrder = orderTask.Result;
        var foundUser = userTask.Result;
        var products = productTask.Result;

        if (foundOrder == null)
        {
            return NotFound();
        }

        if (dto.UserId.HasValue && foundUser == null)
        {
            return NotFound("User not found");
        }
        foundOrder.User = foundUser;

        if (dto.ProductIds != null)
        {
            foundOrder.Products = products;
        }

        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        var foundOrder = await db.Orders.FindAsync(id);

        if (foundOrder == null)
            return NotFound();

        db.Orders.Remove(foundOrder);
        await db.SaveChangesAsync();

        return Ok();
    }
};
