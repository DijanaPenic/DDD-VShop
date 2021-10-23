using System;

namespace VShop.SharedKernel.EventSourcing
{
    public record EventMetadata
    {
        public DateTime EffectiveTime { get; init; }
    }
}