using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ELearningDbContext>
{
    public ELearningDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ELearningDbContext>();
        optionsBuilder.UseSqlServer("Server=DESKTOP-V4IS8JF\\SQLEXPRESS;Database=ELearningDB_New;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
        return new ELearningDbContext(optionsBuilder.Options);
    }
}