using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNhanVien.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedByToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "users",
                newName: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_by",
                schema: "public",
                table: "users",
                newName: "UpdatedBy");
        }
    }
}
