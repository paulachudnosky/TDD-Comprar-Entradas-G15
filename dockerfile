# --- Etapa 1: Build del Backend (.NET) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos y restauramos las dependencias del proyecto .NET
COPY src/EcoHarmony.Tickets.Api/*.csproj ./src/EcoHarmony.Tickets.Api/
COPY src/EcoHarmony.Tickets.Domain/*.csproj ./src/EcoHarmony.Tickets.Domain/
COPY tests/EcoHarmony.Tickets.Tests/*.csproj ./tests/EcoHarmony.Tickets.Tests/
RUN dotnet restore src/EcoHarmony.Tickets.Api/EcoHarmony.Tickets.Api.csproj

# Copiamos todo el código fuente y publicamos la API
COPY . .
RUN dotnet publish src/EcoHarmony.Tickets.Api -c Release -o /app/publish

# --- Etapa 2: Imagen Final ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Exponemos ambos puertos
EXPOSE 5080
EXPOSE 8080

# Cambiamos a usuario root para instalar todo lo necesario
USER root
RUN apt-get update && apt-get install -y nodejs npm

# Copiamos el backend publicado desde la etapa 'build'
COPY --from=build /app/publish .

# Copiamos la carpeta frontend (que ahora incluye server.js)
COPY frontend/ ./frontend

# Instalamos las dependencias de Node en la carpeta frontend (aún como root)
RUN npm install express --prefix frontend

# --- CAMBIO IMPORTANTE ---
# Ahora sí, volvemos al usuario sin privilegios justo antes de ejecutar la aplicación
USER app

# Comando de inicio: Inicia el backend y ejecuta el server.js desde su nueva ubicación
CMD /bin/bash -c "dotnet EcoHarmony.Tickets.Api.dll & node frontend/server.js"
