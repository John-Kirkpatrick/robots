using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Robots.Domain.Entities;

namespace Robots.Data.EntityTypeConfigurations
{
  internal class JobEntityTypeConfiguration : IEntityTypeConfiguration<Job>
  {
    public void Configure(EntityTypeBuilder<Job> builder)
    {
      builder.ToTable("Jobs");

      builder.HasKey(a => a.Id);

      builder.Property(e => e.Id)
             .HasColumnName("JobId")
             .ValueGeneratedNever()
             .IsRequired();

      builder.Property(e => e.PayloadId)
             .HasColumnType("integer")
             .IsRequired();


      builder.Property(e => e.RobotId)
             .HasColumnType("integer")
             .IsRequired();

      builder.Property(e => e.DistanceToGoal)
             .HasColumnType("decimal")
             .IsRequired();

      builder.Property(e => e.BatteryLevel)
             .HasColumnType("integer")
             .IsRequired();

      builder.Property(e => e.Created)
             .HasDefaultValueSql("NOW()")
             .IsRequired();

      builder.HasIndex(e => e.Created);
    }
  }
}
