# Etapa Base
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Instalar dockerize na etapa base
RUN apt-get update && apt-get install -y wget && \
    wget https://github.com/jwilder/dockerize/releases/download/v0.6.1/dockerize-linux-amd64-v0.6.1.tar.gz && \
    tar -C /usr/local/bin -xzvf dockerize-linux-amd64-v0.6.1.tar.gz && \
    rm dockerize-linux-amd64-v0.6.1.tar.gz

# Etapa de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservice.UpdateProtocolStatus.csproj", "."]
RUN dotnet restore "./Microservice.UpdateProtocolStatus.csproj"
COPY . . 
WORKDIR "/src/."
RUN dotnet build "./Microservice.UpdateProtocolStatus.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de Publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Microservice.UpdateProtocolStatus.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar dockerize na etapa final
RUN apt-get update && apt-get install -y wget && \
    wget https://github.com/jwilder/dockerize/releases/download/v0.6.1/dockerize-linux-amd64-v0.6.1.tar.gz && \
    tar -C /usr/local/bin -xzvf dockerize-linux-amd64-v0.6.1.tar.gz && \
    rm dockerize-linux-amd64-v0.6.1.tar.gz

# Copiar a publicação da etapa anterior
COPY --from=publish /app/publish .

# Verificar se dockerize foi instalado corretamente
RUN which dockerize

# Definir o ponto de entrada usando dockerize para esperar outros serviços
ENTRYPOINT ["dockerize", "-wait", "tcp://rabbitmq:5672", "-timeout", "30s", "dotnet", "Microservice.UpdateProtocolStatus.dll"]
