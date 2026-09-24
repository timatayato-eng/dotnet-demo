namespace CoreBase.Entities;

public class Customer
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
  public string Email { get; set; } = "";
  public string Phone { get; set; } = "";
  public string Province { get; set; } = "";
  public string District { get; set; } = "";
  public int? EmployerId { get; set; }
  public string Segment { get; set; } = "retail";
  public decimal CreditLimit { get; set; }
  public DateTime RegisteredAt { get; set; }
  public bool IsActive { get; set; } = true;
}
