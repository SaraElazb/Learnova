using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Roles
            migrationBuilder.Sql(@"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES 
            (NEWID(), 'Admin', 'ADMIN', NEWID()),
            (NEWID(), 'Teacher', 'TEACHER', NEWID()),
            (NEWID(), 'Student', 'STUDENT', NEWID());
            ");

            // Insert Admin User
            var adminId = Guid.NewGuid().ToString();
            var passwordHash = "AQAAAAIAAYagAAAAECb6yOcUxmeXaUElKUeSxD62ZQ+iYSOfs4hMc0weZwMtFz04vYFIP4RV7D3sK5D+Pw=="; // Admin@123

            migrationBuilder.Sql($@"
                INSERT INTO Users (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, First_name, Last_name, Registration_date, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
                VALUES ('{adminId}', 'admin@site.com', 'ADMIN@SITE.COM', 'admin@site.com', 'ADMIN@SITE.COM', 1, '{passwordHash}', NEWID(), NEWID(), 'Admin', 'User', GETDATE(), 0, 0, 0, 0);
            ");

            // Assign Role to Admin
            migrationBuilder.Sql($@"
                DECLARE @adminRoleId NVARCHAR(450);
                SELECT @adminRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN';
          
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                VALUES ('{adminId}', @adminRoleId);
            ");
        }

    }
}
