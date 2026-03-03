using Libertas.Discord.Adventure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libertas.Discord.Adventure.Data.Configurations;

/// <summary>
///     EF Core configuration for the Player entity.
/// </summary>
public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    /// <summary>
    ///     Configure the EF mapping for the <see cref="Player" /> entity including defaults, keys and indexes.
    /// </summary>
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        // Primary key is the user ID (not auto-generated)
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Username)
            .IsRequired()
            .HasMaxLength(100);

        #region Skill Levels & XP

        builder.Property(p => p.AttackLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.AttackXp)
            .IsRequired()
            .HasDefaultValue(0L);

        builder.Property(p => p.MagicLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.MagicXp)
            .IsRequired()
            .HasDefaultValue(0L);

        builder.Property(p => p.SpeechLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.SpeechXp)
            .IsRequired()
            .HasDefaultValue(0L);

        builder.Property(p => p.DefenseLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.DefenseXp)
            .IsRequired()
            .HasDefaultValue(0L);

        #endregion

        #region Statistics

        builder.Property(p => p.TotalGold)
            .IsRequired()
            .HasDefaultValue(0.0);

        builder.Property(p => p.TotalKills)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.TotalDeaths)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.HighestDungeonLevel)
            .IsRequired()
            .HasDefaultValue(0);

        #endregion

        #region Timestamps

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.LastActiveAt)
            .IsRequired();

        #endregion

        // Ignore computed properties
        builder.Ignore(p => p.TotalLevel);
        builder.Ignore(p => p.CombatLevel);

        // Indexes for leaderboard queries
        builder.HasIndex(p => p.TotalGold).IsDescending();
        builder.HasIndex(p => p.HighestDungeonLevel).IsDescending();
    }
}