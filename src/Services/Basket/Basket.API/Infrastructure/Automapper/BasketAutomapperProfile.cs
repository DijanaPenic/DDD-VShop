using AutoMapper;

using VShop.Services.Basket.API.Application.Commands;
using VShop.Services.Basket.API.Application.Models;

namespace VShop.Services.Basket.API.Infrastructure.Automapper
{
    public class BasketAutomapperProfile : Profile
    {
        public BasketAutomapperProfile()
        {
            CreateMap<BasketDto, CreateBasketCommand>();
            CreateMap<BasketItemDto, CreateBasketCommand.BasketItem>();
        }
    }
}