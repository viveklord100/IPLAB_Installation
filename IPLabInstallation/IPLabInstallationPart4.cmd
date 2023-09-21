rem This script Creates the ImageStore and Schedules the IP Tests
cd /D C:\Test
call CreateImageStore.cmd
if Exist "C:\Program Files (x86)\PMS\Fusion\FlexVision_Prod" (
CaConsole.exe /FVonflexvision
CaConsole.exe /IPonflexvision
)
if Exist "C:/Program Files (x86)/PMS/Headless/FlexVisionOS_W7_" (
CaConsole.exe /IPonflexvision
)
cd /D C:\Test\ScheduledTasks
call sched_ip_tasks.cmd
schtasks /Delete /TN "IP Lab Installation Part4" /F
Echo %DATE% %TIME% Installation of %TestSystem% Completed with %ERRORLEVEL%
cd /D C:\Test
call ForceReboot.cmd