using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace O2.Identity.Web.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=tcp:20.62.232.182,1433;Initial Catalog=O2Bionics.O2Platform.IdentityDb;Persist Security Info=False;User ID=sa;Password=Pass@word;Connection Timeout=30");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}