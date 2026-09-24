using CoreBase.Database;
using CoreBase.Entities;
using CoreBase.Helpers;
using Microsoft.EntityFrameworkCore;

namespace RestApi.Config;

public static class UserSeed
{
  public static async Task RunAsync(UserDbContext db)
  {
    var alreadySeeded = await db.Users.CountAsync() == 50
      && await db.Employers.CountAsync() == 200
      && await db.Customers.CountAsync() == 200
      && await db.Orders.CountAsync() == 400;
    if (alreadySeeded) return;

    await db.Database.ExecuteSqlRawAsync("""
      TRUNCATE TABLE "OrderItems", "Orders", "Products", "Customers", "Users", "Employers"
      RESTART IDENTITY CASCADE;
      """);

    var rng = new Random(42);
    var password = PasswordHelper.Hash("Pass123!");

    var employers = BuildEmployers();
    db.Employers.AddRange(employers);
    await db.SaveChangesAsync();

    db.Users.AddRange(BuildUsers(password, rng));
    db.Customers.AddRange(BuildCustomers(rng));
    db.Products.AddRange(BuildProducts(rng));
    await db.SaveChangesAsync();

    var (orders, items) = BuildOrders(rng);
    db.Orders.AddRange(orders);
    await db.SaveChangesAsync();
    db.OrderItems.AddRange(items);
    await db.SaveChangesAsync();
  }

  private static List<Employer> BuildEmployers()
  {
    var industries = new[] { "อาหาร", "เทคโนโลยี", "ค้าปลีก", "โลจิสติกส์", "การเงิน", "ก่อสร้าง", "ท่องเที่ยว", "สุขภาพ" };
    var prefixes = new[] { "แสงทอง", "นครินทร์", "เอเชีย", "สยาม", "พีค", "กรีน", "เมโทร", "โอเรียนท์", "ไทยรุ่ง", "แปซิฟิก" };
    var suffixes = new[] { "กรุ๊ป", "โฮลดิ้ง", "ฟู้ดส์", "เทค", "ซัพพลาย", "โลจิสติกส์", "แคปิตอล", "ดีเวลลอปเมนต์" };
    var places = Places();
    var list = new List<Employer>(200);

    for (var i = 1; i <= 200; i++)
    {
      var place = places[(i - 1) % places.Length];
      list.Add(new Employer
      {
        Name = $"บริษัท {prefixes[(i - 1) % prefixes.Length]} {suffixes[(i / 10) % suffixes.Length]} {i:000} จำกัด",
        Industry = industries[(i - 1) % industries.Length],
        Province = place.Province,
        District = place.District,
        FoundedYear = 1985 + (i % 40),
        Headcount = 8 + (i * 17) % 1200,
        AnnualRevenue = 2_000_000m + (i * 137_000m) % 180_000_000m,
        Status = i % 11 == 0 ? "inactive" : "active"
      });
    }

    return list;
  }

