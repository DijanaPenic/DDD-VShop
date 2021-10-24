FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Sales/Sales.API/Sales.API.csproj", "Sales.API/"]
RUN dotnet restore "Services/Sales/Sales.API/Sales.API.csproj"
COPY . .
WORKDIR "/src/Sales.API"
RUN dotnet build "Sales.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sales.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sales.API.dll"]
