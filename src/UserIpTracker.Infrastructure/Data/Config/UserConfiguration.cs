using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIpTracker.Domain;

namespace UserIpTracker.Infrastructure.Data.Config;

internal sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(id => id.Key, key => new UserId(key))
            .ValueGeneratedNever()
            .HasColumnName("id")
            .IsRequired();

        builder.OwnsMany(u => u.Connections, connBuilder =>
        {
            connBuilder.ToTable("user_connections");
            connBuilder.WithOwner().HasForeignKey("user_id");
            connBuilder.Property<Guid>("id");
            connBuilder.HasKey("id");

            connBuilder.Property(c => c.Ip)
              .HasColumnName("ip")
              .HasColumnType("inet")
              .IsRequired();

            // Used SP-GiST index on IP-Address
            connBuilder
                .HasIndex(c => c.Ip)
                .HasMethod("spgist");

            connBuilder.Property<DateTime?>("_lastSeenUtc")
              .HasColumnName("last_seen_utc")
              .IsRequired(false);
        });

        builder.OwnsOne(u => u.LastConnection, lastConnBuilder =>
        {
            lastConnBuilder.Property(c => c.Ip)
               .HasColumnName("last_ip")
               .HasColumnType("inet")
               .IsRequired();

            lastConnBuilder.Property<DateTime?>("_lastSeenUtc")
             .HasColumnName("last_seen_utc")
             .IsRequired(false);
        });
    }
}
