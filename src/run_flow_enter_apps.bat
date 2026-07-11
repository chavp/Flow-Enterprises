@echo off
setlocal

set "ROOT=%~dp0"

start "flow_enter_backend" cmd /k "%ROOT%run_flow_enter_backend.bat"
start "flow_enter_admin" cmd /k "%ROOT%run_flow_enter_admin.bat"

echo Started flow_enter_backend and flow_enter_admin in separate windows.

