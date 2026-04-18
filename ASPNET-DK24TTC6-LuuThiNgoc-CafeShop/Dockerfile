FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ASPNET-DK24TTC6-LuuThiNgoc-CafeShop/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.csproj", "ASPNET-DK24TTC6-LuuThiNgoc-CafeShop/"]
RUN dotnet restore "ASPNET-DK24TTC6-LuuThiNgoc-CafeShop/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.csproj"
COPY . .
WORKDIR "/src/ASPNET-DK24TTC6-LuuThiNgoc-CafeShop"
RUN dotnet build "./ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ASPNET-DK24TTC6-LuuThiNgoc-CafeShop.dll"]
