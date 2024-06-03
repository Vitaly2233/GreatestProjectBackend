using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    public virtual ICollection<Order>? Orders { get; set; }
}
