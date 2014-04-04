@ECHO OFF
PUSHD %~dp0
SETLOCAL

IF NOT DEFINED DEPLOYMENT_SOURCE (
  SET DEPLOYMENT_SOURCE=%~dp0%.
)

IF NOT DEFINED DEPLOYMENT_TARGET (
  SET DEPLOYMENT_TARGET=%~dp0%artifacts\deploment
)

IF NOT EXIST .nuget (
  MKDIR .nuget
)
IF NOT EXIST .nuget\nuget.exe (
  ECHO Downloading NuGet.exe
  @powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '.nuget\nuget.exe'"
)

ECHO Downloading ProjectK
.nuget\nuget.exe install ProjectK -Prerelease -OutputDirectory packages -NoCache
FOR /D %%G IN (packages\ProjectK.*) DO (
  SET K_CMD=%~dp0%%G\tools\k.cmd
)

FOR /D %%G IN (samples\*.Web) DO (
  CD "%DEPLOYMENT_SOURCE%\%%G"
  CALL "%K_CMD%" restore
  CALL "%K_CMD%" pack --out "%DEPLOYMENT_TARGET%"
)

POPD

