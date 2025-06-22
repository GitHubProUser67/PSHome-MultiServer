# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Servers/SSFWServer/SSFWServer.csproj", "Servers/SSFWServer/"]
COPY ["AuxiliaryServices/HomeTools/HomeTools.csproj", "AuxiliaryServices/HomeTools/"]
COPY ["BackendServices/NetworkLibrary/NetworkLibrary.csproj", "BackendServices/NetworkLibrary/"]
COPY ["BackendServices/CastleLibrary/CastleLibrary.csproj", "BackendServices/CastleLibrary/"]
COPY ["BackendServices/CustomLogger/CustomLogger.csproj", "BackendServices/CustomLogger/"]
COPY ["BackendServices/EndianTools/EndianTools.csproj", "BackendServices/EndianTools/"]
COPY ["BackendServices/CompressionLibrary/CompressionLibrary.csproj", "BackendServices/CompressionLibrary/"]
COPY ["AuxiliaryServices/WebAPIService/WebAPIService.csproj", "AuxiliaryServices/WebAPIService/"]
COPY ["BackendServices/NetCoreServer/NetCoreServer.csproj", "BackendServices/NetCoreServer/"]
COPY ["BackendServices/TechnitiumLibrary.Net.Firewall/TechnitiumLibrary.Net.Firewall.csproj", "BackendServices/TechnitiumLibrary.Net.Firewall/"]
RUN dotnet restore "./Servers/SSFWServer/SSFWServer.csproj"
COPY . .
WORKDIR "/src/Servers/SSFWServer"
RUN dotnet build "./SSFWServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SSFWServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SSFWServer.dll"]