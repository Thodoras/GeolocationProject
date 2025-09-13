namespace GeolocationAPI.Infrastructure.DB.MSSQLExpress.Models
{
    using Microsoft.EntityFrameworkCore;

    public class GeolocationAPIDbContext : DbContext
    {
        public DbSet<BatchProcessModel> BatchProcesses { get; set; }
        public DbSet<BatchProcessItemModel> BatchProcessItems { get; set; }

        public GeolocationAPIDbContext(DbContextOptions<GeolocationAPIDbContext> options) : base(options)
        {
        }
    }
}