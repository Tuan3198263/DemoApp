using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QLNhanVien.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                schema: "public",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "employees",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ma_nhan_vien = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ten_nhan_vien = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    gioi_tinh = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Nam"),
                    bo_phan = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    muc_luong = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_employees_created_at",
                schema: "public",
                table: "employees",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_employees_gioi_tinh",
                schema: "public",
                table: "employees",
                column: "gioi_tinh");

            migrationBuilder.CreateIndex(
                name: "idx_employees_ma_nhan_vien_unique",
                schema: "public",
                table: "employees",
                column: "ma_nhan_vien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_employees_updated_by",
                schema: "public",
                table: "employees",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employees",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "users");
        }
    }
}
