# Build stage
ARG CACHEBUST=1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG CACHEBUST
RUN echo "Cache bust: $CACHEBUST"
WORKDIR /src

# Copiar sólo el csproj para aprovechar cache de restore (ruta corregida)
COPY ["virtualbook_backend/virtualbook_backend.csproj", "./"]
RUN dotnet restore "./virtualbook_backend.csproj"

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish "virtualbook_backend/virtualbook_backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar artefactos publicados
COPY --from=build /app/publish .
            
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production

# Exponer un puerto por claridad (Render provee PORT)
EXPOSE 8080

# Usar la variable PORT que Render exporta; por defecto 8080
ENTRYPOINT ["sh", "-c", "export ASPNETCORE_URLS=http://+:${PORT:-8080} && dotnet virtualbook_backend.dll"]