using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using System.Linq;

namespace AppServer
{
    public class AppContext: DbContext
    {
       
        public DbSet<AssetItem> AssetItems { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instance> Instances { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<PhysicalDetail> PhysicalDetails { get; set; }
        public DbSet<ExpectedDetail> ExpectedDetails { get; set; }
        public AppContext(DbContextOptions<AppContext> options): base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<PhysicalDetail>()
                .HasOne(detail => detail.InstanceNavigation)
                .WithMany()
                .HasForeignKey(detail => detail.InstanceId);
            modelBuilder.Entity<ExpectedDetail>()
              .HasOne(detail => detail.AssetItemNavigation)
              .WithMany()
              .HasForeignKey(detail => detail.AssetItemId);
            modelBuilder.Entity<Instance>()
                .HasOne(instance => instance.AssetItemNavigation)
                .WithMany()
                .HasForeignKey(instance => instance.AssetItemId);
            modelBuilder.Entity<Instance>()
               .HasOne<Department>()
               .WithMany()
               .HasForeignKey(instance => instance.DepartmentId);

        }
    }
}
