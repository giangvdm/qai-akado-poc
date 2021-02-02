@ECHO OFF
SETLOCAL ENABLEEXTENSIONS
SET SCRIPT_DIR=%~dp0
SET PARAM1=%1
SET PARAM2=%2
SET BUILD_MODE=Release
SET BUILD_CLEAN=/Build
SET BUILD_OUTDIR=%SCRIPT_DIR%bin\%BUILD_MODE%
REM Find Visual Studio directory
if exist "%programfiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" (
  for /F "tokens=* USEBACKQ" %%F in (`"%programfiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -version 16.0 -property installationPath`) do set VS_INSTALLPATH=%%F
)

if "%VS_INSTALLPATH%" == "" (
	ECHO Cannot detect VsDevCmd location.
	GOTO Exit
)

IF /I "%PARAM1%"=="Debug" (
 SET BUILD_MODE=Debug
)
IF /I "%PARAM1%"=="Debug" (
 SET BUILD_OUTDIR=%SCRIPT_DIR%bin\%BUILD_MODE%
)

IF /I "%PARAM2%"=="Rebuild" BUILD_CLEAN=/Rebuild

CALL "%VS_INSTALLPATH%\Common7\Tools\VsDevCmd.bat"

@echo %BUILD_MODE% --- %BUILD_OUTDIR%

@echo Build Win32.UtilModule project ...
@echo "-----x64 platform-----"
devenv akaAgent.sln %BUILD_CLEAN% "%BUILD_MODE%|x64" /Project "Win32.UtilModule\Win32.UtilModule.vcxproj"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

@echo "-----x86 platform-----"
devenv akaAgent.sln %BUILD_CLEAN% "%BUILD_MODE%|x86" /Project "Win32.UtilModule\Win32.UtilModule.vcxproj"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR


@echo Build InstallerUtils project ...
@echo "-----x64 platform-----"
devenv akaAgent.sln %BUILD_CLEAN% "%BUILD_MODE%|x64" /Project "InstallerUtils\InstallerUtils.vcxproj"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

@echo "-----x86 platform-----"
devenv akaAgent.sln %BUILD_CLEAN% "%BUILD_MODE%|x86" /Project "InstallerUtils\InstallerUtils.vcxproj"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

@echo "==============Build Agent=============="
@echo devenv akaAgent.sln %BUILD_CLEAN% "%BUILD_MODE%"
devenv akaAgent.sln %BUILD_CLEAN% "%BUILD_MODE%"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Copying x64 libraries
@echo xcopy /f /y /d %BUILD_OUTDIR%\x64\*.* %BUILD_OUTDIR%\netcoreapp3.1\x64\
xcopy /f /y /d %BUILD_OUTDIR%\x64\*.* %BUILD_OUTDIR%\netcoreapp3.1\x64\
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
REM Copying x86 libraries
xcopy /f /y /d %BUILD_OUTDIR%\x86\*.* %BUILD_OUTDIR%\netcoreapp3.1\x86\
IF %ERRORLEVEL% NEQ 0 GOTO ERROR


GOTO Exit

:ERROR
ECHO There are some error while building solution.
:Exit
