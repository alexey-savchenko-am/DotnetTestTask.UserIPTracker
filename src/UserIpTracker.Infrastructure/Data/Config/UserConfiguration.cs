using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net;
using UserIpTracker.Domain;

namespace UserIpTracker.Infrastructure.Data.Config;

internal sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.OwnsMany(u => u.Connections, connBuilder =>
        {
            connBuilder.ToTable("user_connections");
            connBuilder.WithOwner().HasForeignKey("user_id");

            connBuilder.Property(c => c.Ip)
                .HasConversion(
                    ip => ip.ToString(), 
                    ipStr => IPAddress.Parse(ipStr))
                .HasColumnName("ip")
                .IsRequired();

            connBuilder.Property(c => c.LastSeen)
                .HasColumnName("last_seen")
                .IsRequired();
        });

        builder.Navigation(u => u.Connections)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
