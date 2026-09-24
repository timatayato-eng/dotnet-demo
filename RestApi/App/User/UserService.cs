using CoreBase.Cores.Attribute;
using CoreBase.Database;
using CoreBase.Helpers;
using Microsoft.EntityFrameworkCore;
using RestApi.App.User.Req;
using RestApi.Models;

namespace RestApi.App.User;

[Injectable(ServiceLifetime.Scoped)]
public class UserService(UserDbContext dbContext)
{
  private readonly UserDbContext _db = dbContext;

  public async Task<List<UserModel>> FindAll()
  {
    var users = await _db.Users.ToListAsync();
    return users.Select(ToModel).ToList();
  }

  public async Task<UserModel?> FindById(int id)
  {
    var user = await _db.Users.FindAsync(id);
    return user is null ? null : ToModel(user);
  }

  public async Task<UserModel> Save(UserSaveReq form)
  {
    if (string.IsNullOrWhiteSpace(form.Password))
      throw new ArgumentException("Password is required.");

    var user = new CoreBase.Entities.User
    {
      Name = form.Name,
      Email = form.Email,
      Password = PasswordHelper.Hash(form.Password),
      Role = form.Role,
      DateOfBirth = form.DateOfBirth,
      Address = form.Address,
      Income = form.Income,
      Occupation = form.Occupation
    };

    await _db.Users.AddAsync(user);
    await _db.SaveChangesAsync();
    return ToModel(user);
  }

  public async Task<UserModel?> Update(int id, UserSaveReq form)
  {
    var user = await _db.Users.FindAsync(id);
    if (user is null) return null;

    user.Name = form.Name;
    user.Email = form.Email;
    user.Role = form.Role;
    user.DateOfBirth = form.DateOfBirth;
    user.Address = form.Address;
    user.Income = form.Income;
    user.Occupation = form.Occupation;
    if (!string.IsNullOrWhiteSpace(form.Password))
      user.Password = PasswordHelper.Hash(form.Password);

    await _db.SaveChangesAsync();
    return ToModel(user);
  }

  public async Task<bool> Remove(int id)
  {
    var user = await _db.Users.FindAsync(id);
    if (user is null) return false;

    _db.Users.Remove(user);
    await _db.SaveChangesAsync();
    return true;
  }

  private static UserModel ToModel(CoreBase.Entities.User user) => new()
  {
    Id = user.Id,
    Name = user.Name,
    Email = user.Email,
    Role = user.Role,
    DateOfBirth = user.DateOfBirth,
    Address = user.Address,
    Income = user.Income,
    Occupation = user.Occupation
  };
}
