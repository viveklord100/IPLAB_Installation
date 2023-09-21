rem This script installs IP App and IP Tests
schtasks /Create /XML "IPLabInstallationPart4.xml" /TN "IP Lab Installation Part4" /F
cd /D C:\Test
net use stop workstation
net use start workstation
call reinstall_ip_and_subsystem_test.cmd IVVR
schtasks /Delete /TN "IP Lab Installation Part3" /F
call ForceReboot.cmd