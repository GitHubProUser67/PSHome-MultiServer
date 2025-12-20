# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Servers/ApacheNet/ApacheNet.csproj", "Servers/ApacheNet/"]
COPY ["AuxiliaryServices/WebAPIService/WebAPIService.csproj", "AuxiliaryServices/WebAPIService/"]
COPY ["BackendServices/MultiServerLibrary/MultiServerLibrary.csproj", "BackendServices/MultiServerLibrary/"]
COPY ["BackendServices/CastleLibrary/CastleLibrary.csproj", "BackendServices/CastleLibrary/"]
COPY ["BackendServices/CustomLogger/CustomLogger.csproj", "BackendServices/CustomLogger/"]
COPY ["BackendServices/EndianTools/EndianTools.csproj", "BackendServices/EndianTools/"]
COPY ["BackendServices/DNSLibrary/DNSLibrary.csproj", "BackendServices/DNSLibrary/"]
COPY ["BackendServices/TechnitiumLibrary.Net.Firewall/TechnitiumLibrary.Net.Firewall.csproj", "BackendServices/TechnitiumLibrary.Net.Firewall/"]
COPY ["BackendServices/WatsonWebserver/WatsonWebserver.csproj", "BackendServices/WatsonWebserver/"]
COPY ["BackendServices/SpaceWizards.HttpListener/SpaceWizards.HttpListener.csproj", "BackendServices/SpaceWizards.HttpListener/"]
RUN dotnet restore "./Servers/ApacheNet/ApacheNet.csproj"
COPY . .
WORKDIR "/src/Servers/ApacheNet"
RUN dotnet build "./ApacheNet.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApacheNet.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApacheNet.dll"]