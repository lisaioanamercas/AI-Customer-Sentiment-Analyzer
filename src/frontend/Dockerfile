# ===== Stage 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY AISA.Frontend/AISA.Frontend.csproj AISA.Frontend/
RUN dotnet restore AISA.Frontend/AISA.Frontend.csproj

COPY . .
RUN dotnet publish AISA.Frontend/AISA.Frontend.csproj -c Release -o /app/publish --no-restore

# ===== Stage 2: Serve cu nginx =====
FROM nginx:alpine AS runtime

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 5010

CMD ["nginx", "-g", "daemon off;"]
