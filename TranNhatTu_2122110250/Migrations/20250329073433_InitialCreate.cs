using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranNhatTu_2122110250.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.CreateTable(
		name: "Products",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false)
				.Annotation("SqlServer:Identity", "1, 1"),
			Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Price = table.Column<double>(type: "float", nullable: false),

			// Thêm các cột liên quan đến tracking
			CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
			CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
			UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
			UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
			DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
			DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_Products", x => x.Id);
		});
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
