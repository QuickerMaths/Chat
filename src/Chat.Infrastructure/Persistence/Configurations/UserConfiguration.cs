using Chat.Domain.Identifiers;
using Chat.Domain.Users;
using Chat.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        // Id
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasConversion(id => id.Value, value => new UserId(value));

        // UserName
        builder.Property(u => u.UserName)
            .HasConversion(name => name.Value, value => UserName.Create(value))
            .HasMaxLength(32)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(u => u.UserName).IsUnique();
        
        // PasswordHash
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(256)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Ignore(u => u.DomainEvents);
    }
}