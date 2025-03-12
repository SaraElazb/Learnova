using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Category_ID);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Notification_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Notification_ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Course_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Category_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Course_ID);
                    table.ForeignKey(
                        name: "FK_Courses_Categories_Category_ID",
                        column: x => x.Category_ID,
                        principalTable: "Categories",
                        principalColumn: "Category_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_ID = table.Column<int>(type: "int", nullable: false),
                    First_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Profile_picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Registration_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_Users_Roles_Role_ID",
                        column: x => x.Role_ID,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Lesson_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    VideoUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quiz_ID = table.Column<int>(type: "int", nullable: true),
                    Course_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Lesson_ID);
                    table.ForeignKey(
                        name: "FK_Lessons_Courses_Course_ID",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID");
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Enrollment_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enrollment_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Course_ID = table.Column<int>(type: "int", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Enrollment_ID);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_Course_ID",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID");
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID");
                });

            migrationBuilder.CreateTable(
                name: "Receives",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Notification_ID = table.Column<int>(type: "int", nullable: false),
                    Datetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receives", x => new { x.User_ID, x.Notification_ID, x.Datetime });
                    table.ForeignKey(
                        name: "FK_Receives_Notifications_Notification_ID",
                        column: x => x.Notification_ID,
                        principalTable: "Notifications",
                        principalColumn: "Notification_ID");
                    table.ForeignKey(
                        name: "FK_Receives_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Review_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Course_ID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Review_ID);
                    table.ForeignKey(
                        name: "FK_Reviews_Courses_Course_ID",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID");
                    table.ForeignKey(
                        name: "FK_Reviews_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Quiz_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lesson_ID = table.Column<int>(type: "int", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: true),
                    Passing_score = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Quiz_ID);
                    table.ForeignKey(
                        name: "FK_Quizzes_Lessons_Lesson_ID",
                        column: x => x.Lesson_ID,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_ID");
                    table.ForeignKey(
                        name: "FK_Quizzes_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Lesson_ID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studies", x => new { x.User_ID, x.Lesson_ID });
                    table.ForeignKey(
                        name: "FK_Studies_Lessons_Lesson_ID",
                        column: x => x.Lesson_ID,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Studies_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateID);
                    table.ForeignKey(
                        name: "FK_Certificates_Enrollments_EnrollmentID",
                        column: x => x.EnrollmentID,
                        principalTable: "Enrollments",
                        principalColumn: "Enrollment_ID");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Payment_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enrollment_ID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Payment_method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Transaction_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Payment_ID);
                    table.ForeignKey(
                        name: "FK_Payments_Enrollments_Enrollment_ID",
                        column: x => x.Enrollment_ID,
                        principalTable: "Enrollments",
                        principalColumn: "Enrollment_ID");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizID = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    RightAns = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "Quiz_ID");
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quiz_ID = table.Column<int>(type: "int", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Submissions_Quizzes_Quiz_ID",
                        column: x => x.Quiz_ID,
                        principalTable: "Quizzes",
                        principalColumn: "Quiz_ID");
                    table.ForeignKey(
                        name: "FK_Submissions_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID");
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    AnswerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    Answers = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.AnswerID);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "QuestionID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionID",
                table: "Answers",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_EnrollmentID",
                table: "Certificates",
                column: "EnrollmentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Category_ID",
                table: "Courses",
                column: "Category_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_Course_ID",
                table: "Enrollments",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_User_ID",
                table: "Enrollments",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Course_ID",
                table: "Lessons",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Enrollment_ID",
                table: "Payments",
                column: "Enrollment_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizID",
                table: "Questions",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Lesson_ID",
                table: "Quizzes",
                column: "Lesson_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_User_ID",
                table: "Quizzes",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_Notification_ID",
                table: "Receives",
                column: "Notification_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Course_ID",
                table: "Reviews",
                column: "Course_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_User_ID",
                table: "Reviews",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_Lesson_ID",
                table: "Studies",
                column: "Lesson_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_Quiz_ID",
                table: "Submissions",
                column: "Quiz_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_User_ID",
                table: "Submissions",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role_ID",
                table: "Users",
                column: "Role_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Receives");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Studies");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
