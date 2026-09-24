-- Users 50 | Employers 200 | Customers 200 | Products 40 | Orders 400 | OrderItems
-- รหัสผ่าน user ทุกคน: Pass123!

-- ข้อ 1: พนักงาน role = user ที่รายได้ต่ำกว่า 40000
SELECT "Name", "Income", "Role", "Occupation"
FROM "Users"
WHERE "Role" = 'user' AND "Income" < 40000
ORDER BY "Income";

-- ข้อ 2: รายได้เฉลี่ยต่ออาชีพ
SELECT "Occupation", ROUND(AVG("Income"), 0) AS avg_income, COUNT(*) AS total
FROM "Users"
GROUP BY "Occupation"
ORDER BY avg_income DESC;

-- ข้อ 3: คนที่อยู่ในเชียงใหม่
SELECT "Name", "Address"
FROM "Users"
WHERE "Address" LIKE '%เชียงใหม่%';

-- ข้อ 4: เกิดก่อนปี 1990 และเป็นวิศวกรหรือนักบัญชี
SELECT "Name", "Occupation", "DateOfBirth"
FROM "Users"
WHERE "DateOfBirth" < '1990-01-01'
  AND "Occupation" IN ('วิศวกร', 'นักบัญชี')
ORDER BY "DateOfBirth";

-- ข้อ 5: จังหวัดไหนมี user เยอะสุด
SELECT
  TRIM(SPLIT_PART("Address", ' ', 2)) AS province,
  COUNT(*) AS total
FROM "Users"
GROUP BY 1
ORDER BY total DESC;

-- ข้อ 6: user กับบริษัทที่สังกัด (LEFT JOIN — admin บางคนไม่มีนายจ้าง)
SELECT u."Name" AS user_name, u."Occupation", e."Name" AS employer, e."Industry"
FROM "Users" u
LEFT JOIN "Employers" e ON e."Id" = u."EmployerId"
ORDER BY u."Id";

-- ข้อ 7: บริษัทอาหารที่ยัง active และรายได้ปีละเกิน 20 ล้าน
SELECT "Name", "Province", "AnnualRevenue", "Headcount"
FROM "Employers"
WHERE "Industry" = 'อาหาร'
  AND "Status" = 'active'
  AND "AnnualRevenue" > 20000000
ORDER BY "AnnualRevenue" DESC;

-- ข้อ 8: จำนวนบริษัทต่ออุตสาหกรรม
SELECT "Industry", COUNT(*) AS total, ROUND(AVG("Headcount"), 0) AS avg_headcount
FROM "Employers"
GROUP BY "Industry"
ORDER BY total DESC;

-- ข้อ 9: ลูกค้า corporate ที่ผูกกับบริษัท + ชื่อบริษัท
SELECT c."Name" AS customer, c."CreditLimit", e."Name" AS employer, e."Industry"
FROM "Customers" c
JOIN "Employers" e ON e."Id" = c."EmployerId"
WHERE c."Segment" = 'corporate' AND c."IsActive" = true
ORDER BY c."CreditLimit" DESC;

-- ข้อ 10: จังหวัดไหนมีลูกค้าเยอะสุด แยก active/inactive
SELECT "Province",
       COUNT(*) FILTER (WHERE "IsActive") AS active,
       COUNT(*) FILTER (WHERE NOT "IsActive") AS inactive
FROM "Customers"
GROUP BY "Province"
ORDER BY active DESC;

-- ข้อ 11: ออเดอร์ที่จ่ายแล้ว + ชื่อลูกค้า + พนักงานขาย
SELECT o."Id", c."Name" AS customer, u."Name" AS salesperson, o."Channel", o."OrderedAt"
FROM "Orders" o
JOIN "Customers" c ON c."Id" = o."CustomerId"
JOIN "Users" u ON u."Id" = o."UserId"
WHERE o."Status" = 'paid'
ORDER BY o."OrderedAt" DESC
LIMIT 20;

-- ข้อ 12: ยอดขายรวมต่อออเดอร์ (เฉพาะที่ไม่ถูกยกเลิก)
SELECT o."Id",
       c."Name" AS customer,
       SUM(i."Quantity" * i."UnitPrice") AS order_total
FROM "Orders" o
JOIN "Customers" c ON c."Id" = o."CustomerId"
JOIN "OrderItems" i ON i."OrderId" = o."Id"
WHERE o."Status" <> 'cancelled'
GROUP BY o."Id", c."Name"
ORDER BY order_total DESC
LIMIT 15;

-- ข้อ 13: สินค้าขายดี 10 อันดับ (จำนวนชิ้น)
SELECT p."Name", p."Category", SUM(i."Quantity") AS qty_sold,
       SUM(i."Quantity" * i."UnitPrice") AS revenue
FROM "OrderItems" i
JOIN "Products" p ON p."Id" = i."ProductId"
JOIN "Orders" o ON o."Id" = i."OrderId"
WHERE o."Status" IN ('paid', 'shipped')
GROUP BY p."Id", p."Name", p."Category"
ORDER BY qty_sold DESC
LIMIT 10;

-- ข้อ 14: ยอดขายรายเดือน ปี 2024-2025
SELECT DATE_TRUNC('month', o."OrderedAt") AS month,
       COUNT(DISTINCT o."Id") AS orders,
       SUM(i."Quantity" * i."UnitPrice") AS revenue
FROM "Orders" o
JOIN "OrderItems" i ON i."OrderId" = o."Id"
WHERE o."Status" <> 'cancelled'
GROUP BY 1
ORDER BY 1;

-- ข้อ 15: ลูกค้าที่ไม่เคยสั่งของ (NOT EXISTS)
SELECT c."Name", c."Email", c."Province"
FROM "Customers" c
WHERE NOT EXISTS (
  SELECT 1 FROM "Orders" o WHERE o."CustomerId" = c."Id"
);

-- ข้อ 16: พนักงานขายที่ยอดขายสูงกว่าค่าเฉลี่ย
WITH sales AS (
  SELECT u."Id", u."Name",
         SUM(i."Quantity" * i."UnitPrice") AS revenue
  FROM "Users" u
  JOIN "Orders" o ON o."UserId" = u."Id" AND o."Status" <> 'cancelled'
  JOIN "OrderItems" i ON i."OrderId" = o."Id"
  GROUP BY u."Id", u."Name"
)
SELECT *
FROM sales
WHERE revenue > (SELECT AVG(revenue) FROM sales)
ORDER BY revenue DESC;

-- ข้อ 17: สินค้าแพงกว่า 100 บาท พร้อมซัพพลายเออร์ (JOIN ที่เคยเขียนผิด)
SELECT e."Name" AS employer, p."Name" AS product, p."Price"
FROM "Employers" e
JOIN "Products" p ON e."Id" = p."EmployerId"
WHERE p."Price" > 100
ORDER BY p."Price" DESC;
