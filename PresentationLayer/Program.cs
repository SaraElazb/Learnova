using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Manager.CategoryManager;
using BusinessLogicLayer.Manager.CourseManager;
using BusinessLogicLayer.Manager.LessonManager;
using BusinessLogicLayer.Manager.OrderManager;
using BusinessLogicLayer.Manager.QuestionManager;
using BusinessLogicLayer.Manager.QuizManager;
using BusinessLogicLayer.Services.AccountServices;
using BusinessLogicLayer.Services.RoleServices;
using BusinessLogicLayer.Services.UserRoleServices;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ICourseManager, CourseManager>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();
            builder.Services.AddScoped<ILessonManager, LessonManager>();
            builder.Services.AddScoped<IQuizManager, QuizManager>();
            builder.Services.AddScoped<IQuestionManager, QuestionManager>();
            //builder.Services.AddScoped<IOrderManager, OrderManager>();
            builder.Services.AddScoped<ICategoryManager, CategoryManager>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddDbContext<ELearningDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSession();
            #region IdentutyAuth 

            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ELearningDbContext>();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.LoginPath = "/account/login";
                options.AccessDeniedPath = "/account/notauthorized";

            });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthorization();
            app.UseSession();

            // Seed roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                
                // Define roles
                string[] roleNames = { "Admin", "Teacher", "Student" };
                
                foreach (var roleName in roleNames)
                {
                    // Check if role already exists
                    var roleExists = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExists)
                    {
                        // Create role if it doesn't exist
                        await roleManager.CreateAsync(new Role { Name = roleName });
                    }
                }
                
                // Seed admin user
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string adminEmail = "admin@site.com";
                
                // Check if admin user exists
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    // Create admin user if it doesn't exist
                    var admin = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = adminEmail,
                        Email = adminEmail,
                        First_name = "Admin",
                        Last_name = "User",
                        EmailConfirmed = true
                    };
                    
                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        // Assign admin role
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
