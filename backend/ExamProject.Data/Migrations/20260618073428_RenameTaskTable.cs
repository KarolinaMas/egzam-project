using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_user_UserId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "task");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_UserId",
                table: "task",
                newName: "IX_task_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_task",
                table: "task",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_task_user_UserId",
                table: "task",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_task_user_UserId",
                table: "task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_task",
                table: "task");

            migrationBuilder.RenameTable(
                name: "task",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_task_UserId",
                table: "Tasks",
                newName: "IX_Tasks_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_user_UserId",
                table: "Tasks",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
