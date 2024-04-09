using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileStoreApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPhoneClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Phones",
                newName: "Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Phones",
                newName: "MyProperty");
        }
    }
}
