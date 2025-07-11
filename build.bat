@echo off
@echo MultiServer build script 11/07/2025
@echo.

@echo Cleaning up directories:
@rmdir /S /Q ~BuildOutput
@echo.

:Build
@echo Building MultiServer...
dotnet restore MultiServer3.slnf
dotnet clean MultiServer3.slnf
dotnet build MultiServer3.slnf --configuration Debug --property WarningLevel=0
dotnet build MultiServer3.slnf --configuration Release --property WarningLevel=0

@echo Copying build output to ~BuildOutput...
xcopy /E /Y /I "Servers/Horizon/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/MultiSocks/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/QuazalServer/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/SSFWServer/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/SVO/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/MultiSpy/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/ApacheNet/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/MitmDNS/bin" "~BuildOutput"
xcopy /E /Y /I "Servers/EdenServer/bin" "~BuildOutput"
xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Debug/net6.0-windows" "~BuildOutput/Debug"
xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Release/net6.0-windows" "~BuildOutput/Release"
xcopy /E /Y /I "Win32GUI/Json Editor/bin/Debug/net6.0-windows" "~BuildOutput/Debug"
xcopy /E /Y /I "Win32GUI/Json Editor/bin/Release/net6.0-windows" "~BuildOutput/Release"
if exist "Plugins/HTTP/HomeWebTools/bin/Debug/net6.0/static" (
    xcopy /E /Y /I "Plugins/HTTP/HomeWebTools/bin/Debug/net6.0/static" "~BuildOutput/Debug/net6.0/static"
)
if exist "Plugins/HTTP/HomeWebTools/bin/Release/net6.0/static" (
    xcopy /E /Y /I "Plugins/HTTP/HomeWebTools/bin/Release/net6.0/static" "~BuildOutput/Release/net6.0/static"
)
if exist "Plugins/HTTP/EdNetCRCCalculator/bin/Debug/net6.0/static" (
    xcopy /E /Y /I "Plugins/HTTP/EdNetCRCCalculator/bin/Debug/net6.0/static" "~BuildOutput/Debug/net6.0/static"
)
if exist "Plugins/HTTP/EdNetCRCCalculator/bin/Release/net6.0/static" (
    xcopy /E /Y /I "Plugins/HTTP/EdNetCRCCalculator/bin/Release/net6.0/static" "~BuildOutput/Release/net6.0/static"
)
if exist "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/static" (
    xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/static" "~BuildOutput/Debug/net6.0/static"
)
if exist "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/static" (
    xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/static" "~BuildOutput/Release/net6.0/static"
)
if exist "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/runtimes" (
    xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Debug/net6.0/runtimes" "~BuildOutput/Debug/net6.0/runtimes"
)
if exist "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/runtimes" (
    xcopy /E /Y /I "Plugins/HTTP/PdfToJpeg/bin/Release/net6.0/runtimes" "~BuildOutput/Release/net6.0/runtimes"
)

@echo Crafting final output:
if exist "~BuildOutput/Debug/net6.0" (
    xcopy /E /Y /I "~BuildOutput/Debug/net6.0" "~BuildOutput/Debug"
	@rmdir /S /Q "~BuildOutput/Debug/net6.0"
)
if exist "~BuildOutput/Release/net6.0" (
    xcopy /E /Y /I "~BuildOutput/Release/net6.0" "~BuildOutput/Release"
	@rmdir /S /Q "~BuildOutput/Release/net6.0"
)
@echo.

@echo Cleaning up temp build files and directories:
for /d /r . %%d in (bin,obj,".vs") do @if exist "%%d" rd /s/q "%%d"

streams.exe -s -d

@echo.

@echo.
@echo All platforms and kinds of packages are processed.
@echo.

@echo Build completed successfully.
@pause