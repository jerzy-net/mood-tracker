using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodTracker.Domain.Entities;
using MoodTracker.Domain.ValueObjects;

namespace MoodTracker.Infrastructure.Persistence.Configurations;

public class MoodEntryConfiguration : IEntityTypeConfiguration<MoodEntry>
{
    public void Configure(EntityTypeBuilder<MoodEntry> builder)
    {
        builder.ToTable("MoodEntries");

        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id).ValueGeneratedNever();

        builder.Property(entry => entry.UserId).IsRequired(false);

        builder.Property(entry => entry.MoodScore)
            .HasConversion(
                score => score.Value,
                value => MoodScore.Create(value))
            .IsRequired();

        builder.Property(entry => entry.RecordedAt)
            .HasConversion(
                recordedAt => recordedAt.Value,
                value => RecordedAt.Create(value))
            .HasColumnName("RecordedAtUtc")
            .IsRequired();

        builder.Property(entry => entry.CreatedAtUtc)
            .IsRequired();

        builder.Property(entry => entry.UpdatedAtUtc)
            .IsRequired(false);

        builder
            .HasIndex(entry => new { entry.UserId, entry.RecordedAt })
            .IsUnique()
            .HasFilter("\"UserId\" IS NOT NULL");
    }
}
