# DotnetDemo

REST API ด้วย ASP.NET Core (.NET 10) และ PostgreSQL ใช้ Entity Framework Core สำหรับฐานข้อมูล มีข้อมูลผู้ใช้ตัวอย่าง 50 คน

## โครงสร้าง

| โปรเจกต์ | หน้าที่ |
| --- | --- |
| `RestApi` | Web API, DI, Swagger, seed ข้อมูล |
| `CoreBase` | Entity, `UserDbContext`, migration, helper |

```
DotnetDemo/
├── RestApi/
│   ├── App/User/          # controller + service
│   ├── Config/            # connection และ seed
│   └── Models/
├── CoreBase/
│   ├── Entities/
│   └── Database/
└── docker-compose.yml
```

คลาสที่มี `[Injectable]` จะถูกลงทะเบียนอัตโนมัติตอนสตาร์ท

## สิ่งที่ต้องมี

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) สำหรับ PostgreSQL

## วิธีรัน

เปิด PostgreSQL:

```bash
docker compose up -d
```

ค่าเริ่มต้นของฐานข้อมูลอยู่ใน `RestApi/appsettings.Development.json`:

```
Host=localhost;Port=5432;Database=dotnetdemo;Username=demo;Password=demo
```

รัน API:

```bash
dotnet run --project RestApi
```

ตอนสตาร์ทแอปจะรัน migration แล้วใส่ข้อมูลตัวอย่างถ้ายังไม่ครบชุด

| บริการ | URL |
| --- | --- |
| API | http://localhost:5062 |
| Swagger | http://localhost:5062/swagger |

## API ผู้ใช้

| Method | Path | คำอธิบาย |
| --- | --- | --- |
| GET | `/users` | รายชื่อผู้ใช้ทั้งหมด |
| GET | `/user/{id}` | ผู้ใช้ตาม id |
| POST | `/user` | สร้างผู้ใช้ (ต้องมีรหัสผ่าน) |
| PUT | `/user/{id}` | แก้ไขผู้ใช้ |
| DELETE | `/user/{id}` | ลบผู้ใช้ |

ตัวอย่างสร้างผู้ใช้:

```bash
curl -X POST http://localhost:5062/user \
  -H "Content-Type: application/json" \
  -d '{"name":"สมชาย ใจดี","email":"somchai@demo.com","password":"Pass123!","role":"user"}'
```

รหัสผ่านถูกเก็บเป็น hash และไม่ถูกส่งกลับใน response

## ข้อมูลตัวอย่าง

ตาราง `Users` มี 50 แถว อีเมล `user01@demo.com` ถึง `user50@demo.com`

คนที่ 1 คือ Huang บทบาท `super admin` รหัสผ่าน `123` คนที่เหลือรหัสผ่าน `Pass123!`

ถ้าจำนวนแถวไม่ใช่ 50 seed จะล้างตารางแล้วใส่ข้อมูลใหม่
