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
    INSERT INTO [Categories] ([Name], [Description], [CreatedAt])
    VALUES
    (N'Cà phê', N'Các loại cà phê truyền thống và hiện đại', GETDATE()),
    (N'Trà', N'Trà các loại, trà sữa, trà trái cây', GETDATE()),
    (N'Nước ép', N'Nước ép trái cây tươi', GETDATE()),
    (N'Sinh tố', N'Sinh tố các loại trái cây', GETDATE()),
    (N'Đá xay', N'Đồ uống đá xay mát lạnh', GETDATE());
END;

DECLARE @caPheCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Cà phê');
DECLARE @traCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Trà');
DECLARE @nuocEpCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Nước ép');
DECLARE @sinhToCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Sinh tố');
DECLARE @daXayCatId int = (SELECT TOP 1 [Id] FROM [Categories] WHERE [Name] = N'Đá xay');

IF NOT EXISTS (SELECT 1 FROM [Products])
BEGIN
    INSERT INTO [Products] ([Name], [Description], [Price], [ImageUrl], [Stock], [CategoryId], [CreatedAt], [IsActive])
    VALUES
    (N'Cà phê đen đá', N'Cà phê đen truyền thống pha phin, đậm đà hương vị Việt Nam', 25000, N'/uploads/products/ca-phe-den-da.jpg', 100, @caPheCatId, GETDATE(), 1),
    (N'Cà phê sữa đá', N'Cà phê sữa đá thơm ngon, béo ngậy', 29000, N'/uploads/products/ca-phe-sua-da.jpg', 100, @caPheCatId, GETDATE(), 1),
    (N'Bạc xỉu', N'Cà phê sữa nhẹ nhàng, phù hợp cho người mới bắt đầu', 29000, N'/uploads/products/bac-xiu.jpg', 100, @caPheCatId, GETDATE(), 1),
    (N'Cappuccino', N'Cappuccino thơm ngon theo phong cách Ý', 45000, N'/uploads/products/cappuccino.jpg', 100, @caPheCatId, GETDATE(), 1),
    (N'Latte', N'Latte mềm mại với lớp foam mịn', 45000, N'/uploads/products/latte.jpg', 100, @caPheCatId, GETDATE(), 1),
    (N'Americano', N'Cà phê Americano đậm đà, thanh nhẹ', 39000, N'/uploads/products/americano.jpg', 100, @caPheCatId, GETDATE(), 1),
    (N'Trà đào cam sả', N'Trà đào kết hợp cam và sả thơm mát', 35000, N'/uploads/products/tra-dao-cam-sa.jpg', 100, @traCatId, GETDATE(), 1),
    (N'Trà sữa trân châu', N'Trà sữa truyền thống với trân châu dẻo mềm', 35000, N'/uploads/products/tra-sua-tran-chau.jpg', 100, @traCatId, GETDATE(), 1),
    (N'Trà vải', N'Trà vải thơm ngát, ngọt tự nhiên', 32000, N'/uploads/products/tra-vai.jpg', 100, @traCatId, GETDATE(), 1),
    (N'Trà matcha latte', N'Trà xanh matcha kết hợp sữa tươi', 45000, N'/uploads/products/matcha-latte.jpg', 100, @traCatId, GETDATE(), 1),
    (N'Nước ép cam', N'Nước ép cam tươi nguyên chất, giàu vitamin C', 30000, N'/uploads/products/nuoc-ep-cam-v2.jpg', 80, @nuocEpCatId, GETDATE(), 1),
    (N'Nước ép dưa hấu', N'Nước ép dưa hấu mát lạnh giải nhiệt', 28000, N'/uploads/products/nuoc-ep-dua-hau.jpg', 80, @nuocEpCatId, GETDATE(), 1),
    (N'Sinh tố bơ', N'Sinh tố bơ béo ngậy, bổ dưỡng', 35000, N'/uploads/products/sinh-to-bo-v2.jpg', 80, @sinhToCatId, GETDATE(), 1),
    (N'Sinh tố xoài', N'Sinh tố xoài ngọt tự nhiên', 32000, N'/uploads/products/sinh-to-xoai-v2.jpg', 80, @sinhToCatId, GETDATE(), 1),
    (N'Sinh tố dâu', N'Sinh tố dâu tây tươi mát', 35000, N'/uploads/products/sinh-to-dau-v2.jpg', 80, @sinhToCatId, GETDATE(), 1),
    (N'Chocolate đá xay', N'Chocolate đá xay mát lạnh, ngọt ngào', 45000, N'/uploads/products/choco-da-xay-v2.jpg', 80, @daXayCatId, GETDATE(), 1),
    (N'Matcha đá xay', N'Matcha đá xay Nhật Bản chính hiệu', 49000, N'/uploads/products/matcha-da-xay-v2.jpg', 80, @daXayCatId, GETDATE(), 1);
END;

UPDATE p
SET p.[ImageUrl] = v.[ImageUrl]
FROM [Products] AS p
INNER JOIN
(
    VALUES
    (N'Cà phê đen đá', N'/uploads/products/ca-phe-den-da.jpg'),
    (N'Cà phê sữa đá', N'/uploads/products/ca-phe-sua-da.jpg'),
    (N'Bạc xỉu', N'/uploads/products/bac-xiu.jpg'),
    (N'Cappuccino', N'/uploads/products/cappuccino.jpg'),
    (N'Latte', N'/uploads/products/latte.jpg'),
    (N'Americano', N'/uploads/products/americano.jpg'),
    (N'Trà đào cam sả', N'/uploads/products/tra-dao-cam-sa.jpg'),
    (N'Trà sữa trân châu', N'/uploads/products/tra-sua-tran-chau.jpg'),
    (N'Trà vải', N'/uploads/products/tra-vai.jpg'),
    (N'Trà matcha latte', N'/uploads/products/matcha-latte.jpg'),
    (N'Nước ép cam', N'/uploads/products/nuoc-ep-cam-v2.jpg'),
    (N'Nước ép dưa hấu', N'/uploads/products/nuoc-ep-dua-hau.jpg'),
    (N'Sinh tố bơ', N'/uploads/products/sinh-to-bo-v2.jpg'),
    (N'Sinh tố xoài', N'/uploads/products/sinh-to-xoai-v2.jpg'),
    (N'Sinh tố dâu', N'/uploads/products/sinh-to-dau-v2.jpg'),
    (N'Chocolate đá xay', N'/uploads/products/choco-da-xay-v2.jpg'),
    (N'Matcha đá xay', N'/uploads/products/matcha-da-xay-v2.jpg')
) AS v([Name], [ImageUrl])
    ON p.[Name] = v.[Name];

IF NOT EXISTS (SELECT 1 FROM [Coupons])
BEGIN
    INSERT INTO [Coupons] ([Code], [DiscountPercent], [ExpiryDate], [MaxUsage], [CurrentUsage], [IsActive])
    VALUES
    (N'WELCOME10', 10, DATEADD(MONTH, 3, GETDATE()), 100, 0, 1),
    (N'CAFE20', 20, DATEADD(MONTH, 1, GETDATE()), 50, 0, 1);
END;

COMMIT TRANSACTION;
