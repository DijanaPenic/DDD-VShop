FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app
COPY . .

ENTRYPOINT ["dotnet", "VShop.Bootstrapper.dll"]