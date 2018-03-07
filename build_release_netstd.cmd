@set PROGFILES=%PROGRAMFILES%
@if exist "%PROGRAMFILES%" set PROGFILES=%PROGRAMFILES%

"%PROGFILES%\dotnet\dotnet" msbuild /m BSLib.netstd.sln /t:clean /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 PAUSE

"%PROGFILES%\dotnet\dotnet" msbuild /m BSLib.netstd.sln /p:Configuration=Release "/p:Platform=Any CPU" %*
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0

:err
@PAUSE
@exit /B 1