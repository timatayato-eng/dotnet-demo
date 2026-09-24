namespace CoreBase.Entities;

public class Order
{
  public int Id { get; set; }
  public int CustomerId { get; set; }
  public int UserId { get; set; }
  public DateTime OrderedAt { get; set; }
  public string Status { get; set; } = "pending";
  public string Channel { get; set; } = "web";
}
