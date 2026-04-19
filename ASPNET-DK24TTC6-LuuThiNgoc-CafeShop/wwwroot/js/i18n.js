/**
 * CoffeeShop — i18n Translation Engine
 * Client-side VI/EN translation with 30-day cookie persistence.
 */
(function () {
    'use strict';

    /* ===== Translation Dictionaries ===== */
    const translations = {
        vi: {
            // --- Navbar ---
            "nav.home": "Trang chủ",
            "nav.shop": "Cửa hàng",
            "nav.our_story": "Câu chuyện",
            "nav.guides": "Hướng dẫn",

            // --- Footer ---
            "footer.copy": "© 2024 CoffeeShop. Tất cả quyền được bảo lưu.",
            "footer.privacy": "Chính sách bảo mật",
            "footer.terms": "Điều khoản dịch vụ",
            "footer.shipping": "Thông tin vận chuyển",
            "footer.contact": "Liên hệ",

            // --- Home ---
            "home.hero_label": "Cà phê thượng hạng",
            "home.hero_title": "Đánh thức\nGiác quan.",
            "home.hero_text": "Trải nghiệm nghệ thuật pha chế hoàn hảo. Nguồn gốc đạo đức, rang cẩn thận, giao hàng tận tâm.",
            "home.shop_now": "Mua ngay",
            "home.explore": "Khám phá:",
            "home.all_offerings": "Tất cả sản phẩm",
            "home.featured_title": "Sản phẩm nổi bật",
            "home.featured_subtitle": "Được chọn lọc dành cho bạn.",
            "home.view_all": "Xem tất cả →",
            "home.no_products": "Chưa có sản phẩm nào.",
            "home.seasonal_label": "Sản phẩm theo mùa",
            "home.seasonal_title": "Bộ sưu tập mùa đông.",
            "home.seasonal_text": "Hỗn hợp ấm áp được tạo ra cho những buổi sáng lạnh giá. Hương vị mận gia vị, hồ đào nướng và ca cao đen. Số lượng có hạn.",
            "home.seasonal_btn": "Khám phá hương vị",

            // --- Auth ---
            "auth.login_subtitle": "Đăng nhập để tiếp tục.",
            "auth.register_subtitle": "Bắt đầu hành trình cùng chúng tôi.",
            "auth.email_label": "Địa chỉ Email",
            "auth.password_label": "Mật khẩu",
            "auth.forgot": "Quên mật khẩu?",
            "auth.login_btn": "Đăng nhập",
            "auth.no_account": "Chưa có tài khoản?",
            "auth.register_link": "Đăng ký",
            "auth.fullname_label": "Họ và tên",
            "auth.confirm_password": "Xác nhận mật khẩu",
            "auth.signup_btn": "Đăng ký",
            "auth.has_account": "Đã có tài khoản?",
            "auth.login_link": "Đăng nhập",

            // --- Shop ---
            "shop.title": "Bộ sưu tập\nĐặc biệt",
            "shop.desc": "Các loại cà phê chọn lọc và đặc biệt theo mùa. Rang để tôn lên hương vị tự nhiên.",
            "shop.showing": "Hiển thị",
            "shop.offerings": "sản phẩm",
            "shop.refine": "Bộ lọc",
            "shop.category": "Danh mục",
            "shop.all": "Tất cả",
            "shop.no_products": "Không tìm thấy sản phẩm.",
            "shop.view_all": "Xem tất cả",
            "shop.add_to_cart": "Thêm vào giỏ",

            // --- Product Detail ---
            "detail.back": "Quay lại bộ sưu tập",
            "detail.grind": "Kiểu xay",
            "detail.whole_bean": "Nguyên hạt",
            "detail.pour_over": "Pour Over",
            "detail.espresso": "Espresso",
            "detail.add_cart": "Thêm vào giỏ hàng",
            "detail.buy_now": "Mua ngay",
            "detail.category": "Danh mục",
            "detail.stock": "Tồn kho",
            "detail.status": "Trạng thái",
            "detail.available": "Còn hàng",
            "detail.out_of_stock": "Hết hàng",
            "detail.units": "sản phẩm",
            "detail.related": "Khám phá thêm",

            // --- Cart ---
            "cart.title": "Giỏ hàng",
            "cart.subtitle": "Xem lại các sản phẩm đã chọn.",
            "cart.empty": "Giỏ hàng trống. Khám phá bộ sưu tập!",
            "cart.continue": "Tiếp tục mua sắm",
            "cart.summary": "Tóm tắt đơn hàng",
            "cart.subtotal": "Tạm tính",
            "cart.shipping": "Phí vận chuyển",
            "cart.discount": "Giảm giá",
            "cart.total": "Tổng cộng",
            "cart.promo": "Mã ưu đãi",
            "cart.apply": "Áp dụng",
            "cart.payment": "Phương thức thanh toán",
            "cart.cod": "Thanh toán khi nhận hàng (COD)",
            "cart.complete": "Hoàn tất đơn hàng",
            "cart.secure": "Thanh toán an toàn",
            "cart.unit": "/ sản phẩm",
            "payment.cod": "Thanh toán khi nhận hàng (COD)",
            "payment.vnpay": "Thanh toán qua VNPAY",

            // --- Checkout ---
            "checkout.title": "Thanh toán",
            "checkout.subtitle": "Hoàn tất thông tin đơn hàng.",
            "checkout.shipping_info": "Thông tin giao hàng",
            "checkout.address": "Địa chỉ giao hàng",
            "checkout.phone": "Số điện thoại",
            "checkout.notes": "Ghi chú (tùy chọn)",
            "checkout.promo": "Mã ưu đãi",
            "checkout.payment": "Phương thức thanh toán",
            "checkout.place_order": "Đặt hàng",
            "checkout.secure": "Thanh toán an toàn",

            // --- Order Success ---
            "success.title": "Đơn hàng đã được xác nhận!",
            "success.text": "Cảm ơn bạn đã đặt hàng! Đơn hàng đang được chuẩn bị cẩn thận.",
            "success.order_number": "Mã đơn hàng",
            "success.estimated": "Giao hàng dự kiến",
            "success.payment": "Phương thức thanh toán",
            "success.cod": "Thanh toán khi nhận hàng",
            "success.view_order": "Xem đơn hàng",
            "success.continue": "Tiếp tục mua sắm",

            // --- Order History ---
            "history.title": "Lịch sử\nĐơn hàng.",
            "history.subtitle": "Theo dõi mọi đơn hàng từ đặt đến giao.",
            "history.empty": "Bạn chưa có đơn hàng nào.",
            "history.start": "Bắt đầu mua sắm",
            "history.view": "Xem chi tiết",

            // --- Order Details ---
            "order.back": "Quay lại đơn hàng",
            "order.product": "Sản phẩm",
            "order.unit_price": "Đơn giá",
            "order.qty": "SL",
            "order.subtotal": "Thành tiền",
            "order.info": "Thông tin đơn hàng",
            "order.delivery": "Chi tiết giao hàng",
            "order.address": "Địa chỉ:",
            "order.phone": "Điện thoại:",
            "order.notes": "Ghi chú:",
            "order.coupon": "Mã giảm giá:",
            "order.payment_method": "Thanh toán:",

            // --- Statuses ---
            "status.pending": "Chờ xử lý",
            "status.processing": "Đang xử lý",
            "status.completed": "Hoàn thành",
            "status.cancelled": "Đã hủy",
            "status.shipped": "Đang giao",
            "status.active": "Hoạt động",
            "status.inactive": "Không hoạt động",

            // --- Admin Sidebar ---
            "admin.title": "CoffeeShop Admin",
            "admin.subtitle": "Quản lý cửa hàng",
            "admin.dashboard": "Tổng quan",
            "admin.inventory": "Kho hàng",
            "admin.categories": "Danh mục",
            "admin.orders": "Đơn hàng",
            "admin.users": "Người dùng",
            "admin.coupons": "Mã giảm giá",
            "admin.view_store": "Xem cửa hàng",
            "admin.logout": "Đăng xuất",

            // --- Admin Dashboard ---
            "dash.title": "Tổng quan cửa hàng",
            "dash.subtitle": "Theo dõi hiệu suất kinh doanh của bạn.",
            "dash.revenue": "Doanh thu",
            "dash.orders_label": "Đơn hàng",
            "dash.products_label": "Sản phẩm",
            "dash.customers": "Khách hàng",
            "dash.total_revenue": "Tổng doanh thu",
            "dash.total_orders": "Tổng đơn hàng",
            "dash.active_inventory": "Sản phẩm đang bán",
            "dash.registered_users": "Người dùng đã đăng ký",
            "dash.revenue_trends": "Xu hướng doanh thu",
            "dash.monthly_revenue": "Doanh thu tháng cho",
            "dash.by_category": "Doanh thu theo danh mục",

            // --- Admin Product ---
            "aproduct.title": "Quản lý sản phẩm",
            "aproduct.subtitle": "Quản lý kho sản phẩm của bạn.",
            "aproduct.add": "Thêm sản phẩm",
            "aproduct.name": "Tên sản phẩm",
            "aproduct.category": "Danh mục",
            "aproduct.price": "Giá",
            "aproduct.stock": "Tồn kho",
            "aproduct.status": "Trạng thái",
            "aproduct.actions": "Thao tác",
            "aproduct.create_title": "Tạo sản phẩm mới",
            "aproduct.create_subtitle": "Thêm sản phẩm mới vào kho.",
            "aproduct.edit_title": "Chỉnh sửa sản phẩm",
            "aproduct.edit_subtitle": "Cập nhật thông tin sản phẩm.",
            "aproduct.description": "Mô tả",
            "aproduct.image": "Hình ảnh sản phẩm",
            "aproduct.save": "Lưu sản phẩm",
            "aproduct.cancel": "Hủy",
            "aproduct.upload_text": "Kéo thả hoặc nhấn để tải ảnh",
            "aproduct.upload_hint": "PNG, JPG dưới 5MB",
            "aproduct.current_image": "Ảnh hiện tại",
            "aproduct.delete_title": "Xác nhận xóa sản phẩm",
            "aproduct.delete_msg_prefix": "Bạn có chắc muốn xóa sản phẩm ",
            "aproduct.delete_msg_suffix": "?",
            "aproduct.delete_hint": "Hành động này không thể hoàn tác.",

            // --- Admin Category ---
            "acat.title": "Quản lý danh mục",
            "acat.subtitle": "Tổ chức sản phẩm theo danh mục.",
            "acat.add": "Thêm danh mục",
            "acat.name": "Tên danh mục",
            "acat.description": "Mô tả",
            "acat.save": "Lưu danh mục",
            "acat.create_title": "Tạo danh mục mới",
            "acat.create_subtitle": "Thêm danh mục sản phẩm mới.",
            "acat.edit_title": "Chỉnh sửa danh mục",
            "acat.edit_subtitle": "Cập nhật thông tin danh mục.",
            "acat.products_count": "sản phẩm",
            "acat.delete_title": "Xác nhận xóa danh mục",
            "acat.delete_msg_prefix": "Bạn có chắc muốn xóa danh mục ",
            "acat.delete_msg_suffix": "?",
            "acat.delete_hint": "Hành động này không thể hoàn tác.",

            // --- Admin Order ---
            "aorder.title": "Quản lý đơn hàng",
            "aorder.subtitle": "Theo dõi và xử lý đơn hàng.",
            "aorder.customer": "Khách hàng",
            "aorder.date": "Ngày đặt",
            "aorder.total_col": "Tổng tiền",
            "aorder.status": "Trạng thái",
            "aorder.actions": "Thao tác",
            "aorder.view": "Xem",
            "aorder.detail_title": "Chi tiết đơn hàng",
            "aorder.update_status": "Cập nhật trạng thái",
            "aorder.update_btn": "Cập nhật",
            "aorder.items": "Sản phẩm đã đặt",
            "aorder.customer_info": "Thông tin khách hàng",

            // --- Admin User ---
            "auser.title": "Quản lý người dùng",
            "auser.subtitle": "Quản lý tài khoản người dùng.",
            "auser.name": "Người dùng",
            "auser.email": "Email",
            "auser.role": "Vai trò",
            "auser.status": "Trạng thái",
            "auser.actions": "Thao tác",
            "auser.lock": "Khóa",
            "auser.unlock": "Mở khóa",

            // --- Admin Coupon ---
            "acoupon.title": "Quản lý mã giảm giá",
            "acoupon.subtitle": "Tạo và quản lý mã ưu đãi.",
            "acoupon.add": "Tạo mã mới",
            "acoupon.code": "Mã",
            "acoupon.discount": "Giảm giá",
            "acoupon.usage": "Sử dụng",
            "acoupon.expiry": "Ngày hết hạn",
            "acoupon.status": "Trạng thái",
            "acoupon.actions": "Thao tác",
            "acoupon.create_title": "Tạo mã giảm giá mới",
            "acoupon.create_subtitle": "Thiết lập mã ưu đãi mới.",
            "acoupon.edit_title": "Chỉnh sửa mã giảm giá",
            "acoupon.edit_subtitle": "Cập nhật thông tin mã giảm giá.",
            "acoupon.code_label": "Mã giảm giá",
            "acoupon.percent": "Phần trăm giảm",
            "acoupon.max_usage": "Số lần sử dụng tối đa",
            "acoupon.expiry_label": "Ngày hết hạn",
            "acoupon.save": "Lưu mã giảm giá",
            "acoupon.delete_title": "Xác nhận xóa mã giảm giá",
            "acoupon.delete_msg_prefix": "Bạn có chắc muốn xóa mã giảm giá ",
            "acoupon.delete_msg_suffix": "?",
            "acoupon.delete_hint": "Hành động này không thể hoàn tác.",

            // --- Profile ---
            "profile.member_since": "Thành viên từ",
            "profile.edit_btn": "Chỉnh sửa hồ sơ",
            "profile.change_pw_btn": "Đổi mật khẩu",
            "profile.total_orders": "Tổng đơn hàng",
            "profile.total_spent": "Tổng chi tiêu",
            "profile.membership": "Thời gian thành viên",
            "profile.recent_orders": "Đơn hàng gần đây",
            "profile.view_all_orders": "Xem tất cả →",
            "profile.no_orders": "Bạn chưa có đơn hàng nào.",
            "profile.start_shopping": "Bắt đầu mua sắm",
            "profile.back": "Quay lại hồ sơ",
            "profile.edit_title": "Chỉnh sửa hồ sơ",
            "profile.edit_subtitle": "Cập nhật thông tin cá nhân của bạn.",
            "profile.address_label": "Địa chỉ",
            "profile.phone_label": "Số điện thoại",
            "profile.save_btn": "Lưu thay đổi",
            "profile.change_pw_title": "Đổi mật khẩu",
            "profile.change_pw_subtitle": "Nhập mật khẩu hiện tại và mật khẩu mới.",
            "profile.current_pw": "Mật khẩu hiện tại",
            "profile.new_pw": "Mật khẩu mới",
            "profile.confirm_new_pw": "Xác nhận mật khẩu mới",

            // --- Common ---
            "common.edit": "Sửa",
            "common.delete": "Xóa",
            "common.save": "Lưu",
            "common.cancel": "Hủy",
            "common.search_products": "Tìm kiếm sản phẩm...",
            "common.search_orders": "Tìm kiếm đơn hàng...",
            "common.search_users": "Tìm kiếm người dùng...",

            // --- Placeholders ---
            "ph.email": "email@cuaban.com",
            "ph.fullname": "Nguyễn Văn A",
            "ph.product_name": "VD: Cà Phê Đen Đá",
            "ph.product_desc": "Mô tả sản phẩm...",
            "ph.category_name": "VD: Cà phê đen",
            "ph.category_desc": "Mô tả danh mục này...",
            "ph.coupon_code": "VD: GIAM20",
            "ph.address": "Nhập địa chỉ đầy đủ của bạn",
            "ph.notes": "Ghi chú thêm cá nhân",
            "ph.promo": "Nhập mã ưu đãi (nếu có)",
            "ph.promo_cart": "Nhập mã ưu đãi",
        },

        en: {
            // --- Navbar ---
            "nav.home": "Home",
            "nav.shop": "Shop",
            "nav.our_story": "Our Story",
            "nav.guides": "Coffee Guides",

            // --- Footer ---
            "footer.copy": "© 2024 CoffeeShop. All rights reserved.",
            "footer.privacy": "Privacy Policy",
            "footer.terms": "Terms of Service",
            "footer.shipping": "Shipping Info",
            "footer.contact": "Contact Us",

            // --- Home ---
            "home.hero_label": "Curated Roasts",
            "home.hero_title": "Awaken Your\nSenses.",
            "home.hero_text": "Experience the art of the perfect pour. Ethically sourced, carefully roasted, and delivered with warmth.",
            "home.shop_now": "Shop Now",
            "home.explore": "Explore:",
            "home.all_offerings": "All Offerings",
            "home.featured_title": "Featured Pours",
            "home.featured_subtitle": "Hand-selected for your daily ritual.",
            "home.view_all": "View All →",
            "home.no_products": "No products available yet.",
            "home.seasonal_label": "Seasonal Release",
            "home.seasonal_title": "Winter Harvest Reserve.",
            "home.seasonal_text": "A comforting blend crafted for colder mornings. Notes of spiced plum, toasted pecan, and dark cocoa wrapper. Limited small-batch availability.",
            "home.seasonal_btn": "Discover the Blend",

            // --- Auth ---
            "auth.login_subtitle": "Sign in to continue your ritual.",
            "auth.register_subtitle": "Curated Rituals. Begin yours today.",
            "auth.email_label": "Email Address",
            "auth.password_label": "Password",
            "auth.forgot": "Forgot Password?",
            "auth.login_btn": "Login",
            "auth.no_account": "Don't have an account?",
            "auth.register_link": "Register",
            "auth.fullname_label": "Full Name",
            "auth.confirm_password": "Confirm Password",
            "auth.signup_btn": "Sign Up",
            "auth.has_account": "Already have an account?",
            "auth.login_link": "Log in",

            // --- Shop ---
            "shop.title": "The Reserve\nCollection",
            "shop.desc": "Curated micro-lots and seasonal single origins. Roasted to highlight their inherent organic warmth and distinct terroir.",
            "shop.showing": "Showing",
            "shop.offerings": "offerings",
            "shop.refine": "Refine",
            "shop.category": "Category",
            "shop.all": "All",
            "shop.no_products": "No products found.",
            "shop.view_all": "View All",
            "shop.add_to_cart": "Add to cart",

            // --- Product Detail ---
            "detail.back": "Back to Collection",
            "detail.grind": "Grind Preference",
            "detail.whole_bean": "Whole Bean",
            "detail.pour_over": "Pour Over",
            "detail.espresso": "Espresso",
            "detail.add_cart": "Add to Cart",
            "detail.buy_now": "Buy Now",
            "detail.category": "Category",
            "detail.stock": "Stock",
            "detail.status": "Status",
            "detail.available": "Available",
            "detail.out_of_stock": "Out of Stock",
            "detail.units": "units",
            "detail.related": "Explore the Roastery",

            // --- Cart ---
            "cart.title": "Your Cart",
            "cart.subtitle": "Review your selected beans and brewing gear.",
            "cart.empty": "Your cart is empty. Explore our collection!",
            "cart.continue": "Continue Browsing",
            "cart.summary": "Order Summary",
            "cart.subtotal": "Subtotal",
            "cart.shipping": "Shipping",
            "cart.discount": "Discount",
            "cart.total": "Total",
            "cart.promo": "Promo Code",
            "cart.apply": "Apply",
            "cart.payment": "Payment Method",
            "cart.cod": "Cash on Delivery (COD)",
            "cart.complete": "Complete Order",
            "cart.secure": "Secure checkout",
            "cart.unit": "/ unit",
            "payment.cod": "Cash on Delivery (COD)",
            "payment.vnpay": "Pay with VNPAY",

            // --- Checkout ---
            "checkout.title": "Checkout",
            "checkout.subtitle": "Complete your order details.",
            "checkout.shipping_info": "Shipping Information",
            "checkout.address": "Delivery Address",
            "checkout.phone": "Phone Number",
            "checkout.notes": "Notes (optional)",
            "checkout.promo": "Promo Code",
            "checkout.payment": "Payment Method",
            "checkout.place_order": "Place Order",
            "checkout.secure": "Secure checkout",

            // --- Order Success ---
            "success.title": "Your Ritual Awaits.",
            "success.text": "Thank you for your order! Your beans are being carefully prepared for their journey to you.",
            "success.order_number": "Order Number",
            "success.estimated": "Estimated Delivery",
            "success.payment": "Payment Method",
            "success.cod": "Cash on Delivery",
            "success.view_order": "View Order",
            "success.continue": "Continue Browsing",

            // --- Order History ---
            "history.title": "Your Order\nHistory.",
            "history.subtitle": "Track every order from brew to doorstep.",
            "history.empty": "You haven't placed any orders yet.",
            "history.start": "Start Shopping",
            "history.view": "View Details",

            // --- Order Details ---
            "order.back": "Back to Orders",
            "order.product": "Product",
            "order.unit_price": "Unit Price",
            "order.qty": "Qty",
            "order.subtotal": "Subtotal",
            "order.info": "Order Info",
            "order.delivery": "Delivery Details",
            "order.address": "Address:",
            "order.phone": "Phone:",
            "order.notes": "Notes:",
            "order.coupon": "Coupon:",
            "order.payment_method": "Payment:",

            // --- Statuses ---
            "status.pending": "Pending",
            "status.processing": "Processing",
            "status.completed": "Completed",
            "status.cancelled": "Cancelled",
            "status.shipped": "Shipped",
            "status.active": "Active",
            "status.inactive": "Inactive",

            // --- Admin Sidebar ---
            "admin.title": "CoffeeShop Admin",
            "admin.subtitle": "Roastery Management",
            "admin.dashboard": "Dashboard",
            "admin.inventory": "Inventory",
            "admin.categories": "Categories",
            "admin.orders": "Orders",
            "admin.users": "Users",
            "admin.coupons": "Coupons",
            "admin.view_store": "View Store",
            "admin.logout": "Logout",

            // --- Admin Dashboard ---
            "dash.title": "Roastery Overview",
            "dash.subtitle": "Track your coffee business performance at a glance.",
            "dash.revenue": "Revenue",
            "dash.orders_label": "Orders",
            "dash.products_label": "Products",
            "dash.customers": "Customers",
            "dash.total_revenue": "Total revenue",
            "dash.total_orders": "Total orders",
            "dash.active_inventory": "Active inventory",
            "dash.registered_users": "Registered users",
            "dash.revenue_trends": "Revenue Trends",
            "dash.monthly_revenue": "Monthly revenue for",
            "dash.by_category": "Revenue by Category",

            // --- Admin Product ---
            "aproduct.title": "Product Management",
            "aproduct.subtitle": "Manage your product inventory.",
            "aproduct.add": "Add Product",
            "aproduct.name": "Product Name",
            "aproduct.category": "Category",
            "aproduct.price": "Price",
            "aproduct.stock": "Stock",
            "aproduct.status": "Status",
            "aproduct.actions": "Actions",
            "aproduct.create_title": "Create New Product",
            "aproduct.create_subtitle": "Add a new product to your inventory.",
            "aproduct.edit_title": "Edit Product",
            "aproduct.edit_subtitle": "Update product information.",
            "aproduct.description": "Description",
            "aproduct.image": "Product Image",
            "aproduct.save": "Save Product",
            "aproduct.cancel": "Cancel",
            "aproduct.upload_text": "Drag & drop or click to upload",
            "aproduct.upload_hint": "PNG, JPG under 5MB",
            "aproduct.current_image": "Current Image",
            "aproduct.delete_title": "Confirm Delete Product",
            "aproduct.delete_msg_prefix": "Are you sure you want to delete ",
            "aproduct.delete_msg_suffix": "?",
            "aproduct.delete_hint": "This action cannot be undone.",

            // --- Admin Category ---
            "acat.title": "Category Management",
            "acat.subtitle": "Organize products by category.",
            "acat.add": "Add Category",
            "acat.name": "Category Name",
            "acat.description": "Description",
            "acat.save": "Save Category",
            "acat.create_title": "Create New Category",
            "acat.create_subtitle": "Add a new product category.",
            "acat.edit_title": "Edit Category",
            "acat.edit_subtitle": "Update category information.",
            "acat.products_count": "products",
            "acat.delete_title": "Confirm Delete Category",
            "acat.delete_msg_prefix": "Are you sure you want to delete ",
            "acat.delete_msg_suffix": "?",
            "acat.delete_hint": "This action cannot be undone.",

            // --- Admin Order ---
            "aorder.title": "Order Management",
            "aorder.subtitle": "Track and manage customer orders.",
            "aorder.customer": "Customer",
            "aorder.date": "Date",
            "aorder.total_col": "Total",
            "aorder.status": "Status",
            "aorder.actions": "Actions",
            "aorder.view": "View",
            "aorder.detail_title": "Order Details",
            "aorder.update_status": "Update Status",
            "aorder.update_btn": "Update",
            "aorder.items": "Ordered Items",
            "aorder.customer_info": "Customer Info",

            // --- Admin User ---
            "auser.title": "User Management",
            "auser.subtitle": "Manage user accounts.",
            "auser.name": "User",
            "auser.email": "Email",
            "auser.role": "Role",
            "auser.status": "Status",
            "auser.actions": "Actions",
            "auser.lock": "Lock",
            "auser.unlock": "Unlock",

            // --- Admin Coupon ---
            "acoupon.title": "Coupon Management",
            "acoupon.subtitle": "Create and manage promotional codes.",
            "acoupon.add": "Create Coupon",
            "acoupon.code": "Code",
            "acoupon.discount": "Discount",
            "acoupon.usage": "Usage",
            "acoupon.expiry": "Expiry",
            "acoupon.status": "Status",
            "acoupon.actions": "Actions",
            "acoupon.create_title": "Create New Coupon",
            "acoupon.create_subtitle": "Set up a new promotional code.",
            "acoupon.edit_title": "Edit Coupon",
            "acoupon.edit_subtitle": "Update coupon information.",
            "acoupon.code_label": "Coupon Code",
            "acoupon.percent": "Discount Percent",
            "acoupon.max_usage": "Max Usage",
            "acoupon.expiry_label": "Expiry Date",
            "acoupon.save": "Save Coupon",
            "acoupon.delete_title": "Confirm Delete Coupon",
            "acoupon.delete_msg_prefix": "Are you sure you want to delete ",
            "acoupon.delete_msg_suffix": "?",
            "acoupon.delete_hint": "This action cannot be undone.",

            // --- Profile ---
            "profile.member_since": "Member since",
            "profile.edit_btn": "Edit Profile",
            "profile.change_pw_btn": "Change Password",
            "profile.total_orders": "Total Orders",
            "profile.total_spent": "Total Spent",
            "profile.membership": "Membership Duration",
            "profile.recent_orders": "Recent Orders",
            "profile.view_all_orders": "View All →",
            "profile.no_orders": "You haven't placed any orders yet.",
            "profile.start_shopping": "Start Shopping",
            "profile.back": "Back to Profile",
            "profile.edit_title": "Edit Profile",
            "profile.edit_subtitle": "Update your personal information.",
            "profile.address_label": "Address",
            "profile.phone_label": "Phone Number",
            "profile.save_btn": "Save Changes",
            "profile.change_pw_title": "Change Password",
            "profile.change_pw_subtitle": "Enter your current and new password.",
            "profile.current_pw": "Current Password",
            "profile.new_pw": "New Password",
            "profile.confirm_new_pw": "Confirm New Password",

            // --- Common ---
            "common.edit": "Edit",
            "common.delete": "Delete",
            "common.save": "Save",
            "common.cancel": "Cancel",
            "common.search_products": "Search products...",
            "common.search_orders": "Search orders...",
            "common.search_users": "Search users...",

            // --- Placeholders ---
            "ph.email": "your@email.com",
            "ph.fullname": "John Doe",
            "ph.product_name": "e.g. Colombian Dark Roast",
            "ph.product_desc": "Describe the product...",
            "ph.category_name": "e.g. Single Origin",
            "ph.category_desc": "Describe this category...",
            "ph.coupon_code": "e.g. SAVE20",
            "ph.address": "Enter your full address",
            "ph.notes": "Any special instructions",
            "ph.promo": "Enter promo code (if any)",
            "ph.promo_cart": "Enter promo code",
        }
    };

    /* ===== Cookie Helpers ===== */
    function getCookie(name) {
        const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
        return match ? match[2] : null;
    }

    function setCookie(name, value, days) {
        const d = new Date();
        d.setTime(d.getTime() + days * 86400000);
        document.cookie = name + '=' + value + ';expires=' + d.toUTCString() + ';path=/;SameSite=Lax';
    }

    /* ===== Translation Logic ===== */
    function getLanguage() {
        return getCookie('lang') || 'vi';
    }

    function applyTranslations(lang) {
        const dict = translations[lang];
        if (!dict) return;

        // Translate text content
        document.querySelectorAll('[data-i18n]').forEach(function (el) {
            const key = el.getAttribute('data-i18n');
            if (dict[key] !== undefined) {
                // Handle line breaks in translations
                if (dict[key].includes('\n')) {
                    el.innerHTML = dict[key].replace(/\n/g, '<br/>');
                } else {
                    el.textContent = dict[key];
                }
            }
        });

        // Translate placeholders
        document.querySelectorAll('[data-i18n-ph]').forEach(function (el) {
            const key = el.getAttribute('data-i18n-ph');
            if (dict[key] !== undefined) {
                el.setAttribute('placeholder', dict[key]);
            }
        });

        // Translate title attributes
        document.querySelectorAll('[data-i18n-title]').forEach(function (el) {
            const key = el.getAttribute('data-i18n-title');
            if (dict[key] !== undefined) {
                el.setAttribute('title', dict[key]);
            }
        });

        // Update html lang attribute
        document.documentElement.lang = lang === 'vi' ? 'vi' : 'en';

        // Update toggle button text
        var toggleBtn = document.getElementById('langToggle');
        if (toggleBtn) {
            toggleBtn.innerHTML = lang === 'vi'
                ? '<i class="fas fa-globe"></i> EN'
                : '<i class="fas fa-globe"></i> VI';
            toggleBtn.setAttribute('title', lang === 'vi' ? 'Switch to English' : 'Chuyển sang Tiếng Việt');
        }
    }

    function setLanguage(lang) {
        setCookie('lang', lang, 30);
        applyTranslations(lang);
    }

    function toggleLanguage() {
        var current = getLanguage();
        setLanguage(current === 'vi' ? 'en' : 'vi');
    }

    /* ===== Init ===== */
    document.addEventListener('DOMContentLoaded', function () {
        applyTranslations(getLanguage());
    });

    // Expose globally
    window.CoffeeI18n = {
        setLanguage: setLanguage,
        getLanguage: getLanguage,
        toggleLanguage: toggleLanguage
    };
})();
