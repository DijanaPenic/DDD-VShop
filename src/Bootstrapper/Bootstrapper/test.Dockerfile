FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY . .

ENTRYPOINT ["dotnet", "Bootstrapper.dll"]