using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Dto;

public class UpdateOrderDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
}
