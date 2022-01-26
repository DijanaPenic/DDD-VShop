using AutoMapper;

using VShop.Modules.Billing.API.Models;
using VShop.Modules.Billing.Infrastructure.Commands;

namespace VShop.Modules.Billing.API.Automapper
{
    internal class PaymentAutomapperProfile : Profile
    {
        public PaymentAutomapperProfile()
        {
            CreateMap<TransferRequest, TransferCommand>();
        }
    }
}