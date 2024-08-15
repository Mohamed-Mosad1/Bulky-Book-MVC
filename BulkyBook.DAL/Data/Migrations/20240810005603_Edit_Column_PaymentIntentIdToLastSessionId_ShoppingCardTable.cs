using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class Edit_Column_PaymentIntentIdToLastSessionId_ShoppingCardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "ShoppingCarts",
                newName: "LastSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastSessionId",
                table: "ShoppingCarts",
                newName: "PaymentIntentId");
        }
    }
}
