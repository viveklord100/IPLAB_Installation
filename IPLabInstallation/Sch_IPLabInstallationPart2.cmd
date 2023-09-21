rem Schedules Installation Part2
schtasks /Create /XML "IPLabInstallationPart2.xml" /TN "IP Lab Installation Part2" /F
cd /d C:\Test
call ForceReboot.cmd