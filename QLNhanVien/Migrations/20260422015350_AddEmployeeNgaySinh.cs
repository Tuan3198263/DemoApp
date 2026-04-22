using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNhanVien.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeNgaySinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ngay_sinh",
                schema: "public",
                table: "employees",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ngay_sinh",
                schema: "public",
                table: "employees");
        }
    }
}
