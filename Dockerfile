# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar sólo el csproj primero para aprovechar cache de restore
COPY ["virtualbook_backend.csproj", "./"]
RUN dotnet restore "./virtualbook_backend.csproj"

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish "virtualbook_backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar artefactos publicados
COPY --from=build /app/publish .

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production

# Exponer un puerto por claridad (no obligatorio para Render)
EXPOSE 8080

# Respetar la variable PORT que Render pasa al contenedor. Si no existe, usa 8080.
ENTRYPOINT ["sh", "-c", "export ASPNETCORE_URLS=http://+:${PORT:-8080} && dotnet virtualbook_backend.dll"]