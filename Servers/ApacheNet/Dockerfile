#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Servers/ApacheNet/ApacheNet.csproj", "Servers/ApacheNet/"]
COPY ["AuxiliaryServices/WebAPIService/WebAPIService.csproj", "AuxiliaryServices/WebAPIService/"]
COPY ["BackendServices/NetworkLibrary/NetworkLibrary.csproj", "BackendServices/NetworkLibrary/"]
COPY ["BackendServices/CastleLibrary/CastleLibrary.csproj", "BackendServices/CastleLibrary/"]
COPY ["BackendServices/CustomLogger/CustomLogger.csproj", "BackendServices/CustomLogger/"]
COPY ["BackendServices/EndianTools/EndianTools.csproj", "BackendServices/EndianTools/"]
COPY ["BackendServices/CompressionLibrary/CompressionLibrary.csproj", "BackendServices/CompressionLibrary/"]
COPY ["BackendServices/DNSLibrary/DNSLibrary.csproj", "BackendServices/DNSLibrary/"]
COPY ["BackendServices/TechnitiumLibrary.Net.Firewall/TechnitiumLibrary.Net.Firewall.csproj", "BackendServices/TechnitiumLibrary.Net.Firewall/"]
COPY ["BackendServices/WatsonWebserver/WatsonWebserver.csproj", "BackendServices/WatsonWebserver/"]
COPY ["BackendServices/SpaceWizards.HttpListener/SpaceWizards.HttpListener.csproj", "BackendServices/SpaceWizards.HttpListener/"]
RUN dotnet restore "Servers/ApacheNet/ApacheNet.csproj"
COPY . .
WORKDIR "/src/Servers/ApacheNet"
RUN dotnet build "ApacheNet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApacheNet.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApacheNet.dll"]