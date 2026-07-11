@echo off
setlocal

set "ROOT=%~dp0"
set "BACKEND_PROJECT=%ROOT%flow_enter_backend\Flowenter.Api\Flowenter.Api.csproj"

if not exist "%BACKEND_PROJECT%" (
  echo [ERROR] Backend project not found: %BACKEND_PROJECT%
  exit /b 1
)

echo Starting flow_enter_backend...
dotnet run --project "%BACKEND_PROJECT%" --launch-profile http

