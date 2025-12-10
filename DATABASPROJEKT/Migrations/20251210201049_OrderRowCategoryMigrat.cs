using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATABASPROJEKT.Migrations
{
    /// <inheritdoc />
    public partial class OrderRowCategoryMigrat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRows_Categories_CategorieId",
                table: "OrderRows");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "OrderRows");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRows_Categories_CategorieId",
                table: "OrderRows",
                column: "CategorieId",
                principalTable: "Categories",
                principalColumn: "CategorieId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRows_Categories_CategorieId",
                table: "OrderRows");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "OrderRows",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRows_Categories_CategorieId",
                table: "OrderRows",
                column: "CategorieId",
                principalTable: "Categories",
                principalColumn: "CategorieId");
        }
    }
}
