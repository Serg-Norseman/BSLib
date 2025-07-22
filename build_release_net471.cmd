@set PROGFILES=%PROGRAMFILES%
@if exist "%PROGRAMFILES(x86)%" set PROGFILES=%PROGRAMFILES(x86)%

set MSB="msbuild_not_found"
@if exist "%PROGFILES%\MSBuild\12.0\Bin\msbuild" set MSB=%PROGFILES%\MSBuild\12.0\Bin\msbuild
@if exist "%PROGFILES%\MSBuild\14.0\Bin\msbuild" set MSB=%PROGFILES%\MSBuild\14.0\Bin\msbuild
@if exist "%PROGFILES%\MSBuild\15.0\Bin\msbuild" set MSB=%PROGFILES%\MSBuild\15.0\Bin\msbuild

"%MSB%" /m BSLib.sln /t:Rebuild /p:Configuration=Release "/p:Platform=Any CPU" /p:TargetFrameworkVersion=v4.7.1 %*
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0

:err
@PAUSE
@exit /B 1
