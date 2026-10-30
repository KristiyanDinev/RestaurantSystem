# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["RestaurantSystem.csproj", "."]
RUN dotnet restore "RestaurantSystem.csproj"

COPY . .
RUN dotnet publish "RestaurantSystem.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    --property UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:7278

EXPOSE 7278

COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/assets/images/user /app/wwwroot/assets/images/dishes \
    && chown -R $APP_UID:$APP_UID /app

USER $APP_UID
ENTRYPOINT ["dotnet", "RestaurantSystem.dll"]
