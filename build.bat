@echo off
@echo MultiServer build script 27/03/2026
@echo.

@echo Cleaning up directories:
@rmdir /S /Q ~BuildOutput
@echo.

:Build
@echo Building MultiServer...
dotnet restore PSHMultiServer.slnf
dotnet clean PSHMultiServer.slnf
dotnet build PSHMultiServer.slnf --configuration Debug --property WarningLevel=0
dotnet build PSHMultiServer.slnf --configuration Release --property WarningLevel=0

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
xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Debug/net10.0-windows" "~BuildOutput/Debug"
xcopy /E /Y /I "Win32GUI/RemoteControl/bin/Release/net10.0-windows" "~BuildOutput/Release"
xcopy /E /Y /I "Win32GUI/Json Editor/bin/Debug/net10.0-windows" "~BuildOutput/Debug"
xcopy /E /Y /I "Win32GUI/Json Editor/bin/Release/net10.0-windows" "~BuildOutput/Release"

@echo Scanning plugin static assets...

for /r "Plugins" %%P in (.) do (
    if exist "%%P\bin" (
        echo Processing plugin: %%~nxP

        if exist "%%P\bin\Debug\net10.0\static" (
            xcopy /E /Y /I "%%P\bin\Debug\net10.0\static" "~BuildOutput\Debug\net10.0\static"
        )

        if exist "%%P\bin\Debug\net10.0\runtimes" (
            xcopy /E /Y /I "%%P\bin\Debug\net10.0\runtimes" "~BuildOutput\Debug\net10.0\runtimes"
        )

        if exist "%%P\bin\Release\net10.0\static" (
            xcopy /E /Y /I "%%P\bin\Release\net10.0\static" "~BuildOutput\Release\net10.0\static"
        )

        if exist "%%P\bin\Release\net10.0\runtimes" (
            xcopy /E /Y /I "%%P\bin\Release\net10.0\runtimes" "~BuildOutput\Release\net10.0\runtimes"
        )
    )
)

@echo Crafting final output:
if exist "~BuildOutput/Debug/net10.0" (
    xcopy /E /Y /I "~BuildOutput/Debug/net10.0" "~BuildOutput/Debug"
	@rmdir /S /Q "~BuildOutput/Debug/net10.0"
	if exist "ZZDeps" (
		echo Copying custom files...
		xcopy /E /Y /I "ZZDeps" "~BuildOutput/Debug"
	)
)
if exist "~BuildOutput/Release/net10.0" (
    xcopy /E /Y /I "~BuildOutput/Release/net10.0" "~BuildOutput/Release"
	@rmdir /S /Q "~BuildOutput/Release/net10.0"
	if exist "ZZDeps" (
		echo Copying custom files...
		xcopy /E /Y /I "ZZDeps" "~BuildOutput/Release"
	)
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