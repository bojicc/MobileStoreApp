using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileStoreApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertiesInPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperationSystem",
                table: "Phones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationSystem",
                table: "Phones");
        }
    }
}
