using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Subscriptions.DAL.Entities;

namespace VShop.SharedKernel.Subscriptions.DAL.Configurations
{
    internal class MessageDeadLetterLogEntityTypeConfiguration : IEntityTypeConfiguration<MessageDeadLetterLog>
    {
        public void Configure(EntityTypeBuilder<MessageDeadLetterLog> builder)
        {
            builder.ToTable("message_dead_letter_queue", SubscriptionDbContext.SubscriptionSchema);
            
            builder.HasKey(el => el.Id);
            builder.Property(el => el.StreamId).IsRequired();
            builder.Property(el => el.MessageType).IsRequired();
            builder.Property(el => el.MessageId).IsRequired();
            builder.Property(el => el.MessageData).IsRequired();
            builder.Property(el => el.Status).IsRequired();
            builder.Property(el => el.Error).IsRequired();
            builder.Property(el => el.DateCreated).IsRequired();
            builder.Property(el => el.DateUpdated).IsRequired();
            
            builder.UseXminAsConcurrencyToken();
        }
    }
}