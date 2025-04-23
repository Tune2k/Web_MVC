using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranNhatTu_2122110250.Migrations
{
    public partial class AddUsernameToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
