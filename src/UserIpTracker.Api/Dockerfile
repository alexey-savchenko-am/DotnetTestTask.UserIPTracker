
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["src/UserIpTracker.Api/UserIpTracker.Api.csproj", "UserIpTracker.Api/"]
COPY ["src/UserIpTracker.Application/UserIpTracker.Application.csproj", "UserIpTracker.Application/"]
COPY ["src/UserIpTracker.Domain/UserIpTracker.Domain.csproj", "UserIpTracker.Domain/"]
COPY ["src/SharedKernel.Domain/SharedKernel.Domain.csproj", "SharedKernel.Domain/"]
COPY ["src/UserIpTracker.Infrastructure/UserIpTracker.Infrastructure.csproj", "UserIpTracker.Infrastructure/"]
RUN dotnet restore "./UserIpTracker.Api/UserIpTracker.Api.csproj"
COPY . .
WORKDIR "/src/UserIpTracker.Api"
RUN dotnet build "./UserIpTracker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./UserIpTracker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserIpTracker.Api.dll"]