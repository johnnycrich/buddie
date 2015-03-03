setlocal

SET BUILD_TYPE=Release
if "%1" == "Debug" set BUILD_TYPE=Debug

REM Determine whether we are on an 32 or 64 bit machine
if "%PROCESSOR_ARCHITECTURE%"=="x86" if "%PROCESSOR_ARCHITEW6432%"=="" goto x86

set ProgramFilesPath=%ProgramFiles(x86)%

goto startInstall

:x86

set ProgramFilesPath=%ProgramFiles%

:startInstall

pushd "%~dp0"

set PROJECTNAME=Buddie_game
set PRODUCTVERSION=1.0.1
set WIX_BUILD_LOCATION=%ProgramFiles%\Windows Installer XML v3\bin
set OUTPUTNAME=%PROJECTNAME%.msi

REM Cleanup leftover intermediate files

del /f /q "*.wixobj"
del /f /q "%OUTPUTNAME%"

REM Build the MSI for the setup package

"%WIX_BUILD_LOCATION%\candle.exe" "%PROJECTNAME%.wxs" -dProductVersion=%PRODUCTVERSION% -out "%PROJECTNAME%.wixobj"
"%WIX_BUILD_LOCATION%\candle.exe" "Buddie.wxs" -dProductVersion=%PRODUCTVERSION% -out "Buddie.wixobj"
"%WIX_BUILD_LOCATION%\light.exe" "%PROJECTNAME%.wixobj" "Buddie.wixobj" -cultures:en-US -ext "%WIX_BUILD_LOCATION%\WixUIExtension.dll" -ext "%WIX_BUILD_LOCATION%\WixNetfxExtension.dll" -loc "%PROJECTNAME%_en-us.wxl" -out "%OUTPUTNAME%"

popd

endlocal