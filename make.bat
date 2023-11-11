@echo off
setlocal

set CWD=%CD%

rem set MSBUILD="c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
set MSBUILD="c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
rem set MSBUILD="c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

rem Solution file
set SLNFILE=ZenCamera.sln

rem Generate strong key: sn -k sign.snk
set KEYFILE=%CD%\sign.snk

rem Enable assmebly signing
set KEYSIGN=true

rem Using: make.bat strongname
if "%1"=="strongname" goto list-public-keys

if not exist %MSBUILD% goto error-msbuild

if not exist Release mkdir Release
if exist Release del /q Release\*.*

if not "%KEYSIGN%"=="true" (
  rem Build "unsigned" assemblies
  %MSBUILD% %SLNFILE% /t:Build /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=Release\AnyCPU
  rem %MSBUILD% %SLNFILE% /t:Build /p:Configuration=Release /p:Platform=x86 /p:PlatformTarget=x86 /p:OutputPath=Release\x86
  rem %MSBUILD% %SLNFILE% /t:Build /p:Configuration=Release /p:Platform=x64 /p:PlatformTarget=x64 /p:OutputPath=Release\x64
  if not "%ERRORLEVEL%"=="0" goto error-compile
)

if "%KEYSIGN%"=="true" (
  rem Build "signed" strong name assmeblies
  if not exist %KEYFILE% goto error-missing-keyfile
  %MSBUILD% %SLNFILE% /t:Build /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=Release\AnyCPU /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=%KEYFILE% /p:DelaySign=false
  rem %MSBUILD% %SLNFILE% /t:Build /p:Configuration=Release /p:Platform=x86 /p:PlatformTarget=x86 /p:OutputPath=Release\x86 /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=%KEYFILE% /p:DelaySign=false
  rem %MSBUILD% %SLNFILE% /t:Build /p:Configuration=Release /p:Platform=x64 /p:PlatformTarget=x64 /p:OutputPath=Release\x64 /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=%KEYFILE% /p:DelaySign=false
  if not "%ERRORLEVEL%"=="0" goto error-compile
)

rem copy bin\Release\*.dll Release > nul
rem copy bin\Release\*.exe Release > nul
del Release\AnyCPU\*.pdb
del Release\AnyCPU\*.config
del Release\AnyCPU\*.xml
rem del Release\x86\*.pdb
rem del Release\x86\*.config
rem del Release\x64\*.pdb
rem del Release\x64\*.config

echo Done!
goto end

:list-public-keys
rem Check if "sn.exe" is a available
sn >nul 2>&1
if not "%ERRORLEVEL%" == "0" (
  echo Error: Requires Developer Command Prompt for Visual Studio
  exit /b 1
)

rem Check the second argument
if not "%2"=="" (
  echo %2
  sn -T Release\%2 | findstr Public
  echo.
  goto end
)

rem Check each file
call %0 strongname avconv.exe

goto end

:error-missing-keyfile
echo Error: key file not found: %KEYFILE%
echo Hint: open Developer Command Prompt for VS and run
echo   sn -k %KEYFILE%
endlocal
exit /b 0

:error-msbuild
echo Error: msbuild.exe not found
echo Hint: edit make.bat and update the path
endlocal
exit /b 0

:error-compile
echo Failed to build the release!
endlocal
exit /b 0

:end
endlocal
exit /b 0