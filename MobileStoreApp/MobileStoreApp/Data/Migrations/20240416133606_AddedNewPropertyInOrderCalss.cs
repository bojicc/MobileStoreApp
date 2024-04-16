using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileStoreApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertyInOrderCalss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Shipped",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shipped",
                table: "Orders");
        }
    }
}
