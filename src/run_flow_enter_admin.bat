@echo off
setlocal

set "ROOT=%~dp0"
set "ADMIN_DIR=%ROOT%flow_enter_admin"

if not exist "%ADMIN_DIR%\package.json" (
  echo [ERROR] Frontend project not found: %ADMIN_DIR%
  exit /b 1
)

cd /d "%ADMIN_DIR%"

if not exist "node_modules" (
  echo Installing npm dependencies...
  npm install
  if errorlevel 1 exit /b %errorlevel%
)

echo Starting flow_enter_admin...
npm run dev

