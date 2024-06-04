using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(AppDbContext appDbContext) : ControllerBase
{
    private readonly AppDbContext db = appDbContext;

    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] User user)
    {
        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            return Conflict("A user with this email already exists.");
        }
        return Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetUsers()
    {
        return await db.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        User? user = await db.Users.FindAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(User user)
    {
        var userFound = await db.Users.FindAsync(user.Id);
        if (userFound == null)
        {
            db.Users.Add(user);
        }
        else
        {
            db.Users.Update(user);
        }
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        User? foundUser = await db.Users.FindAsync(id);
        if (foundUser == null)
        {
            return NotFound();
        }
        db.Users.Remove(foundUser);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
