# Dữ liệu thử (`sample-data`)

## Kết nối SQL Server (giống Docker Compose mặc định)

Khi chạy SQL Server bằng [`docker/compose.yaml`](../../docker/compose.yaml):

| Thông số | Giá trị (mặc định) |
|----------|---------------------|
| Server | `127.0.0.1` hoặc `localhost` |
| Cổng | `1433` |
| User | `sa` |
| Mật khẩu | Giống biến `MSSQL_SA_PASSWORD` trong compose (mặc định: `1234qwer##`) |
| Database | `CafeShopDb` (sau khi đã chạy migration / khởi động app ít nhất một lần) |

Chuỗi kết nối mẫu (chạy app trên máy host, SQL trong Docker):

`Server=127.0.0.1,1433;Database=CafeShopDb;User Id=sa;Password=...;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True`

**Lưu ý:** Trong `appsettings.json` của project có thể đang dùng host `sqlserver-cafeshop` (phù hợp app chạy trong Docker). Khi xuất dữ liệu từ máy bạn, dùng `127.0.0.1` hoặc `localhost`.

## Xuất toàn bộ schema + dữ liệu ra file `.sql` (tự động)

Script [`export-db.sh`](export-db.sh) dùng **SqlPackage** (Microsoft):

1. **Extract** database thành file `.dacpac` (có bật dữ liệu bảng).
2. **Script** từ `.dacpac` ra file SQL (thường gồm `CREATE` + `INSERT`).

Chạy từ thư mục gốc repo (hoặc từ bất kỳ đâu):

```bash
cd setup/sample-data
chmod +x export-db.sh   # lần đầu
./export-db.sh
```

Tuỳ chỉnh bằng biến môi trường (tùy chọn):

```bash
export SQL_SERVER=127.0.0.1
export SQL_PORT=1433
export SQL_DATABASE=CafeShopDb
export SQL_USER=sa
export SQL_PASSWORD='mật-khẩu-của-bạn'
./export-db.sh
```

Kết quả mặc định: [`exported-database.sql`](exported-database.sql) (ghi đè nếu chạy lại).

Lần đầu script sẽ tải **sqlpackage** vào thư mục `.tools/` (đã thêm vào `.gitignore`).

## File có sẵn

- [`script.sql`](script.sql): script seed mẫu đi kèm source (roles, admin, dữ liệu demo) — khác với bản export đầy đủ từ DB thực tế.

## Cách thủ công (SSMS / Azure Data Studio)

Nếu script tự động lỗi (mạng, quyền, phiên bản SqlPackage):

1. Kết nối tới SQL Server với thông tin trên.
2. Chuột phải database `CafeShopDb` → **Tasks** → **Generate Scripts…** (SSMS) hoặc tương đương trong Azure Data Studio.
3. Chọn **Schema and data**, lưu file `.sql` vào `setup/sample-data/`.
