using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAVLink.Gateway.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddItemsToList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToDoListItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoListItem_TodoLists_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TodoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListItem_OwnerId",
                table: "ToDoListItem",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDoListItem");
        }
    }
}
