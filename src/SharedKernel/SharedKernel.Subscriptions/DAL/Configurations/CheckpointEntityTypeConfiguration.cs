using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VShop.SharedKernel.Subscriptions.DAL.Entities;

namespace VShop.SharedKernel.Subscriptions.DAL.Configurations;

internal class CheckpointEntityTypeConfiguration : IEntityTypeConfiguration<Checkpoint>
{
    public void Configure(EntityTypeBuilder<Checkpoint> builder)
    {
        builder.ToTable("checkpoint", SubscriptionDbContext.SubscriptionSchema);
            
        builder.HasKey(c => c.SubscriptionId);
        builder.Property(c => c.Position);
        builder.Property(c => c.DateCreated).IsRequired();
        builder.Property(c => c.DateUpdated).IsRequired();
            
        builder.UseXminAsConcurrencyToken();
    }
}