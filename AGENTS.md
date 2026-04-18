# 📌 GLOBAL RULE - WEBSITE BÁN CAFE GIẢI KHÁT

## 🧾 Thông tin dự án
- **Tên đề tài**: Xây dựng website bán sản phẩm cafe giải khát  
- **Công nghệ sử dụng**:
  - Backend: ASP.NET Core
  - Database: SQL Server
  - Frontend: (Razor Pages / MVC / React tùy triển khai)
- **Mục tiêu**:
  - Xây dựng hệ thống bán hàng online cho cafe & đồ uống
  - Quản lý sản phẩm, đơn hàng, người dùng
  - Tích hợp thanh toán online

---

## 🧱 Kiến trúc hệ thống
- Áp dụng mô hình **3-tier architecture**:
  - Presentation Layer (UI)
  - Business Logic Layer (Services)
  - Data Access Layer (Repository / Entity Framework)

- Sử dụng:
  - Entity Framework Core (ORM)
  - RESTful API (nếu tách frontend)

---

## 👤 Phân quyền hệ thống
- **Admin**
  - Toàn quyền quản lý hệ thống
- **User**
  - Mua hàng, quản lý tài khoản cá nhân

---

## ⚙️ Chức năng hệ thống

### 1. 🔐 ADMIN

#### 1.1 Quản lý loại sản phẩm
- Thêm / sửa / xóa loại sản phẩm
- Phân loại cafe, trà, nước giải khát,...

#### 1.2 Quản lý sản phẩm
- CRUD sản phẩm
- Upload hình ảnh
- Giá, mô tả, tồn kho

#### 1.3 Quản lý người dùng
- Xem danh sách user
- Khóa / mở tài khoản
- Phân quyền

#### 1.4 Quản lý đơn hàng
- Xem danh sách đơn
- Cập nhật trạng thái:
  - Pending
  - Processing
  - Completed
  - Cancelled

#### 1.5 Quản lý mã giảm giá
- Tạo mã discount
- Thiết lập:
  - % giảm
  - Ngày hết hạn
  - Số lần sử dụng

#### 1.6 Thống kê doanh thu
- Theo:
  - Tháng
  - Năm
  - Loại sản phẩm
- Biểu đồ (chart)

---

### 2. 🛒 USER

#### 2.1 Danh sách sản phẩm
- Hiển thị sản phẩm
- Lọc theo loại

#### 2.2 Chi tiết sản phẩm
- Hình ảnh
- Mô tả
- Giá
- Đánh giá (optional)

#### 2.3 Giỏ hàng
- Thêm / xóa sản phẩm
- Cập nhật số lượng

#### 2.4 Thanh toán
- COD (nếu có)
- Online qua **VNPAY**

#### 2.5 Tìm kiếm sản phẩm
- Theo tên
- Theo loại

#### 2.6 Profile
- Thông tin cá nhân
- Lịch sử đơn hàng

#### 2.7 Authentication
- Đăng ký qua email
- Đăng nhập
- Mã hóa mật khẩu (bcrypt)

---

## 💳 Tích hợp thanh toán VNPAY
- Redirect đến VNPAY Gateway
- Xử lý callback:
  - Success
  - Failed
- Verify checksum

---

## 🗄️ Database (SQL Server)

### Các bảng chính:
- Users
- Roles
- Products
- Categories
- Orders
- OrderDetails
- Coupons
- Payments

---

## 🔒 Bảo mật
- JWT Authentication hoặc Cookie Auth
- Hash password
- Validate input
- Chống SQL Injection (EF Core)

---

## 🚀 Quy ước coding
- Clean Code
- Naming convention:
  - PascalCase: Class
  - camelCase: variable
- Tách Service & Repository
- Dependency Injection

---

## 📈 Mở rộng (Future)
- Review sản phẩm
- Chat support
- AI gợi ý sản phẩm
- Mobile app

---

## 📌 Ghi chú
- Ưu tiên UX/UI đơn giản, dễ dùng
- Tối ưu performance query
- Logging & exception handling đầy đủ