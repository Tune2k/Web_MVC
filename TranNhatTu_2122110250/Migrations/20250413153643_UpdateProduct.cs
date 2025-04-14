using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranNhatTu_2122110250.Migrations
{
    public partial class UpdateProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "Products",
                newName: "Category_id");

            migrationBuilder.AddColumn<string>(
                name: "Category_name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category_name",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Category_id",
                table: "Products",
                newName: "category_id");
        }
    }
}
