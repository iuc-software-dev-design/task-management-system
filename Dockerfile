FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/BackendApi.csproj", "backend/"]
RUN dotnet restore "backend/BackendApi.csproj"
COPY . .
WORKDIR "/src/backend"
RUN dotnet build "BackendApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackendApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendApi.dll"]
HEALTHCHECK --interval=30s --timeout=30s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1
