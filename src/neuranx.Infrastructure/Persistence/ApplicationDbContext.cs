using Microsoft.EntityFrameworkCore;
using neuranx.Domain.Entities;

namespace neuranx.Infrastructure.Persistence;

using neuranx.Application.Interfaces;
using neuranx.Infrastructure.Persistence.Extensions;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<ApplicationUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyAllConfiguration();
        base.OnModelCreating(modelBuilder);
    }
}
