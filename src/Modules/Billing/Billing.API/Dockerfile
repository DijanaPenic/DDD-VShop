FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Modules/Billing/Billing.API/Billing.API.csproj", "Billing.API/"]
RUN dotnet restore "Modules/Billing/Billing.API/Billing.API.csproj"
COPY . .
WORKDIR "/src/Billing.API"
RUN dotnet build "Billing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Billing.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Billing.API.dll"]
