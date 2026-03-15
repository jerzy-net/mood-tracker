using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodTracker.Domain.Entities;

namespace MoodTracker.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();

        builder.Property(user => user.Email)
            .HasMaxLength(256);

        builder.Property(user => user.CreatedAtUtc)
            .IsRequired();

        builder.Property(user => user.UpdatedAtUtc)
            .IsRequired(false);

        builder
            .HasMany(user => user.MoodEntries)
            .WithOne(entry => entry.User)
            .HasForeignKey(entry => entry.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
