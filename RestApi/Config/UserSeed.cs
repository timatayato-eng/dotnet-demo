using CoreBase.Database;
using CoreBase.Entities;
using CoreBase.Helpers;
using Microsoft.EntityFrameworkCore;

namespace RestApi.Config;

public static class UserSeed
{
  public static async Task RunAsync(UserDbContext db)
  {
    if (await db.Users.CountAsync() == 50)
    {
      var sample = await db.Users.FirstOrDefaultAsync(u => u.Email == "user01@demo.com");
      if (sample?.Name == "Huang"
          && sample.Password is not null
          && PasswordHelper.Verify("123", sample.Password))
        return;
    }

    await db.Database.ExecuteSqlRawAsync("""
      TRUNCATE TABLE "Users" RESTART IDENTITY CASCADE;
      """);

    var rng = new Random(42);
    var password = PasswordHelper.Hash("Pass123!");
    var adminPassword = PasswordHelper.Hash("123");
    db.Users.AddRange(BuildUsers(password, adminPassword, rng));
    await db.SaveChangesAsync();
  }

  private static List<User> BuildUsers(string password, string adminPassword, Random rng)
  {
    var first = FirstNames();
    var last = LastNames();
    var roles = new[] { "admin", "manager", "user", "user", "user" };
    var jobs = new[] { "วิศวกร", "นักบัญชี", "พนักงานขาย", "ครู", "พยาบาล", "นักออกแบบ", "โปรแกรมเมอร์", "HR" };
    var places = Places();
    var list = new List<User>(50);

    for (var i = 1; i <= 50; i++)
    {
      var place = places[rng.Next(places.Length)];
      list.Add(new User
      {
        Name = i == 1 ? "Huang" : $"{first[(i - 1) % first.Length]} {last[(i * 3) % last.Length]}",
        Email = $"user{i:00}@demo.com",
        Password = i == 1 ? adminPassword : password,
        Role = i == 1 ? "super admin" : roles[(i - 1) % roles.Length],
        DateOfBirth = Utc(1968 + (i % 35), 1 + i % 12, 1 + i % 28),
        Address = $"{place.District} {place.Province}",
        Income = 18000 + (i * 2300) % 140000,
        Occupation = jobs[(i - 1) % jobs.Length]
      });
    }

    return list;
  }

  private static (string District, string Province)[] Places() =>
  [
    ("สุขุมวิท", "กรุงเทพฯ"),
    ("ลาดพร้าว", "กรุงเทพฯ"),
    ("จตุจักร", "กรุงเทพฯ"),
    ("พระโขนง", "กรุงเทพฯ"),
    ("นิมมาน", "เชียงใหม่"),
    ("สันติธรรม", "เชียงใหม่"),
    ("หางดง", "เชียงใหม่"),
    ("เมือง", "ขอนแก่น"),
    ("ป่าตอง", "ภูเก็ต"),
    ("เมือง", "ภูเก็ต"),
    ("หาดใหญ่", "สงขลา"),
    ("เมือง", "นครราชสีมา"),
    ("บางแสน", "ชลบุรี"),
    ("ศรีราชา", "ชลบุรี"),
    ("เมือง", "เชียงราย")
  ];

  private static string[] FirstNames() =>
  [
    "สมชาย", "สมหญิง", "นัฐพงศ์", "มาลี", "อนันต์", "ปราณี", "วิชัย", "กัญญา",
    "พิชัย", "อารยา", "ธนพล", "สุนิสา", "วรพล", "กมล", "ชัยวัฒน์", "พิมพ์ใจ",
    "ภาคิน", "นฤมล", "ธีรภัทร", "ศิริพร"
  ];

  private static string[] LastNames() =>
  [
    "ใจดี", "ทองชัย", "ศรีสุข", "จันทร์", "วงศ์", "ดี", "บุญ", "ฤทธิ์",
    "ลิ้ม", "สมบัติ", "พงศ์เพชร", "รัตนะ", "ชัยมงคล", "ศรีทอง", "อินทร์"
  ];

  private static DateTime Utc(int year, int month, int day) =>
    new(year, month, day, 0, 0, 0, DateTimeKind.Utc);
}
