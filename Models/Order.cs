using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    public ICollection<Product>? Products { get; set; }

    [Required]
    public User? User { get; set; }
}
