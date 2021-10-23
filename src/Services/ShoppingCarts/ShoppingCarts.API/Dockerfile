FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Basket/ShoppingCarts.API/ShoppingCarts.API.csproj", "ShoppingCarts.API/"]
RUN dotnet restore "Services/Basket/ShoppingCarts.API/ShoppingCarts.API.csproj"
COPY . .
WORKDIR "/src/ShoppingCarts.API"
RUN dotnet build "ShoppingCarts.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShoppingCarts.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCarts.API.dll"]
