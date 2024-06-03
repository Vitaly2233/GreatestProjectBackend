using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleApi.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    public required ICollection<Product> Products { get; set; }
}
