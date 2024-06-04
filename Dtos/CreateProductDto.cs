using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Dto;

public class CreateProductDto
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
}
