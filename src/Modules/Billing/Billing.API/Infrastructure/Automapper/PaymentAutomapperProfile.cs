using AutoMapper;

using VShop.Modules.Billing.API.Models;
using VShop.Modules.Billing.API.Application.Commands;

namespace VShop.Modules.Billing.API.Infrastructure.Automapper
{
    public class PaymentAutomapperProfile : Profile
    {
        public PaymentAutomapperProfile()
        {
            CreateMap<InitiatePaymentRequest, InitiateTransferCommand>();
        }
    }
}