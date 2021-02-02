@ECHO OFF
SETLOCAL ENABLEEXTENSIONS
SET SCRIPT_DIR=%~dp0
SET PUBLISH_DIR=%SCRIPT_DIR%Publish\
SET PUBLISH_DIR_X64=%SCRIPT_DIR%Publish\x64\
SET PUBLISH_DIR_X86=%SCRIPT_DIR%Publish\x86\
SET FIXPACK_DIR=%SCRIPT_DIR%Publish\FIX\
SET CURRENT_TIME=%date:~10,4%%date:~4,2%%date:~7,2%
SET ZIP_TOOL_DIR=%SCRIPT_DIR%Tools\7z1900\

set hour=%time:~0,2%
if "%hour:~0,1%" == " " set hour=0%hour:~1,1%
set min=%time:~3,2%
if "%min:~0,1%" == " " set min=0%min:~1,1%
SET CURRENT_TIME=%CURRENT_TIME%-%hour%%min%

@echo Generating fixpack ...
IF EXIST %FIXPACK_DIR% (
  @echo Remove existing fix directory: %FIXPACK_DIR%
  RMDIR /s /q %FIXPACK_DIR%
  IF %ERRORLEVEL% NEQ 0 GOTO ERROR
 )
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
FOR /F "delims=" %%a IN (%SCRIPT_DIR%FileList.txt) DO (
 xcopy /f /y /d %PUBLISH_DIR_X64%%%~a %FIXPACK_DIR%Files\%%~a*
 IF %ERRORLEVEL% NEQ 0 GOTO ERROR
 )
 
REM Copy GPOs
xcopy %SCRIPT_DIR%BrowserExt\GPOs %FIXPACK_DIR%Files\BrowserExt\GPOs /F /Y /E /H /C /I /d

REM Copy complied extension for chromnium & firefox
xcopy %SCRIPT_DIR%BrowserExt\extension-packages %FIXPACK_DIR%Files\BrowserExt /F /Y /E /H /C /I /d

xcopy /f /y /d %SCRIPT_DIR%CustomCmd.bat %FIXPACK_DIR%CustomCmd.bat*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Copy InstallerUtils.exe file
xcopy /f /y /d %SCRIPT_DIR%bin\Release\x64\InstallerUtils.exe %FIXPACK_DIR%Tools\InstallerUtils.exe*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Copy Installer.Net.exe file
xcopy /f /y /d %SCRIPT_DIR%Publish\Installer\Tools\Installer.Net.exe %FIXPACK_DIR%Tools\Installer.Net.exe*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

@ECHO Archives package ...
%ZIP_TOOL_DIR%7za.exe a -tzip "%PUBLISH_DIR%Installer\akaFocusFixpack-x64-%CURRENT_TIME%.zip" "%FIXPACK_DIR%*"

GOTO Exit
:ERROR
ECHO There are some errors while generating fixpack.
:Exit