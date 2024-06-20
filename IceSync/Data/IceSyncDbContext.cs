using IceSync.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IceSync.Data
{
    public class IceSyncDbContext: DbContext
    {
        public IceSyncDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Workflow> Workflows { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(IceSyncDbContext).Assembly);

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
        }
    }
}