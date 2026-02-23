# ===== Stage 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copiem fișierele de proiect pentru restore (cacheable)
COPY AISA.Domain/AISA.Domain.csproj AISA.Domain/
COPY AISA.Application/AISA.Application.csproj AISA.Application/
COPY AISA.Infrastructure/AISA.Infrastructure.csproj AISA.Infrastructure/
COPY AISA.API/AISA.API.csproj AISA.API/

RUN dotnet restore AISA.API/AISA.API.csproj

# Copiem tot codul sursă și compilăm
COPY . .
RUN dotnet publish AISA.API/AISA.API.csproj -c Release -o /app/publish --no-restore

# ===== Stage 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AISA.API.dll"]
