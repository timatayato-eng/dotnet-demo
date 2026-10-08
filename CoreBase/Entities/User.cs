namespace CoreBase.Entities;

public class User
{
  public int Id { get; set; }
  public string? Name { get; set; }
  public string? Email { get; set; }
  public string? Password { get; set; }
  public string? Role { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string? Address { get; set; }
  public decimal? Income { get; set; }
  public string? Occupation { get; set; }
}
