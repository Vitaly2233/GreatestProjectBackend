namespace SimpleApi.Dto;

public class OrderPopulatedDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<ProductDto>? Products { get; set; }
    public UserDto? User { get; set; }
}
