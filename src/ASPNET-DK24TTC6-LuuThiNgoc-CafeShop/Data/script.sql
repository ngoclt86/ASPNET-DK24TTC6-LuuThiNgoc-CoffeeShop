SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'ADMIN')
BEGIN
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (CONVERT(nvarchar(450), NEWID()), N'Admin', N'ADMIN', CONVERT(nvarchar(36), NEWID()));
END;

IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'USER')
BEGIN
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (CONVERT(nvarchar(450), NEWID()), N'User', N'USER', CONVERT(nvarchar(36), NEWID()));
END;

DECLARE @adminUserId nvarchar(450) = (
    SELECT TOP 1 [Id]
    FROM [AspNetUsers]
    WHERE [NormalizedEmail] = N'ADMIN@CAFESHOP.COM'
);

IF @adminUserId IS NULL
BEGIN
    SET @adminUserId = CONVERT(nvarchar(450), NEWID());

    INSERT INTO [AspNetUsers]
    (
        [Id], [FullName], [Address], [IsLocked], [CreatedAt],
        [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed],
        [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed],
        [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]
    )
    VALUES
    (
        @adminUserId, N'Administrator', N'Hà Nội, Việt Nam', 0, GETDATE(),
        N'admin@cafeshop.com', N'ADMIN@CAFESHOP.COM', N'admin@cafeshop.com', N'ADMIN@CAFESHOP.COM', 1,
        N'AQAAAAIAAYagAAAAEDLV5fYyDx+wPEyoBVXHaQJiQKHq8+RWEd+VoK4ChnXYn7Ylng845SZw2tqzUy8pbg==',
        CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()), NULL, 0,
        0, NULL, 1, 0
    );
END;

DECLARE @adminRoleId nvarchar(450) = (
    SELECT TOP 1 [Id]
    FROM [AspNetRoles]
    WHERE [NormalizedName] = N'ADMIN'
);

IF @adminRoleId IS NOT NULL
   AND @adminUserId IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [AspNetUserRoles]
       WHERE [UserId] = @adminUserId
         AND [RoleId] = @adminRoleId
   )
BEGIN
    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (@adminUserId, @adminRoleId);
END;

IF NOT EXISTS (SELECT 1 FROM [Categories])
BEGIN
    SET IDENTITY_INSERT [Categories] ON;

    INSERT INTO [Categories] ([Id], [Name], [Description], [CreatedAt], [DeletedAt], [IsDeleted])
    VALUES
    (1, N'Cà phê', N'Các loại cà phê truyền thống và hiện đại', CAST(N'2026-04-19 14:25:37.5367890' AS datetime2(7)), NULL, 0),
    (2, N'Trà', N'Trà các loại, trà sữa, trà trái cây', CAST(N'2026-04-18 23:35:13.0785290' AS datetime2(7)), NULL, 0),
    (3, N'Nước ép', N'Nước ép trái cây tươi', CAST(N'2026-04-18 23:35:13.0785400' AS datetime2(7)), NULL, 0),
    (4, N'Sinh tố', N'Sinh tố các loại trái cây', CAST(N'2026-04-18 23:35:13.0785420' AS datetime2(7)), NULL, 0),
    (5, N'Đá xay', N'Đồ uống đá xay mát lạnh', CAST(N'2026-04-18 23:35:13.0785430' AS datetime2(7)), NULL, 0),
    (1002, N'Khác', NULL, CAST(N'2026-04-19 14:25:47.7056070' AS datetime2(7)), CAST(N'2026-04-19 14:34:28.9753530' AS datetime2(7)), 1);

    SET IDENTITY_INSERT [Categories] OFF;
END;

DECLARE @caPheCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Cà phê');
DECLARE @traCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Trà');
DECLARE @nuocEpCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Nước ép');
DECLARE @sinhToCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Sinh tố');
DECLARE @daXayCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Đá xay');

