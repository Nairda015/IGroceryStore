using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGroceryStore.UserBasket.Core.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "IGroceryStore.Baskets");

            migrationBuilder.CreateTable(
                name: "Baskets",
                schema: "IGroceryStore.Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                schema: "IGroceryStore.Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    BasketId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalSchema: "IGroceryStore.Baskets",
                        principalTable: "Baskets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_BasketId",
                schema: "IGroceryStore.Baskets",
                table: "Product",
                column: "BasketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product",
                schema: "IGroceryStore.Baskets");

            migrationBuilder.DropTable(
                name: "Baskets",
                schema: "IGroceryStore.Baskets");
        }
    }
}
