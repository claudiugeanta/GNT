using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GNT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCodeToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Product");
        }
    }
}
