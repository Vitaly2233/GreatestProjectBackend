using System.Text.Json.Serialization;

namespace SimpleApi.Models;

public class CreateProduct : Product
{
    [JsonIgnore]
    public override ICollection<Order>? Orders { get; set; }
}
