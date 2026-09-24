-- เฉลยแบบทดสอบ queries/exam.sql
-- รันทีละข้อได้ ผลลัพธ์อ้างอิงจาก seed ชุดปัจจุบัน

-- ===== ระดับ 1 พื้นฐาน =====

-- ข้อ 1
SELECT "Name", "Email"
FROM "Users"
ORDER BY "Name";

-- ข้อ 2
SELECT COUNT(*) AS inactive_employers
FROM "Employers"
WHERE "Status" = 'inactive';

-- ข้อ 3
SELECT "Name", "Category", "Price"
FROM "Products"
WHERE "Price" BETWEEN 100 AND 500
ORDER BY "Price" DESC;

-- ข้อ 4
SELECT "Name", "Email", "District"
FROM "Customers"
WHERE "Province" = 'ภูเก็ต' AND "IsActive" = true
ORDER BY "Name";

-- ข้อ 5
SELECT "Id", "Status", "Channel", "OrderedAt"
FROM "Orders"
ORDER BY "OrderedAt" DESC
LIMIT 5;

-- ===== ระดับ 2 กรองและนิพจน์ =====

-- ข้อ 6
SELECT "Name", "Role", "Email"
FROM "Users"
WHERE "EmployerId" IS NULL
ORDER BY "Id";

-- ข้อ 7
SELECT "Name", "Industry", "FoundedYear", "Province"
FROM "Employers"
WHERE "FoundedYear" < 2000
  AND "Industry" IN ('เทคโนโลยี', 'การเงิน')
ORDER BY "FoundedYear", "Name";

-- ข้อ 8
SELECT
  CASE
    WHEN EXTRACT(YEAR FROM AGE("DateOfBirth")) < 30 THEN 'ต่ำกว่า 30'
    WHEN EXTRACT(YEAR FROM AGE("DateOfBirth")) <= 45 THEN '30-45'
    ELSE 'มากกว่า 45'
  END AS age_band,
  COUNT(*) AS total
FROM "Users"
GROUP BY 1
ORDER BY 1;

-- ข้อ 9
SELECT "Name", TO_CHAR("RegisteredAt", 'YYYY-MM') AS registered_month
FROM "Customers"
WHERE EXTRACT(YEAR FROM "RegisteredAt") = 2024
ORDER BY "RegisteredAt", "Name";

-- ข้อ 10
SELECT
  "Name",
  "Category",
  "Stock",
  CASE WHEN "Stock" < 25 THEN 'ต่ำ' ELSE 'เฝ้าระวัง' END AS stock_flag
FROM "Products"
WHERE "Stock" < 50
ORDER BY "Stock", "Name";

-- ===== ระดับ 3 สรุปข้อมูล =====

-- ข้อ 11
SELECT "Role", COUNT(*) AS total
FROM "Users"
GROUP BY "Role"
ORDER BY total DESC;

-- ข้อ 12
SELECT "Industry", ROUND(AVG("Headcount"), 0) AS avg_headcount, COUNT(*) AS companies
FROM "Employers"
GROUP BY "Industry"
HAVING AVG("Headcount") > 580
ORDER BY avg_headcount DESC;

-- ข้อ 13
SELECT "Segment", ROUND(AVG("CreditLimit"), 0) AS avg_credit, COUNT(*) AS customers
FROM "Customers"
WHERE "IsActive" = true
GROUP BY "Segment"
ORDER BY avg_credit DESC;

-- ข้อ 14
SELECT "Channel", COUNT(*) AS orders
FROM "Orders"
WHERE "Status" <> 'cancelled'
GROUP BY "Channel"
ORDER BY orders DESC;

-- ข้อ 15
SELECT "Category", ROUND(AVG("Price"), 2) AS avg_price, COUNT(*) AS products
FROM "Products"
GROUP BY "Category"
ORDER BY avg_price DESC
LIMIT 1;

-- ===== ระดับ 4 JOIN =====

-- ข้อ 16
SELECT u."Name" AS user_name, u."Occupation", e."Name" AS employer
FROM "Users" u
LEFT JOIN "Employers" e ON e."Id" = u."EmployerId"
ORDER BY u."Id";

-- ข้อ 17
SELECT o."Id" AS order_id, c."Name" AS customer, c."Province", o."OrderedAt"
FROM "Orders" o
JOIN "Customers" c ON c."Id" = o."CustomerId"
WHERE o."Status" = 'cancelled'
ORDER BY o."OrderedAt" DESC;

