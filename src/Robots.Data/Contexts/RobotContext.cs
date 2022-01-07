using Microsoft.EntityFrameworkCore;
using Robots.Data.EntityTypeConfigurations;
using Robots.Domain.Entities;

namespace Robots.Data.Contexts
{
  public class RobotContext : DbContext
  {
    public RobotContext(DbContextOptions<RobotContext> options) : base(options)
    {
    }

    public virtual DbSet<Job> Jobs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfiguration(new JobEntityTypeConfiguration());
    }
  }
}
