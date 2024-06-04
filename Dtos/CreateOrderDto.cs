using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Dto;

public class CreateOrderDto
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    public required int UserId { get; set; }
    public ICollection<int>? ProductIds { get; set; }
}
