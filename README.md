# CafeShop - Website bán sản phẩm cafe giải khát

## Thông tin liên hệ nhóm tác giả
- **Tên:** Lưu Thị Ngọc
- **SĐT:** 0384001801
- **Email:** ngoclt120486@tvu-onschool.edu.vn

## Tổng quan dự án
CafeShop là ứng dụng web thương mại điện tử xây dựng bằng **ASP.NET Core 8 MVC**, tách lớp theo mô hình 3 tầng: `UI -> Service -> Data (EF Core)`.

Phạm vi hiện tại đã triển khai:
- Khu vực **User**: duyệt sản phẩm, tìm kiếm/lọc/sắp xếp, quản lý giỏ hàng, áp mã giảm giá, checkout COD, lịch sử đơn hàng, hồ sơ cá nhân.
- Khu vực **Admin**: quản lý danh mục, sản phẩm, mã giảm giá, người dùng, đơn hàng và dashboard thống kê.
- Tích hợp **ASP.NET Core Identity** cho đăng nhập/đăng ký và phân quyền `Admin` / `User`.

## Công nghệ sử dụng
- **Backend:** ASP.NET Core 8 MVC
- **Ngôn ngữ:** C#
- **ORM:** Entity Framework Core 8
- **Database:** SQL Server
- **Auth:** ASP.NET Core Identity + Cookie Authentication
- **Frontend:** Razor Views, HTML/CSS/JavaScript
- **Container DB:** Docker Compose (SQL Server 2022)

## Cấu trúc thư mục theo quy định nộp đồ án

| Thư mục | Nội dung |
|--------|----------|
| [`src/`](src/) | Mã nguồn ASP.NET Core và file giải pháp `ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.sln` |
| [`setup/`](setup/) | Bản `dotnet publish` (`setup/publish/`), hướng dẫn cài đặt, dữ liệu thử (`setup/sample-data/`) |
| [`thesis/`](thesis/) | Tài liệu đồ án: `doc/`, `pdf/`, `html/` (mockup web), `abs/` (ppt/video), `refs/` (sơ đồ draw.io, tham khảo) |
| [`progress-report/`](progress-report/) | Báo cáo tiến độ (bắt buộc) |
| [`docker/`](docker/) | `compose.yaml`, `Dockerfile` ứng dụng, `.dockerignore` |
| [`soft/`](soft/) | Ghi chú phần mềm liên quan (tùy chọn) |

## Kiến trúc và cấu trúc mã nguồn (trong `src/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop/`)

- `Controllers/`: luồng người dùng (`Home`, `Product`, `Cart`, `Order`, `Account`).
- `Areas/Admin/Controllers/`: luồng quản trị (`AdminDashboard`, `AdminProduct`, `AdminCategory`, `AdminCoupon`, `AdminOrder`, `AdminUser`).
- `Services/`: xử lý nghiệp vụ (`ProductService`, `OrderService`, `CouponService`, `DashboardService`, ...).
- `Data/ApplicationDbContext.cs`: cấu hình EF Core và quan hệ dữ liệu.
- `Data/script.sql`: seed role, tài khoản admin mặc định và dữ liệu mẫu danh mục/sản phẩm/coupon.

## Chức năng đã hoàn thành

### 1) Người dùng (User)
- Đăng ký, đăng nhập, đăng xuất.
- Xem danh sách sản phẩm có phân trang, lọc theo danh mục, tìm kiếm và sắp xếp.
- Xem chi tiết sản phẩm và sản phẩm liên quan.
- Quản lý giỏ hàng: thêm/xóa/cập nhật số lượng, kiểm tra tồn kho trước checkout.
- Áp dụng và gỡ mã giảm giá trực tiếp trong giỏ hàng.
- Thanh toán **COD** (cash on delivery), tạo đơn và trừ tồn kho theo trạng thái.
- Xem lịch sử đơn, chi tiết đơn, hủy đơn ở trạng thái `Pending`.
- Quản lý hồ sơ cá nhân, cập nhật thông tin, đổi mật khẩu.

### 2) Quản trị viên (Admin)
- Dashboard thống kê doanh thu theo năm/khoảng ngày.
- Xuất báo cáo doanh thu CSV.
- CRUD danh mục sản phẩm.
- CRUD sản phẩm (hỗ trợ upload ảnh file hoặc tải ảnh từ URL).
- CRUD mã giảm giá.
- Quản lý đơn hàng và cập nhật trạng thái (`Pending`, `Processing`, `Completed`, `Cancelled`) kèm xử lý tồn kho.
- Quản lý người dùng: tìm kiếm, khóa/mở khóa, đổi vai trò.

## Mô hình dữ liệu chính
- `ApplicationUser`, `IdentityRole`
- `Category`, `Product`
- `Order`, `OrderDetail`
- `Coupon`
- `PaymentTransaction`

## Hướng dẫn cài đặt và chạy

### 1. Yêu cầu
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Docker Desktop (nếu chạy SQL Server bằng container)
- IDE: Visual Studio 2022 / Rider / VS Code

### 2. Khởi động SQL Server bằng Docker
Chạy từ thư mục gốc project:
```bash
docker compose -f docker/compose.yaml up -d
```
Container DB: `sqlserver-cafeshop` tại cổng `1433`.

### 3. Cấu hình kết nối database
- Dự án dùng **một file cấu hình duy nhất**: `src/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop/appsettings.json`.
- Nếu chạy app local bằng IDE và DB chạy qua Docker port mapping, đặt `Server=localhost,1433`.
- Nếu chạy app trong môi trường container cùng network với SQL Server, có thể dùng `Server=sqlserver-cafeshop,1433`.

### 4. Restore packages và cập nhật database
```bash
cd src/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop
dotnet restore
dotnet ef database update
```

Hoặc mở và build từ file giải pháp:

```bash
dotnet restore src/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.sln
dotnet ef database update --project src/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.csproj
```

### 5. Chạy ứng dụng (development)
```bash
cd src/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop
dotnet watch run
```
Sau khi chạy, truy cập URL được in trên terminal (thường là `https://localhost:7198`).

### 6. Chạy bản publish / Docker

- Hướng dẫn chi tiết cho thư mục `setup/publish/`: xem [`setup/README.md`](setup/README.md).
- Build image ứng dụng: `docker build -f docker/Dockerfile -t cafeshop-web:latest src`

## Tài khoản và dữ liệu mẫu
- Dự án tự chạy migration và thực thi `Data/script.sql` khi khởi động.
- Script seed:
  - Roles: `Admin`, `User`
  - Tài khoản admin mặc định: `admin@cafeshop.com`
  - Dữ liệu mẫu cho danh mục, sản phẩm và coupon.

