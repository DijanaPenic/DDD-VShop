﻿using AutoMapper;
using Google.Type;
using VShop.Modules.Sales.API.Models;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.API.Infrastructure.Automapper
{
    public class ShoppingCartAutomapperProfile : Profile
    {
        public ShoppingCartAutomapperProfile()
        {
            CreateMap<CreateShoppingCartRequest, CreateShoppingCartCommand>();
            CreateMap<CreateShoppingCartRequest.ShoppingCartItemRequest, ShoppingCartItemCommand>();
            CreateMap<AddShoppingCartProductRequest, ShoppingCartItemCommand>();
            CreateMap<RemoveShoppingCartProductRequest, RemoveShoppingCartProductCommand>();
            CreateMap<SetContactInformationRequest, SetContactInformationCommand>();
            CreateMap<SetDeliveryAddressRequest, SetDeliveryAddressCommand>();
            CreateMap<decimal, Money>().ConvertUsing(src => src.ToMoney());
        }
    }
}