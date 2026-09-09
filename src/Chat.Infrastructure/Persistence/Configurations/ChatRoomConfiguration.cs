using Chat.Domain.Identifiers;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Persistence.Configurations;

internal sealed class ChatRoomConfiguration: IEntityTypeConfiguration<ChatRoom>
{
    public void Configure(EntityTypeBuilder<ChatRoom> builder)
    {
        builder.ToTable("ChatRooms");
        
        // Id
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new RoomId(value));
        
        // Name
        builder.Property(r => r.Name)
            .HasConversion(name => name.Value, value => RoomName.Create(value))
            .HasMaxLength(50)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(r => r.Name).IsUnique();
        
        // OwnerId
        builder.Property(r => r.OwnerId)
            .HasConversion(id => id.Value, value => new UserId(value));
        
        // Members
        builder.OwnsMany(r => r.Members, b =>
        {
            b.ToTable("RoomMembers");
            b.WithOwner().HasForeignKey("RoomId");
            b.HasKey("RoomId", "UserId");
            b.Property(m => m.UserId)
                .HasConversion(id => id.Value, value => new UserId(value))
                .HasColumnName("UserId");
        });
        builder.Navigation(r => r.Members)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Ignore(r => r.DomainEvents);
        
        builder.Property<byte[]>("RowVersion").IsRowVersion();
    }
}