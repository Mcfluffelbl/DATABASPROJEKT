using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATABASPROJEKT.Migrations
{
    /// <inheritdoc />
    public partial class OrderCategoryMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategorieId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CategorieId",
                table: "Orders",
                column: "CategorieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Categories_CategorieId",
                table: "Orders",
                column: "CategorieId",
                principalTable: "Categories",
                principalColumn: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Categories_CategorieId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CategorieId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CategorieId",
                table: "Orders");
        }
    }
}
