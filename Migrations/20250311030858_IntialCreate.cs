using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Grad_Project.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Role table
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

            // Create Category table
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Category_ID);
                });

            // Create Notification table
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Notification_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Notification_ID);
                });

            // Create User table
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
                    Profile_picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
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

            // Create unique index for User.Email
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            // Create index for User.Role_ID
            migrationBuilder.CreateIndex(
                name: "IX_Users_Role_ID",
                table: "Users",
                column: "Role_ID");

            // Create Course table
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Course_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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

            // Create index for Course.Category_ID
            migrationBuilder.CreateIndex(
                name: "IX_Courses_Category_ID",
                table: "Courses",
                column: "Category_ID");

            // Create Lesson table
            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Lesson_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    VideoUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index for Lesson.Course_ID
            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Course_ID",
                table: "Lessons",
                column: "Course_ID");

            // Create Enrollment table
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
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index for Enrollment.Course_ID
            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_Course_ID",
                table: "Enrollments",
                column: "Course_ID");

            // Create index for Enrollment.User_ID
            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_User_ID",
                table: "Enrollments",
                column: "User_ID");

            // Create Quiz table
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
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Quiz_ID);
                    table.ForeignKey(
                        name: "FK_Quizzes_Lessons_Lesson_ID",
                        column: x => x.Lesson_ID,
                        principalTable: "Lessons",
                        principalColumn: "Lesson_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quizzes_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            // Create index for Quiz.Lesson_ID (One-to-One relationship)
            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Lesson_ID",
                table: "Quizzes",
                column: "Lesson_ID",
                unique: true);

            // Create index for Quiz.User_ID
            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_User_ID",
                table: "Quizzes",
                column: "User_ID");

            // Update Lesson.Quiz_ID foreign key now that Quiz table exists
            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Quizzes_Quiz_ID",
                table: "Lessons",
                column: "Quiz_ID",
                principalTable: "Quizzes",
                principalColumn: "Quiz_ID");

            // Create index for Lesson.Quiz_ID (One-to-One relationship)
            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Quiz_ID",
                table: "Lessons",
                column: "Quiz_ID",
                unique: true,
                filter: "[Quiz_ID] IS NOT NULL");

            // Create Review table
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Review_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Course_ID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Review_ID);
                    table.ForeignKey(
                        name: "FK_Reviews_Courses_Course_ID",
                        column: x => x.Course_ID,
                        principalTable: "Courses",
                        principalColumn: "Course_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create index for Review.Course_ID
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Course_ID",
                table: "Reviews",
                column: "Course_ID");

            // Create index for Review.User_ID
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_User_ID",
                table: "Reviews",
                column: "User_ID");

            // Create Studies table (Many-to-Many between User and Lesson)
            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Lesson_ID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
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

            // Create index for Studies.Lesson_ID
            migrationBuilder.CreateIndex(
                name: "IX_Studies_Lesson_ID",
                table: "Studies",
                column: "Lesson_ID");

            // Create Receive table (Many-to-Many between User and Notification with DateTime)
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
                        principalColumn: "Notification_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Receives_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index for Receive.Notification_ID
            migrationBuilder.CreateIndex(
                name: "IX_Receives_Notification_ID",
                table: "Receives",
                column: "Notification_ID");

            // Create Certificate table
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
                        principalColumn: "Enrollment_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create unique index for Certificate.EnrollmentID (One-to-One relationship)
            migrationBuilder.CreateIndex(
                name: "IX_Certificates_EnrollmentID",
                table: "Certificates",
                column: "EnrollmentID",
                unique: true);

            // Create Payment table
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Payment_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enrollment_ID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Payment_method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Transaction_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Payment_ID);
                    table.ForeignKey(
                        name: "FK_Payments_Enrollments_Enrollment_ID",
                        column: x => x.Enrollment_ID,
                        principalTable: "Enrollments",
                        principalColumn: "Enrollment_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create unique index for Payment.Enrollment_ID (One-to-One relationship)
            migrationBuilder.CreateIndex(
                name: "IX_Payments_Enrollment_ID",
                table: "Payments",
                column: "Enrollment_ID",
                unique: true);

            // Create Question table
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizID = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    RightAns = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "Quiz_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index for Question.QuizID
            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizID",
                table: "Questions",
                column: "QuizID");

            // Create Submission table
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
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Submissions_Quizzes_Quiz_ID",
                        column: x => x.Quiz_ID,
                        principalTable: "Quizzes",
                        principalColumn: "Quiz_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index for Submission.Quiz_ID
            migrationBuilder.CreateIndex(
                name: "IX_Submissions_Quiz_ID",
                table: "Submissions",
                column: "Quiz_ID");

            // Create index for Submission.User_ID
            migrationBuilder.CreateIndex(
                name: "IX_Submissions_User_ID",
                table: "Submissions",
                column: "User_ID");

            // Create Answer table
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
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create index for Answer.QuestionID
            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionID",
                table: "Answers",
                column: "QuestionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop tables in reverse order
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
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}