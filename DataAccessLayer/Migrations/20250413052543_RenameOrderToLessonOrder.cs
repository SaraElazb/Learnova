using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderToLessonOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Order",
                table: "Lessons",
                newName: "LessonOrder");

            migrationBuilder.AddColumn<string>(
                name: "Course_Name",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course_Name",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "LessonOrder",
                table: "Lessons",
                newName: "Order");
        }
    }
}
