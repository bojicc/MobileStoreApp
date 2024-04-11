using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileStoreApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertiesInPhoneClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Phones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Phones");
        }
    }
}
