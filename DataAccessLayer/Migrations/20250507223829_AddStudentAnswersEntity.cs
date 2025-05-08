using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAnswersEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Submission_ID = table.Column<int>(type: "int", nullable: false),
                    Question_ID = table.Column<int>(type: "int", nullable: false),
                    Answer_ID = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Answers_Answer_ID",
                        column: x => x.Answer_ID,
                        principalTable: "Answers",
                        principalColumn: "AnswerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Questions_Question_ID",
                        column: x => x.Question_ID,
                        principalTable: "Questions",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Submissions_Submission_ID",
                        column: x => x.Submission_ID,
                        principalTable: "Submissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_Answer_ID",
                table: "StudentAnswers",
                column: "Answer_ID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_Question_ID",
                table: "StudentAnswers",
                column: "Question_ID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_Submission_ID",
                table: "StudentAnswers",
                column: "Submission_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAnswers");
        }
    }
}
