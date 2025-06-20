#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Servers/MultiSocks/MultiSocks.csproj", "Servers/MultiSocks/"]
COPY ["BlazeSDK/Blaze2SDK/Blaze2SDK.csproj", "BlazeSDK/Blaze2SDK/"]
COPY ["BlazeSDK/BlazeCommon/BlazeCommon.csproj", "BlazeSDK/BlazeCommon/"]
COPY ["BackendServices/CastleLibrary/CastleLibrary.csproj", "BackendServices/CastleLibrary/"]
COPY ["BackendServices/CustomLogger/CustomLogger.csproj", "BackendServices/CustomLogger/"]
COPY ["BackendServices/EndianTools/EndianTools.csproj", "BackendServices/EndianTools/"]
COPY ["BlazeSDK/Tdf/Tdf.csproj", "BlazeSDK/Tdf/"]
COPY ["BlazeSDK/Blaze3SDK/Blaze3SDK.csproj", "BlazeSDK/Blaze3SDK/"]
COPY ["BackendServices/NetworkLibrary/NetworkLibrary.csproj", "BackendServices/NetworkLibrary/"]
COPY ["BackendServices/CompressionLibrary/CompressionLibrary.csproj", "BackendServices/CompressionLibrary/"]
COPY ["BackendServices/TechnitiumLibrary.Net.Firewall/TechnitiumLibrary.Net.Firewall.csproj", "BackendServices/TechnitiumLibrary.Net.Firewall/"]
RUN dotnet restore "Servers/MultiSocks/MultiSocks.csproj"
COPY . .
WORKDIR "/src/Servers/MultiSocks"
RUN dotnet build "MultiSocks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MultiSocks.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MultiSocks.dll"]