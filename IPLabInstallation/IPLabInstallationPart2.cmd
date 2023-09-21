rem This script checks & Installs OS on IP & FV PCs
IPLabInstallationIPFV_OS.exe
schtasks /Create /XML "IPLabInstallationPart3.xml" /TN "IP Lab Installation Part3" /F
schtasks /Delete /TN "IP Lab Installation Part2" /F
cd /D C:\Test
call ForceReboot.cmd