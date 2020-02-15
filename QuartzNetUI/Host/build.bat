@echo off & cd /d %~dp0

:menu
cls
Echo.
Echo.&Echo   1 → build_win
Echo.&Echo   2 → build_console
Echo.&Echo   3 → build_docker
Echo.
Set /p a=输入数字按回车：
if "%a%"=="1" Echo.& goto build_win
if "%a%"=="2" Echo.& goto build_console
if "%a%"=="3" Echo.& goto build_docker
Echo.&Echo 输入无效，请重新输入！
pause >nul
goto menu

:build_win
rd /s/q "obj" 1>nul 2>nul
set hostType=win
echo hostType=%hostType%
dotnet publish -o "bin\publish_win"
copy /y "..\libs\hostfxr.dll" "bin\publish_win\hostfxr.dll"
copy /y "..\libs\hostpolicy.dll" "bin\publish_win\hostpolicy.dll"
copy /y "..\libs\QjobHost.exe" "bin\publish_win\QjobHost.exe"
del /s/q "bin\publish_win\appsettings.Development.json" 1>nul 2>nul
del /s/q "bin\publish_win\web.config" 1>nul 2>nul
goto next

:build_console
rd /s/q "obj" 1>nul 2>nul
set hostType=console
echo hostType=%hostType%
dotnet publish -o "bin\publish_console"
goto next

:build_docker
rd /s/q "obj" 1>nul 2>nul
set hostType=docker
echo hostType=%hostType%
dotnet publish -r linux-x64 -o "bin\publish_linux64"
goto next

:next
pause
goto menu