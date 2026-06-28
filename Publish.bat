@echo off
@echo MultiServer publisher script 27/03/2026
@echo.

@echo Cleaning up directories:
@rmdir /S /Q ~PublishOutput
@echo.

:Build
dotnet restore PSHMultiServer.slnx
dotnet clean PSHMultiServer.slnx

@echo.
@echo ============================================
@echo Building all plugins...
@echo ============================================

for /r "Plugins" %%f in (*.csproj) do (
    echo Building plugin: %%~nxf
    dotnet build "%%f" --configuration Debug --property WarningLevel=0
    dotnet build "%%f" --configuration Release --property WarningLevel=0
)

@echo.
@echo ============================================
@echo Publishing MultiServer...
@echo ============================================

setlocal enabledelayedexpansion

set RIDs=win-x64 win-x86 osx-x64 osx-arm64 linux-x64 linux-arm linux-arm64

set params=-p:DebugType=None -p:DebugSymbols=false --property WarningLevel=0 --self-contained

for %%r in (%RIDs%) do (

    echo.
    echo Publishing for %%r ...

    REM Special handling for linux-armv6 (not yet supported for NET10)
    if "%%r"=="linux-armv6" (
		docker --version >nul 2>&1
		if %ERRORLEVEL% NEQ 0 (
			echo Docker not found. Skipping linux-armv6 build.
		) else (
			echo Building linux-armv6 using Docker...

			REM Install PowerShell then build Debug
			docker run --pull always --rm ^
				-v "%cd%:/src" ^
				-w /src ^
				taphome/dotnet-armv6:latest ^
				/bin/bash -c "apt-get update && apt-get install -y wget apt-transport-https software-properties-common && wget -q https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb && apt-get update && apt-get install -y powershell && dotnet publish PSHMultiServer.slnf -r linux-armv6 -f net10.0 -c Debug %params%"

			echo Copying server Debug publish outputs...

			for /d %%S in ("Servers\*") do (
				if exist "%%S\bin\Debug\net10.0\%%r\publish" (
					xcopy /E /Y /I "%%S\bin\Debug\net10.0\%%r\publish" "~PublishOutput\%%r\Debug"
				)
			)
			
			echo Scanning Debug plugin static assets...

			for /r "Plugins" %%P in (.) do (
				if exist "%%P\bin" (
					echo Processing plugin: %%~nxP

					if exist "%%P\bin\Debug\net10.0\static" (
						xcopy /E /Y /I "%%P\bin\Debug\net10.0\static" "~PublishOutput/%%r/Debug/static"
					)

					if exist "%%P\bin\Debug\net10.0\runtimes" (
						xcopy /E /Y /I "%%P\bin\Debug\net10.0\runtimes" "~PublishOutput/%%r/Debug/runtimes"
					)
				)
			)

			REM Install PowerShell then build Release
			docker run --pull always --rm ^
				-v "%cd%:/src" ^
				-w /src ^
				taphome/dotnet-armv6:latest ^
				/bin/bash -c "apt-get update && apt-get install -y wget apt-transport-https software-properties-common && wget -q https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb && apt-get update && apt-get install -y powershell && dotnet publish PSHMultiServer.slnf -r linux-armv6 -f net10.0 -c Release %params%"
		)
	) else (
		if "%%r"=="win-x64" (
			dotnet publish PSHMultiServer.slnx -r %%r -c Debug -p:PublishReadyToRun=true %params%
			dotnet publish PSHMultiServer.slnx -r %%r -c Release -p:PublishReadyToRun=true %params%
		) else if "%%r"=="win-x86" (
			dotnet publish PSHMultiServer.slnx -r %%r -c Debug -p:PublishReadyToRun=true %params%
			dotnet publish PSHMultiServer.slnx -r %%r -c Release -p:PublishReadyToRun=true %params%
		) else (
		    dotnet publish PSHMultiServer.slnf -r %%r -c Debug -p:PublishReadyToRun=true %params%
			dotnet publish PSHMultiServer.slnf -r %%r -c Release -p:PublishReadyToRun=true %params%
		)
		
		echo Copying server Debug publish outputs...

			for /d %%S in ("Servers\*") do (
				if exist "%%S\bin\Debug\net10.0\%%r\publish" (
					xcopy /E /Y /I "%%S\bin\Debug\net10.0\%%r\publish" "~PublishOutput\%%r\Debug"
				)
			)
		
		echo Scanning Debug plugin static assets...

			for /r "Plugins" %%P in (.) do (
				if exist "%%P\bin" (
					echo Processing plugin: %%~nxP

					if exist "%%P\bin\Debug\net10.0\static" (
						xcopy /E /Y /I "%%P\bin\Debug\net10.0\static" "~PublishOutput/%%r/Debug/static"
					)

					if exist "%%P\bin\Debug\net10.0\runtimes" (
						xcopy /E /Y /I "%%P\bin\Debug\net10.0\runtimes" "~PublishOutput/%%r/Debug/runtimes"
					)
				)
			)
    )
	
	echo Copying server publish outputs...

	for /d %%S in ("Servers\*") do (
		if exist "%%S\bin\Release\net10.0\%%r\publish" (
			xcopy /E /Y /I "%%S\bin\Release\net10.0\%%r\publish" "~PublishOutput\%%r\Release"
		)
	)

    echo Scanning plugin static assets...

    for /r "Plugins" %%P in (.) do (
		if exist "%%P\bin" (
			echo Processing plugin: %%~nxP

			if exist "%%P\bin\Release\net10.0\static" (
				xcopy /E /Y /I "%%P\bin\Release\net10.0\static" "~PublishOutput/%%r/Release/static"
			)

			if exist "%%P\bin\Release\net10.0\runtimes" (
				xcopy /E /Y /I "%%P\bin\Release\net10.0\runtimes" "~PublishOutput/%%r/Release/runtimes"
			)
		)
	)

    if "%%r"=="win-x64" (
        if exist "Win32GUI\RemoteControl\bin\Debug\net10.0-windows7.0\%%r\publish" (
            xcopy /E /Y /I "Win32GUI\RemoteControl\bin\Debug\net10.0-windows7.0\%%r\publish" "~PublishOutput\%%r\Debug"
        )
        if exist "Win32GUI\RemoteControl\bin\Release\net10.0-windows7.0\%%r\publish" (
            xcopy /E /Y /I "Win32GUI\RemoteControl\bin\Release\net10.0-windows7.0\%%r\publish" "~PublishOutput\%%r\Release"
        )
    )

    if "%%r"=="win-x86" (
        if exist "Win32GUI\RemoteControl\bin\Debug\net10.0-windows7.0\%%r\publish" (
            xcopy /E /Y /I "Win32GUI\RemoteControl\bin\Debug\net10.0-windows7.0\%%r\publish" "~PublishOutput\%%r\Debug"
        )
        if exist "Win32GUI\RemoteControl\bin\Release\net10.0-windows7.0\%%r\publish" (
            xcopy /E /Y /I "Win32GUI\RemoteControl\bin\Release\net10.0-windows7.0\%%r\publish" "~PublishOutput\%%r\Release"
        )
    )
	
	if exist "ZZDeps" (
		echo Copying custom files...
		xcopy /E /Y /I "ZZDeps" "~PublishOutput\%%r\Debug"
		xcopy /E /Y /I "ZZDeps" "~PublishOutput\%%r\Release"
	)
)

endlocal

@echo.
@echo Cleaning up temp build files and directories:
for /d /r . %%d in (bin,obj,".vs") do @if exist "%%d" rd /s/q "%%d"

streams.exe -s -d

@echo.
@echo All platforms and kinds of packages are processed.
@echo.
@echo Publishing completed successfully.
@pause
