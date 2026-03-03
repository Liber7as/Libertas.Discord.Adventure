using Libertas.Discord.Adventure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libertas.Discord.Adventure.Data.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="MobPreset"/> entity.
/// </summary>
public partial class MobPresetConfiguration : IEntityTypeConfiguration<MobPreset>
{
    /// <summary>
    /// Configure entity mapping and seed data.
    /// </summary>
    public void Configure(EntityTypeBuilder<MobPreset> builder)
    {
        builder.HasKey(e => e.Id);

        HasData(builder);
    }
}
