using Microsoft.EntityFrameworkCore;
using OnlineTicketingSystemCore.Models;

namespace OnlineTicketingSystemCore.Data
{
    public class OnlineTicketingSystemCoreContext : DbContext
    {
        public OnlineTicketingSystemCoreContext(DbContextOptions<OnlineTicketingSystemCoreContext> options)
            : base(options)
        { }        

        public DbSet<Project> Projects { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Ticket> Tickets { get; set; }       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasMany(c => c.Tickets).WithOne(e => e.Employee).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Department>().HasMany(c => c.Tickets).WithOne(e => e.Department).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Project>().HasMany(c => c.Tickets).WithOne(e => e.Project).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
