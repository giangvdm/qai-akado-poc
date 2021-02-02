@ECHO OFF
SETLOCAL ENABLEEXTENSIONS
SET SCRIPT_DIR=%~dp0
SET BUILD_MODE=Release
SET BUILD_CLEAN=/Build
SET BUILD_OUTDIR=%SCRIPT_DIR%bin\%BUILD_MODE%
SET PUBLISH_DIR_X64=%SCRIPT_DIR%Publish\x64\
SET PUBLISH_DIR_X86=%SCRIPT_DIR%Publish\x86\
SET MAKE_FIX=%1
SET FIXPACK_DIR=%SCRIPT_DIR%Publish\FIX\


REM Copying GPO files ...
@echo Copying GPO files ...
xcopy "%SCRIPT_DIR%\BrowserExt\GPOs" "%PUBLISH_DIR_X86%BrowserExt\GPOs" /F /Y /E /H /C /I /d
xcopy "%SCRIPT_DIR%\BrowserExt\GPOs" "%PUBLISH_DIR_X64%BrowserExt\GPOs" /F /Y /E /H /C /I /d
xcopy "%SCRIPT_DIR%\BrowserExt\extension-packages" "%PUBLISH_DIR_X86%BrowserExt" /F /Y /E /H /C /I /d
xcopy "%SCRIPT_DIR%\BrowserExt\extension-packages" "%PUBLISH_DIR_X64%BrowserExt" /F /Y /E /H /C /I /d