@set PROGFILES=%PROGRAMFILES%
@if exist "%PROGRAMFILES(x86)%" set PROGFILES=%PROGRAMFILES(x86)%

"%PROGFILES%\MSBuild\12.0\Bin\msbuild" /m BSLib.sln /t:Rebuild /p:Configuration=Release "/p:Platform=Any CPU" /p:TargetFrameworkVersion=v4.5 %*
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0

:err
@PAUSE
@exit /B 1
