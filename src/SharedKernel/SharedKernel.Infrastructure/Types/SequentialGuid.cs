using System;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace VShop.SharedKernel.Infrastructure.Types
{
    public static class SequentialGuid
    {
        private static SequentialGuidValueGenerator _sequentialGuidGenerator;

        private static SequentialGuidValueGenerator SequentialGuidGenerator
        {
            get
            {
                _sequentialGuidGenerator ??= new SequentialGuidValueGenerator();
                return _sequentialGuidGenerator;
            }
        }

        public static Guid Create() => SequentialGuidGenerator.Next(null!);

        public static bool IsNullOrEmpty(object value)
            => ((value as Guid?).GetValueOrDefault() == Guid.Empty);
    }
}