﻿using AutoMapper;

using VShop.Services.Basket.API.Models;
using VShop.Services.Basket.API.Application.Commands;
using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Infrastructure.Automapper
{
    public class BasketAutomapperProfile : Profile
    {
        public BasketAutomapperProfile()
        {
            CreateMap<CreateBasketRequest, CreateBasketCommand>();
            CreateMap<AddBasketProductRequest, BasketItemDto>();
        }
    }
}