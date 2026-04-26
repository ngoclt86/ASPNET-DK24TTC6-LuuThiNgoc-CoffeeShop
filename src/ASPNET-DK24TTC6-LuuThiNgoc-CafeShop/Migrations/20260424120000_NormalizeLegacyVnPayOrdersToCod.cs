using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeLegacyVnPayOrdersToCod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [Orders] SET [PaymentMethod] = 0 WHERE [PaymentMethod] = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration; reverting would incorrectly mark COD orders as VNPAY.
        }
    }
}
