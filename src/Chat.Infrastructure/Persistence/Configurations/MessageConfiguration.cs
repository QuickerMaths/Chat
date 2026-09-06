using Chat.Domain.Identifiers;
using Chat.Domain.Messages;
using Chat.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Infrastructure.Persistence.Configurations;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message> 
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
     
        // Id
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasConversion(id => id.Value, value => new MessageId(value));
        
        // Room Id
        builder.Property(m => m.RoomId)
            .HasConversion(id => id.Value, value => new RoomId(value));
        
        // Author Id
        builder.Property(m => m.AuthorId)
            .HasConversion(id => id.Value, value => new UserId(value));
        
        // Body
        builder.Property(m => m.Body)
            .HasConversion(body => body.Value, value => MessageBody.Create(value))
            .HasMaxLength(2000)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasIndex(m => new { m.RoomId, m.SentAtUtc, m.Id })
            .IsUnique();
        
        builder.HasIndex(m => new { m.RoomId, m.ClientMessageId})
            .IsUnique();
        
        builder.Ignore(m => m.DomainEvents);
    }
}