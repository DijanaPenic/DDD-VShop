using System;

namespace VShop.SharedKernel.Tests.IntegrationTests.Probing
{
    public class AssertErrorException : Exception
    {
        public AssertErrorException(string message) : base(message) { }
    }
}