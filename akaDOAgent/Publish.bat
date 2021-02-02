@ECHO OFF
SETLOCAL ENABLEEXTENSIONS
SET SCRIPT_DIR=%~dp0
SET BUILD_MODE=Release
SET BUILD_CLEAN=/Build
SET BUILD_OUTDIR=%SCRIPT_DIR%bin\%BUILD_MODE%
SET PUBLISH_DIR_X64=%SCRIPT_DIR%Publish\x64\
SET PUBLISH_DIR_X86=%SCRIPT_DIR%Publish\x86\
SET MAKE_UPGRADE=%1
SET FIXPACK_DIR=%SCRIPT_DIR%Publish\FIX\

REM Compiling the projects ...
CALL %SCRIPT_DIR%build.bat

REM Publish the akaSensorAgent project
@ECHO Publish the akaSensorAgent project ....
dotnet publish -r win-x64 %SCRIPT_DIR%akaSensorAgent\akaSensorAgent.csproj -o %SCRIPT_DIR%Publish\x64\ -c %BUILD_MODE% --self-contained true
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
dotnet publish -r win-x86 %SCRIPT_DIR%akaSensorAgent\akaSensorAgent.csproj -o %SCRIPT_DIR%Publish\x86\ -c %BUILD_MODE% --self-contained true
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Publish the akaFocus.Monitor project
@ECHO Publish the akaForcus.Monitor project ....
dotnet publish -r win-x64 %SCRIPT_DIR%akaForcus.Monitor\akaFocus.Monitor.csproj -o %SCRIPT_DIR%Publish\x64\ -c %BUILD_MODE% --self-contained true
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
dotnet publish -r win-x86 %SCRIPT_DIR%akaForcus.Monitor\akaFocus.Monitor.csproj -o %SCRIPT_DIR%Publish\x86\ -c %BUILD_MODE% --self-contained true


REM Publish the akaFocusUpdater project
@ECHO Publish the akaFocusUpdater project ....
dotnet publish -r win-x64 %SCRIPT_DIR%akaFocusUpdater\akaFocus.Updater.csproj -o %SCRIPT_DIR%Publish\x64\ -c %BUILD_MODE% --self-contained true
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
dotnet publish -r win-x86 %SCRIPT_DIR%akaFocusUpdater\akaFocus.Updater.csproj -o %SCRIPT_DIR%Publish\x86\ -c %BUILD_MODE% --self-contained true

REM Publish download tool in single file
dotnet publish -r win-x64 %SCRIPT_DIR%Installer.Net\Installer.Net.csproj -o %SCRIPT_DIR%Publish\Installer\Tools\ -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true


REM Copying x64 libraries
@echo xcopy /f /y /d %BUILD_OUTDIR%\x64\*.dll %PUBLISH_DIR_X64%x64
xcopy /f /y /d %BUILD_OUTDIR%\x64\*.dll %PUBLISH_DIR_X64%x64\
@echo xcopy /f /y /d %BUILD_OUTDIR%\x64\*.dll %PUBLISH_DIR_X64%x86
xcopy /f /y /d %BUILD_OUTDIR%\x64\*.dll %PUBLISH_DIR_X64%x86\
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
REM Copying x86 libraries
@echo xcopy /f /y /d %BUILD_OUTDIR%\x86\*.dll %PUBLISH_DIR_X86%x64
xcopy /f /y /d %BUILD_OUTDIR%\x86\*.dll %PUBLISH_DIR_X86%x64\
@echo xcopy /f /y /d %BUILD_OUTDIR%\x86\*.dll %PUBLISH_DIR_X86%x86
xcopy /f /y /d %BUILD_OUTDIR%\x86\*.dll %PUBLISH_DIR_X86%x86\
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Copying InstallerUtils.exe
xcopy /f /y /d %BUILD_OUTDIR%\x64\*.exe %PUBLISH_DIR_X64%
xcopy /f /y /d %BUILD_OUTDIR%\x86\*.exe %PUBLISH_DIR_X86%

REM Copying GPO files ...
@echo Copying GPO files ...
CALL %SCRIPT_DIR%ExtensionIntergration.bat


if exist "%programfiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" (
  for /F "tokens=* USEBACKQ" %%F in (`"%programfiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -version 16.0 -property installationPath`) do set VS_INSTALLPATH=%%F
)

if "%VS_INSTALLPATH%" == "" (
	ECHO Cannot detect VsDevCmd location.
	GOTO Exit
)
CALL "%VS_INSTALLPATH%\Common7\Tools\VsDevCmd.bat"

REM build custom action project
@echo Build custom action project
devenv akaAgent.sln /Build "Release" /Project "Installer.CustomAction.PwdPrompt\ClosePromptCA.csproj"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM build installer
@echo "-----x64 platform installer-----"
devenv akaAgent.sln /Build "Release|x64" /Project "Installer\Installer.wixproj"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

@echo "-----x86 platform installer-----"
devenv akaAgent.sln /Build "Release|x86" /Project "Installer\Installer.wixproj"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

IF "%MAKE_UPGRADE%"=="makeupgrade" (
 GOTO MAKEUPGRADE
) ELSE (
 GOTO MAKEHOTFIX
)

:MAKEUPGRADE
@ECHO Generating upgrade msi hotfix ...
CALL %SCRIPT_DIR%UpdateVersionCmd\MakeFix.bat
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
GOTO Exit

:MAKEHOTFIX
REM Make fixpack
@ECHO Generating file hotfix ...
call %SCRIPT_DIR%MakeFix.bat
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
GOTO Exit

:ERROR
ECHO There are some errors while publishing the solution.
:Exit