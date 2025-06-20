#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Servers/Horizon/Horizon.csproj", "Servers/Horizon/"]
COPY ["AuxiliaryServices/HorizonService/HorizonService.csproj", "AuxiliaryServices/HorizonService/"]
COPY ["BackendServices/NetworkLibrary/NetworkLibrary.csproj", "BackendServices/NetworkLibrary/"]
COPY ["BackendServices/CastleLibrary/CastleLibrary.csproj", "BackendServices/CastleLibrary/"]
COPY ["BackendServices/CustomLogger/CustomLogger.csproj", "BackendServices/CustomLogger/"]
COPY ["BackendServices/EndianTools/EndianTools.csproj", "BackendServices/EndianTools/"]
COPY ["BackendServices/CompressionLibrary/CompressionLibrary.csproj", "BackendServices/CompressionLibrary/"]
COPY ["AuxiliaryServices/WebAPIService/WebAPIService.csproj", "AuxiliaryServices/WebAPIService/"]
COPY ["BackendServices/PS2FloatLibrary/PS2FloatLibrary.csproj", "BackendServices/PS2FloatLibrary/"]
COPY ["BackendServices/TechnitiumLibrary.Net.Firewall/TechnitiumLibrary.Net.Firewall.csproj", "BackendServices/TechnitiumLibrary.Net.Firewall/"]
COPY ["BackendServices/WatsonWebserver/WatsonWebserver.csproj", "BackendServices/WatsonWebserver/"]
COPY ["BackendServices/SpaceWizards.HttpListener/SpaceWizards.HttpListener.csproj", "BackendServices/SpaceWizards.HttpListener/"]
RUN dotnet restore "Servers/Horizon/Horizon.csproj"
COPY . .
WORKDIR "/src/Servers/Horizon"
RUN dotnet build "Horizon.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Horizon.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Horizon.dll"]