@echo off
@echo MultiServer publisher script 11/11/2025
@echo.

@echo Make sure to download and install the latest Powershell revision here: https://github.com/PowerShell/PowerShell
@echo.

@echo Cleaning up directories:
@rmdir /S /Q ~PublishOutput
@echo.

:Build
dotnet restore PSHMultiServer.slnf
dotnet clean PSHMultiServer.slnf
dotnet build "Plugins/HTTP/HomeWebTools/HomeWebTools.csproj" --configuration Debug --property WarningLevel=0
dotnet build "Plugins/HTTP/HomeWebTools/HomeWebTools.csproj" --configuration Release --property WarningLevel=0
dotnet build "Plugins/HTTP/EdNetCRCCalculator/EdNetCRCCalculator.csproj" --configuration Debug --property WarningLevel=0
dotnet build "Plugins/HTTP/EdNetCRCCalculator/EdNetCRCCalculator.csproj" --configuration Release --property WarningLevel=0
dotnet build "Plugins/HTTP/PdfToJpeg/PdfToJpeg.csproj" --configuration Debug --property WarningLevel=0
dotnet build "Plugins/HTTP/PdfToJpeg/PdfToJpeg.csproj" --configuration Release --property WarningLevel=0
dotnet build "Plugins/HTTP/SceProfanityFilterService/SceProfanityFilterService.csproj" --configuration Debug --property WarningLevel=0
dotnet build "Plugins/HTTP/SceProfanityFilterService/SceProfanityFilterService.csproj" --configuration Release --property WarningLevel=0

@echo off

setlocal enabledelayedexpansion

rem List of RIDs to publish for
set RIDs=win-x64 win-x86 osx-x64 osx-arm64 linux-x64 linux-arm linux-arm64

rem Common parameters
set params=-p:PublishReadyToRun=true -p:DebugType=None -p:DebugSymbols=false --property WarningLevel=0 --self-contained

for %%r in (%RIDs%) do (
    @echo Publishing MultiServer for %%r ...
    dotnet publish PSHMultiServer.slnf -r %%r -c Debug %params%
    dotnet publish PSHMultiServer.slnf -r %%r -c Release %params%
	
	@echo Copying %%r build output to ~PublishOutput...
	xcopy /E /Y /I "Servers/Horizon/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/MultiSocks/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/QuazalServer/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/SSFWServer/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/SVO/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/MultiSpy/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/ApacheNet/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/MitmDNS/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/EdenServer/bin/Debug/net6.0/%%r/publish" "~PublishOutput/%%r/Debug"
	xcopy /E /Y /I "Servers/Horizon/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/MultiSocks/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/QuazalServer/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/SSFWServer/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/SVO/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/MultiSpy/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/ApacheNet/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/MitmDNS/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	xcopy /E /Y /I "Servers/EdenServer/bin/Release/net6.0/%%r/publish" "~PublishOutput/%%r/Release"
	if exist "Plugins/HTTP/HomeWebTools/bin/Debug/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/HomeWebTools/bin/Debug/net6.0/static" "~PublishOutput/%%r/Debug/static"
	)
	if exist "Plugins/HTTP/HomeWebTools/bin/Release/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/HomeWebTools/bin/Release/net6.0/static" "~PublishOutput/%%r/Release/static"
	)
	if exist "Plugins/HTTP/EdNetCRCCalculator/bin/Debug/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/EdNetCRCCalculator/bin/Debug/net6.0/static" "~PublishOutput/%%r/Debug/static"
	)
	if exist "Plugins/HTTP/EdNetCRCCalculator/bin/Release/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/EdNetCRCCalculator/bin/Release/net6.0/static" "~PublishOutput/%%r/Release/static"
	)
	if exist "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/static" "~PublishOutput/%%r/Debug/static"
	)
	if exist "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/static" "~PublishOutput/%%r/Release/static"
	)
	if exist "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/runtimes" (
		xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/runtimes" "~PublishOutput/%%r/Debug/runtimes"
	)
	if exist "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/runtimes" (
		xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/runtimes" "~PublishOutput/%%r/Release/runtimes"
	)
	if exist "Plugins/HTTP/SceProfanityFilterService/bin/Debug/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/SceProfanityFilterService/bin/Debug/net6.0/static" "~PublishOutput/%%r/Debug/static"
	)
	if exist "Plugins/HTTP/SceProfanityFilterService/bin/Release/net6.0/static" (
		xcopy /E /Y /I "Plugins/HTTP/SceProfanityFilterService/bin/Release/net6.0/static" "~PublishOutput/%%r/Release/static"
	)
	if exist "Plugins/HTTP/SceProfanityFilterService/bin/Debug/net6.0/runtimes" (
		xcopy /E /Y /I "Plugins/HTTP/SceProfanityFilterService/bin/Debug/net6.0/runtimes" "~PublishOutput/%%r/Debug/runtimes"
	)
	if exist "Plugins/HTTP/SceProfanityFilterService/bin/Release/net6.0/runtimes" (
		xcopy /E /Y /I "Plugins/HTTP/SceProfanityFilterService/bin/Release/net6.0/runtimes" "~PublishOutput/%%r/Release/runtimes"
	)
	if "%%r"=="win-x64" (
		xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Debug/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Debug"
		xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Release/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Release"
		xcopy /E /Y /I "Win32GUI/Json Editor/bin/Debug/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Debug"
		xcopy /E /Y /I "Win32GUI/Json Editor/bin/Release/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Release"
	)
	if "%%r"=="win-x86" (
		xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Debug/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Debug"
		xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Release/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Release"
		xcopy /E /Y /I "Win32GUI/Json Editor/bin/Debug/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Debug"
		xcopy /E /Y /I "Win32GUI/Json Editor/bin/Release/net6.0-windows/%%r/publish" "~PublishOutput/%%r/Release"
	)
)

endlocal

@echo Cleaning up temp build files and directories:
for /d /r . %%d in (bin,obj,".vs") do @if exist "%%d" rd /s/q "%%d"

streams.exe -s -d

@echo.

@echo.
@echo All platforms and kinds of packages are processed.
@echo.

@echo Publishing completed successfully.
@pause