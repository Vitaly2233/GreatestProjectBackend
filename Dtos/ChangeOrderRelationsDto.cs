namespace SimpleApi.Dto;

public class ChangeOrderRelationsDto
{
    public int? UserId { get; set; }
    public ICollection<int>? ProductIds { get; set; }
}
