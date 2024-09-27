
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["F-Driver.API/F-Driver.API.csproj", "F-Driver.API/"]
COPY ["F-Driver.Service/F-Driver.Service.csproj", "F-Driver.Service/"]
COPY ["F-Driver.Repository/F-Driver.Repository.csproj", "F-Driver.Repository/"]
COPY ["F-Driver.DataAccessObject/F-Driver.DataAccessObject.csproj", "F-Driver.DataAccessObject/"]
RUN dotnet restore "./F-Driver.API/F-Driver.API.csproj"
COPY . .
WORKDIR "/src/F-Driver.API"
RUN dotnet build "./F-Driver.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS public
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./F-Driver.API.csproj" -c $BUILD_CONFIGURATION -o /app/public /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=public /app/public .
ENTRYPOINT ["dotnet", "F-Driver.API.dll"]
