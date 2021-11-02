using System;

namespace VShop.SharedKernel.EventSourcing
{
    public record MessageMetadata
    {
        public DateTime EffectiveTime { get; init; }
    }
}