IF NOT EXISTS (SELECT 1 FROM [Products])
BEGIN
    SET IDENTITY_INSERT [Products] ON;

    INSERT INTO [Products] ([Id], [Name], [Description], [Price], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsActive], [DeletedAt], [IsDeleted])
    VALUES
    (1, N'Cà phê đen đá', N'Cà phê đen truyền thống pha phin, đậm đà hương vị Việt Nam', 25000.00, N'/uploads/products/ca-phe-den-da.jpg', 100, @caPheCatId, CAST(N'2026-04-18 23:35:13.1858000' AS datetime2(7)), 0, CAST(N'2026-04-19 14:23:14.5174570' AS datetime2(7)), 1),
    (2, N'Cà phê sữa đá', N'Cà phê sữa đá thơm ngon, béo ngậy', 29000.00, N'/uploads/products/ca-phe-sua-da.jpg', 100, @caPheCatId, CAST(N'2026-04-18 23:35:13.1859400' AS datetime2(7)), 1, NULL, 0),
    (3, N'Bạc xỉu', N'Cà phê sữa nhẹ nhàng, phù hợp cho người mới bắt đầu', 29000.00, N'/uploads/products/bac-xiu.jpg', 100, @caPheCatId, CAST(N'2026-04-18 23:35:13.1859410' AS datetime2(7)), 0, CAST(N'2026-04-19 19:11:18.0913160' AS datetime2(7)), 1),
    (4, N'Cappuccino', N'Cappuccino thơm ngon theo phong cách Ý', 45000.00, N'/uploads/products/cappuccino.jpg', 100, @caPheCatId, CAST(N'2026-04-18 23:35:13.1859420' AS datetime2(7)), 1, NULL, 0),
    (5, N'Latte', N'Latte mềm mại với lớp foam mịn', 45000.00, N'/uploads/products/a96201a7-0cb4-4d08-96cf-88cb70f5ee08.jpg', 100, @caPheCatId, CAST(N'2026-04-18 23:35:13.1859420' AS datetime2(7)), 1, NULL, 0),
    (6, N'Americano', N'Cà phê Americano đậm đà, thanh nhẹ', 39000.00, N'/uploads/products/671985d7-9999-4335-b972-b444676fdcb2.jpg', 100, @caPheCatId, CAST(N'2026-04-18 23:35:13.1859430' AS datetime2(7)), 1, NULL, 0),
    (7, N'Trà đào cam sả', N'Trà đào kết hợp cam và sả thơm mát', 35000.00, N'/uploads/products/91178ccb-5555-4095-aeb2-360a9f235303.jpg', 100, @traCatId, CAST(N'2026-04-18 23:35:13.1859440' AS datetime2(7)), 1, NULL, 0),
    (8, N'Trà sữa trân châu', N'Trà sữa truyền thống với trân châu dẻo mềm', 35000.00, N'/uploads/products/06e4522a-7cdd-4a6b-bb48-3c500749aae8.jpg', 100, @traCatId, CAST(N'2026-04-18 23:35:13.1859440' AS datetime2(7)), 1, NULL, 0),
    (9, N'Trà vải', N'Trà vải thơm ngát, ngọt tự nhiên', 32000.00, N'/uploads/products/057f503e-4917-4e39-a24f-24cd2ea372eb.png', 100, @traCatId, CAST(N'2026-04-18 23:35:13.1859450' AS datetime2(7)), 1, NULL, 0),
    (10, N'Trà matcha latte', N'Trà xanh matcha kết hợp sữa tươi', 45000.00, N'/uploads/products/baedfded-859e-4062-b011-ee0df8562c62.jpg', 100, @traCatId, CAST(N'2026-04-18 23:35:13.1859450' AS datetime2(7)), 1, NULL, 0),
    (11, N'Nước ép cam', N'Nước ép cam tươi nguyên chất, giàu vitamin C', 30000.00, N'/uploads/products/fba02125-7113-4edb-8d72-c8b7b5582b5b.jpg', 80, @nuocEpCatId, CAST(N'2026-04-18 23:35:13.1859460' AS datetime2(7)), 1, NULL, 0),
    (12, N'Nước ép dưa hấu', N'Nước ép dưa hấu mát lạnh giải nhiệt', 28000.00, N'/uploads/products/d720f28d-a3c2-4bb7-8991-0e0032d4505b.jpg', 80, @nuocEpCatId, CAST(N'2026-04-18 23:35:13.1859460' AS datetime2(7)), 1, NULL, 0),
    (13, N'Sinh tố bơ', N'Sinh tố bơ béo ngậy, bổ dưỡng', 35000.00, N'/uploads/products/d86f7d7c-ed51-4885-90b2-80fcc67578fa.jpg', 80, @sinhToCatId, CAST(N'2026-04-18 23:35:13.1859470' AS datetime2(7)), 1, NULL, 0),
    (14, N'Sinh tố xoài', N'Sinh tố xoài ngọt tự nhiên', 32000.00, N'/uploads/products/e3eb71bd-ef9d-43ae-b4fe-3680030ef720.jpg', 80, @sinhToCatId, CAST(N'2026-04-18 23:35:13.1859470' AS datetime2(7)), 1, NULL, 0),
    (15, N'Sinh tố dâu', N'Sinh tố dâu tây tươi mát', 35000.00, N'/uploads/products/69c9448e-d6ae-444a-8e43-d2ae50c9c66d.png', 80, @sinhToCatId, CAST(N'2026-04-18 23:35:13.1859480' AS datetime2(7)), 1, NULL, 0),
    (16, N'Chocolate đá xay', N'Chocolate đá xay mát lạnh, ngọt ngào', 45000.00, N'/uploads/products/4027dfcd-7923-448c-a2e2-eb169bf243dc.jpg', 80, @daXayCatId, CAST(N'2026-04-18 23:35:13.1859480' AS datetime2(7)), 1, NULL, 0),
    (17, N'Matcha đá xay', N'Matcha đá xay Nhật Bản chính hiệu', 49000.00, N'/uploads/products/3d0fa02f-43f6-4a49-94e2-2196c54ce927.jpg', 80, @daXayCatId, CAST(N'2026-04-18 23:35:13.1859480' AS datetime2(7)), 1, NULL, 0),
    (1002, N'Bạc xỉu', N'Bac xiu ngon', 35000.00, N'/uploads/products/59257a4c-97ae-49e4-881f-497067c3b78e.jpg', 0, @caPheCatId, CAST(N'2026-04-19 13:34:25.4062400' AS datetime2(7)), 1, NULL, 0),
    (1003, N'Nước dừa', N'Nước dừa ngon', 30000.00, N'/uploads/products/d8501d7b-30bf-4313-9423-585b57190e39.webp', 120, @nuocEpCatId, CAST(N'2026-04-19 13:35:10.4049540' AS datetime2(7)), 1, NULL, 0);

    SET IDENTITY_INSERT [Products] OFF;
END;

IF NOT EXISTS (SELECT 1 FROM [Coupons])
BEGIN
    INSERT INTO [Coupons] ([Code], [DiscountPercent], [ExpiryDate], [MaxUsage], [CurrentUsage], [IsActive], [IsDeleted], [DeletedAt])
    VALUES
    (N'WELCOME10', 10, DATEADD(MONTH, 3, GETDATE()), 100, 0, 1, 0, NULL),
    (N'CAFE20', 20, DATEADD(MONTH, 1, GETDATE()), 50, 0, 1, 0, NULL);
END;

COMMIT TRANSACTION;
