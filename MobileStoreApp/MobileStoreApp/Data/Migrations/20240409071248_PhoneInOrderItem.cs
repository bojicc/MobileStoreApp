using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileStoreApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class PhoneInOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_PhoneId",
                table: "OrderItems",
                column: "PhoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Phones_PhoneId",
                table: "OrderItems",
                column: "PhoneId",
                principalTable: "Phones",
                principalColumn: "PhoneId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Phones_PhoneId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_PhoneId",
                table: "OrderItems");
        }
    }
}
