# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copia as csprojs primeiro pra cache inteligente
COPY StoreService.Domain/StoreService.Domain.csproj StoreService.Domain/
COPY StoreService.Application/StoreService.Application.csproj StoreService.Application/
COPY StoreService.Infrastructure/StoreService.Infrastructure.csproj StoreService.Infrastructure/
COPY SportsStoreService/SportsStoreService.csproj SportsStoreService/
RUN dotnet restore SportsStoreService/SportsStoreService.csproj

# copia o restante
COPY StoreService.Domain/ StoreService.Domain/
COPY StoreService.Application/ StoreService.Application/
COPY StoreService.Infrastructure/ StoreService.Infrastructure/
COPY SportsStoreService/ SportsStoreService/

RUN dotnet publish SportsStoreService/SportsStoreService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# porta interna
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "SportsStoreService.dll"]