-- ข้อ 18
SELECT p."Name", i."Quantity", i."UnitPrice",
       i."Quantity" * i."UnitPrice" AS line_total
FROM "OrderItems" i
JOIN "Products" p ON p."Id" = i."ProductId"
WHERE i."OrderId" = 1
ORDER BY i."Id";

-- ข้อ 19
SELECT e."Id", e."Name", e."Industry"
FROM "Employers" e
LEFT JOIN "Products" p ON p."EmployerId" = e."Id"
WHERE p."Id" IS NULL
ORDER BY e."Id";

-- ข้อ 20
SELECT u."Name", COUNT(*) AS shipped_orders
FROM "Users" u
JOIN "Orders" o ON o."UserId" = u."Id"
WHERE o."Status" = 'shipped'
GROUP BY u."Id", u."Name"
ORDER BY shipped_orders DESC, u."Name";

-- ===== ระดับ 5 งานในองค์กร =====

-- ข้อ 21
SELECT c."Name",
       COUNT(DISTINCT o."Id") AS orders,
       SUM(i."Quantity" * i."UnitPrice") AS revenue
FROM "Customers" c
JOIN "Orders" o ON o."CustomerId" = c."Id"
JOIN "OrderItems" i ON i."OrderId" = o."Id"
WHERE o."Status" IN ('paid', 'shipped')
GROUP BY c."Id", c."Name"
ORDER BY revenue DESC
LIMIT 5;

-- ข้อ 22
SELECT DATE_TRUNC('month', o."OrderedAt") AS month,
       o."Channel",
       COUNT(DISTINCT o."Id") AS orders,
       SUM(i."Quantity" * i."UnitPrice") AS revenue
FROM "Orders" o
JOIN "OrderItems" i ON i."OrderId" = o."Id"
WHERE o."Status" <> 'cancelled'
  AND o."OrderedAt" >= '2025-01-01'
  AND o."OrderedAt" < '2026-01-01'
GROUP BY 1, 2
ORDER BY 1, revenue DESC;

-- ข้อ 23
WITH product_revenue AS (
  SELECT p."Category",
         p."Name",
         SUM(i."Quantity" * i."UnitPrice") AS revenue
  FROM "Products" p
  JOIN "OrderItems" i ON i."ProductId" = p."Id"
  JOIN "Orders" o ON o."Id" = i."OrderId"
  WHERE o."Status" IN ('paid', 'shipped')
  GROUP BY p."Id", p."Category", p."Name"
),
ranked AS (
  SELECT *,
         RANK() OVER (PARTITION BY "Category" ORDER BY revenue DESC) AS rnk
  FROM product_revenue
)
SELECT "Category", "Name", revenue
FROM ranked
WHERE rnk = 1
ORDER BY revenue DESC;

-- ข้อ 24
WITH sales AS (
  SELECT u."Id",
         u."Name",
         SUM(i."Quantity" * i."UnitPrice") AS revenue
  FROM "Users" u
  JOIN "Orders" o ON o."UserId" = u."Id" AND o."Status" <> 'cancelled'
  JOIN "OrderItems" i ON i."OrderId" = o."Id"
  GROUP BY u."Id", u."Name"
)
SELECT "Name", revenue
FROM sales
WHERE revenue > (SELECT AVG(revenue) FROM sales)
ORDER BY revenue DESC;

-- ข้อ 25
WITH sales AS (
  SELECT p."EmployerId",
         SUM(i."Quantity" * i."UnitPrice") AS revenue
  FROM "Products" p
  JOIN "OrderItems" i ON i."ProductId" = p."Id"
  JOIN "Orders" o ON o."Id" = i."OrderId"
  WHERE o."Status" IN ('paid', 'shipped')
  GROUP BY p."EmployerId"
)
SELECT e."Industry",
       COUNT(*) FILTER (WHERE e."Status" = 'active') AS active_companies,
       SUM(e."AnnualRevenue") AS annual_revenue,
       COUNT(DISTINCT p."Id") AS products,
       COALESCE(SUM(s.revenue), 0) AS product_sales
FROM "Employers" e
LEFT JOIN "Products" p ON p."EmployerId" = e."Id"
LEFT JOIN sales s ON s."EmployerId" = e."Id"
GROUP BY e."Industry"
ORDER BY product_sales DESC;
