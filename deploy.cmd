@ECHO OFF
PUSHD %~dp0
SETLOCAL

IF NOT DEFINED DEPLOYMENT_SOURCE (
  SET DEPLOYMENT_SOURCE=%~dp0%.
)

IF NOT DEFINED DEPLOYMENT_TARGET (
  SET DEPLOYMENT_TARGET=%~dp0%artifacts\deploment
)

ECHO Downloading NuGet.exe
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '.nuget\nuget.exe'"

ECHO Downloading ProjectK
.nuget\nuget.exe install ProjectK -Version 0.1-alpha-291 -OutputDirectory packages -NoCache
SET K_CMD=%~dp0packages\ProjectK.0.1-alpha-291\tools\k.cmd

CD "%DEPLOYMENT_SOURCE%\samples\Builder.Filtering.Web"
CALL "%K_CMD%" restore
CALL "%K_CMD%" pack --out "%DEPLOYMENT_TARGET%"

POPD

