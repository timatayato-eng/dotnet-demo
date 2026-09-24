namespace CoreBase.Entities;

public class Employer
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
  public string Industry { get; set; } = "";
  public string Province { get; set; } = "";
  public string District { get; set; } = "";
  public int FoundedYear { get; set; }
  public int Headcount { get; set; }
  public decimal AnnualRevenue { get; set; }
  public string Status { get; set; } = "active";
}
