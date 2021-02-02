@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

SET SCRIPT_DIR=%~dp0
REM this argument is set in Update.cs
SET INSTALL_DIR=%1
SET INSTALL_DIR=%INSTALL_DIR:"=%
SET FIREFOX_DIR=C:\Program Files\Mozilla Firefox\
SET FIREFOX_POLICY=%INSTALL_DIR%\firefox\
SET FIX_ID=""
SET MD5_V11=72c1a9f9bc943ea17b68fa084d1be2fe

for /f %%q in ("%~dp0.") do SET FIX_ID=%%~nxq

REM Check if version 1.1 is installed
REG QUERY HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\{62B1D431-D581-4A46-B86C-86E213080F64} /reg:64 > null
if %errorlevel% equ 0 (
	GOTO APPLY_HOTFIX
)

:INSTALL_V11
REM Download v1.1 base file
"%SCRIPT_DIR%Tools\Installer.Net.exe" /md5:"%MD5_V11%" /dest:"%SCRIPT_DIR%..\..\Temp\%MD5_V11%.ZIP"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
REM Extract to update folder
"%SCRIPT_DIR%Tools\Installer.Net.exe" /unzipsrc:"%SCRIPT_DIR%..\..\Temp\%MD5_V11%.ZIP" /unzipdest:"%SCRIPT_DIR%..\%MD5_V11%"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Upgrade to ver1.1
CD "%SCRIPT_DIR%..\%MD5_V11%"
@ECHO CALL "%SCRIPT_DIR%..\%MD5_V11%\CustomCmd.bat"
CALL "%SCRIPT_DIR%..\%MD5_V11%\CustomCmd.bat" "%INSTALL_DIR%"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
CD %SCRIPT_DIR%

:APPLY_HOTFIX

ECHO Applying hotfix in progress, pleas wait ...
@ECHO Stopping aka service and applications ...
%SCRIPT_DIR%Tools/InstallerUtils.exe /moduledir:"%INSTALL_DIR%"  /service:"akaSensorService"
REM Example: copy x86 libraries
REM xcopy /f /y /d "%SCRIPT_DIR%x86\*.dll" %INSTALL_DIR%\x86\
xcopy /f /y /d "%SCRIPT_DIR%Files\*.*" "%INSTALL_DIR%\"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
xcopy /f /y /d "%SCRIPT_DIR%Files\x64\*.*" "%INSTALL_DIR%\x64\"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
xcopy /f /y /d "%SCRIPT_DIR%Files\x86\*.*" "%INSTALL_DIR%\x86\"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Copy BrowserExt
echo xcopy "%SCRIPT_DIR%Files\BrowserExt" "%INSTALL_DIR%\BrowserExt" /F /Y /E /H /C /I /d
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
xcopy "%SCRIPT_DIR%Files\BrowserExt" "%INSTALL_DIR%\BrowserExt" /F /Y /E /H /C /I /d
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM deploy GPOs
echo  "%INSTALL_DIR%\BrowserExt\GPOs\Firefox" "%windir%\PolicyDefinitions" /F /Y /E /H /C /I / /d
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
xcopy "%INSTALL_DIR%\BrowserExt\GPOs\Firefox" %windir%\PolicyDefinitions /F /Y /E /H /C /I /d
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
echo  "%INSTALL_DIR%\BrowserExt\GPOs\Google Chrome" %windir%\PolicyDefinitions /F /Y /E /H /C /I /d
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
xcopy "%INSTALL_DIR%\BrowserExt\GPOs\Google Chrome" %windir%\PolicyDefinitions /F /Y /E /H /C /I /d
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
xcopy "%INSTALL_DIR%\BrowserExt\GPOs\Microsoft Edge" %windir%\PolicyDefinitions /F /Y /E /H /C /I /d
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Register extensions
echo regedit.exe /S "%INSTALL_DIR%\BrowserExt\Chromium\chrome_policy.reg"
regedit.exe /S "%INSTALL_DIR%\BrowserExt\Chromium\chrome_policy.reg"
regedit.exe /S "%INSTALL_DIR%\BrowserExt\Chromium\edge_policy.reg"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

IF EXIST  "%FIREFOX_DIR%" xcopy "%INSTALL_DIR%\BrowserExt\firefox\*.json" "%FIREFOX_DIR%\distribution\" /f /y /d 
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

:SUCCEDED
@echo The akaFocus installs successfully.
@echo %FIX_ID% > "%SCRIPT_DIR%..\latestMd5.txt"

GOTO EXIT
:ERROR
@ECHO There is an error while updating ...
:EXIT

for /F "tokens=3 delims=: " %%H in ('sc query "akaSensorService" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "RUNNING" (
   REM Restart akaSensorService if it's already topped.
   net start "akaSensorService"
  )
)