  private static List<User> BuildUsers(string password, Random rng)
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
        Name = $"{first[(i - 1) % first.Length]} {last[(i * 3) % last.Length]}",
        Email = $"user{i:00}@demo.com",
        Password = password,
        Role = roles[(i - 1) % roles.Length],
        DateOfBirth = Utc(1968 + (i % 35), 1 + i % 12, 1 + i % 28),
        Address = $"{place.District} {place.Province}",
        Income = 18000 + (i * 2300) % 140000,
        Occupation = jobs[(i - 1) % jobs.Length],
        EmployerId = i <= 5 ? null : 1 + (i * 7) % 200
      });
    }

    return list;
  }

  private static List<Customer> BuildCustomers(Random rng)
  {
    var first = FirstNames();
    var last = LastNames();
    var segments = new[] { "retail", "retail", "corporate", "wholesale" };
    var places = Places();
    var list = new List<Customer>(200);

    for (var i = 1; i <= 200; i++)
    {
      var place = places[rng.Next(places.Length)];
      var corporate = i % 5 == 0;
      list.Add(new Customer
      {
        Name = $"{first[(i * 2) % first.Length]} {last[(i * 5) % last.Length]}",
        Email = $"customer{i:000}@mail.com",
        Phone = $"08{i:00000000}"[..10],
        Province = place.Province,
        District = place.District,
        EmployerId = corporate ? 1 + (i * 11) % 200 : null,
        Segment = corporate ? "corporate" : segments[i % segments.Length],
        CreditLimit = corporate ? 80000 + (i * 1500) % 400000 : 5000 + (i * 400) % 45000,
        RegisteredAt = Utc(2022 + i % 4, 1 + i % 12, 1 + i % 28),
        IsActive = i % 13 != 0
      });
    }

    return list;
  }

  private static List<Product> BuildProducts(Random rng)
  {
    var catalog = new (string Name, string Category, decimal Price)[]
    {
      ("พิซซ่ามาร์เกริต้า", "อาหาร", 259),
      ("พิซซ่าฮาวายเอี้ยน", "อาหาร", 289),
      ("พิซซ่าซีฟู้ด", "อาหาร", 359),
      ("พิซซ่าเปปเปอโรนี", "อาหาร", 299),
      ("พาสต้าคาโบนาร่า", "อาหาร", 189),
      ("สลัดซีซาร์", "อาหาร", 149),
      ("ไก่ทอดกรอบ", "อาหาร", 129),
      ("ซุปครีมเห็ด", "อาหาร", 99),
      ("โค้กกระป๋อง", "เครื่องดื่ม", 25),
      ("น้ำส้มคั้น", "เครื่องดื่ม", 45),
      ("กาแฟลาเต้", "เครื่องดื่ม", 75),
      ("ชาเขียวเย็น", "เครื่องดื่ม", 55),
      ("ชีสเค้ก", "ของหวาน", 89),
      ("บราวนี่", "ของหวาน", 69),
      ("ไอศกรีมวนิลา", "ของหวาน", 49),
      ("กล่องพิซซ่า", "บรรจุภัณฑ์", 12),
      ("ถ้วยกระดาษ", "บรรจุภัณฑ์", 8),
      ("ถุงกระดาษ", "บรรจุภัณฑ์", 6),
      ("เตาอบพิซซ่าเล็ก", "อุปกรณ์", 8900),
      ("เครื่องบดกาแฟ", "อุปกรณ์", 4500),
      ("ชุดช้อนส้อม", "อุปกรณ์", 120),
      ("เสื้อยูนิฟอร์ม", "เครื่องแต่งกาย", 350),
      ("หมวกพนักงาน", "เครื่องแต่งกาย", 90),
      ("ผ้ากันเปื้อน", "เครื่องแต่งกาย", 150),
      ("ซอสมะเขือเทศ", "วัตถุดิบ", 45),
      ("แป้งพิซซ่า", "วัตถุดิบ", 85),
      ("มอซซาเรลล่า 1kg", "วัตถุดิบ", 280),
      ("เปปเปอโรนี 500g", "วัตถุดิบ", 190),
      ("เห็ดแชมปิญอง", "วัตถุดิบ", 65),
      ("น้ำมันมะกอก", "วัตถุดิบ", 220),
      ("โต๊ะไม้ 4 ที่นั่ง", "เฟอร์นิเจอร์", 3200),
      ("เก้าอี้ร้านอาหาร", "เฟอร์นิเจอร์", 890),
      ("ป้ายเมนูตั้งโต๊ะ", "เฟอร์นิเจอร์", 180),
      ("คูปองส่วนลด 50", "โปรโมชัน", 50),
      ("คูปองส่วนลด 100", "โปรโมชัน", 100),
      ("บัตรสมาชิกทอง", "โปรโมชัน", 499),
      ("ชุดปิกนิกพิซซ่า", "เซ็ต", 599),
      ("ชุดครอบครัว", "เซ็ต", 899),
      ("ชุดเครื่องดื่ม 4 แก้ว", "เซ็ต", 199),
      ("กล่องของขวัญ", "เซ็ต", 399)
    };

    return catalog.Select((p, i) => new Product
    {
      Name = p.Name,
      Category = p.Category,
      Price = p.Price,
      Stock = 10 + rng.Next(5, 400),
      EmployerId = 1 + (i * 13) % 200
    }).ToList();
  }

  private static (List<Order> Orders, List<OrderItem> Items) BuildOrders(Random rng)
  {
    var statuses = new[] { "pending", "paid", "paid", "shipped", "cancelled" };
    var channels = new[] { "web", "store", "phone", "app" };
    var products = Enumerable.Range(1, 40).ToArray();
    var prices = new decimal[]
    {
      259, 289, 359, 299, 189, 149, 129, 99, 25, 45,
      75, 55, 89, 69, 49, 12, 8, 6, 8900, 4500,
      120, 350, 90, 150, 45, 85, 280, 190, 65, 220,
      3200, 890, 180, 50, 100, 499, 599, 899, 199, 399
    };

    var orders = new List<Order>(400);
    var items = new List<OrderItem>(1000);

    for (var i = 1; i <= 400; i++)
    {
      orders.Add(new Order
      {
        CustomerId = 1 + (i * 9) % 200,
        UserId = 1 + (i * 3) % 50,
        OrderedAt = Utc(2024 + i % 2, 1 + i % 12, 1 + i % 28).AddHours(i % 23),
        Status = statuses[i % statuses.Length],
        Channel = channels[i % channels.Length]
      });

      var count = 1 + i % 4;
      for (var n = 0; n < count; n++)
      {
        var productId = products[(i + n * 7) % products.Length];
        items.Add(new OrderItem
        {
          OrderId = i,
          ProductId = productId,
          Quantity = 1 + (i + n) % 5,
          UnitPrice = prices[productId - 1]
        });
      }
    }

    return (orders, items);
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
