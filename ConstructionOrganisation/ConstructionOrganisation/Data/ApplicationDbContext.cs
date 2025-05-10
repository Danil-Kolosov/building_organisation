using ConstructionOrganisation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConstructionOrganisation.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {              
}
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseMySql(
            "Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;",
            ServerVersion.AutoDetect("Server=localhost;Database=building_organisation;User=root;Password=Ammon Dgerro;")
        );
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Management> Managements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //base.OnModelCreating(builder);

        //builder.Entity<Management>()
        //    .HasOne(u => u.ManagementNumber)
        //    .WithMany()
        //    .HasForeignKey(u => u.ManagementNumber);
    }
}
