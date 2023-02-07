using GeliosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GeliosAPI.EntityFramework
{
    public class AppDbContext: DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        public DbSet<CarModel> Cars { get; set; }
    }
}
