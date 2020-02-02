@echo off & cd /d %~dp0
>NUL 2>&1 fltmc || (
    ECHO SET UAC = CreateObject^("Shell.Application"^) > "%TEMP%\Getadmin.vbs"
    ECHO UAC.ShellExecute "%~f0", "%1", "", "runas", 1 >> "%TEMP%\Getadmin.vbs"
    "%TEMP%\Getadmin.vbs"
    DEL /f /q "%TEMP%\Getadmin.vbs" 2>NUL
    Exit /b
)

set service_name=QjobHost
set exeordll=QjobHost

set a=%1
set a0=%1

if defined a goto :swtich

title %service_name% installer
:menu
cls
Echo.
Echo.&Echo   1 → install
Echo.&Echo   2 → uninstall
Echo.&Echo   3 → start
Echo.&Echo   4 → stop
Echo.&Echo   5 → 开启端口52726
Echo.&Echo   6 → 关闭端口52726
Echo.
Set /p a=输入数字按回车：
:swtich
if "%a%"=="1" Echo.& goto sc_install
if "%a%"=="2" Echo.& goto sc_uninstall
if "%a%"=="3" Echo.& goto sc_start
if "%a%"=="4" Echo.& goto sc_stop
if "%a%"=="5" Echo.& goto open_port
if "%a%"=="6" Echo.& goto close_port
Echo.&Echo 输入无效，请重新输入！
pause >nul
goto menu

:sc_install
rem binpath引号里面不能有多余的空格
sc create "%service_name%" displayname= "%service_name%" start= delayed-auto ^
binpath= "\"%cd%\%exeordll%.exe\""
goto next
:sc_uninstall
sc delete %service_name%
goto next

:sc_start
sc start %service_name%
goto next
:sc_stop
sc stop %service_name%
goto next

:open_port
(netsh advfirewall firewall show rule name="port 52726" | find "port 52726")>__tmp__.log
set /p x=<__tmp__.log 
if not defined x ( netsh advfirewall firewall add rule name="port 52726" dir=in protocol=tcp localport=52726 action=allow )
set "x=" & del /s/q __tmp__.log 1>nul 2>nul
goto next
:close_port
netsh advfirewall firewall delete rule "port 52726" in
goto next

:next
ping -n 2 10>nul
if not defined a0 goto menu