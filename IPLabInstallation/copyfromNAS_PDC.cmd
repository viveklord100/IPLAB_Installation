Echo OFF
set TestSystem=%1
set Project=%2
set NASIP=130.144.236.18

Echo %DATE% %TIME% Installation of %TestSystem% Started

call :changeHostnameofTestsystem
call :copyfilesfromNAS
call :copyfilesfromPDC %3 %4
call :installMSIS
xcopy /y "F:\InstallScripts\*.lnk" "C:\Users\FusionX\Desktop"

GOTO :EOF

:changeHostnameofTestsystem
wmic computersystem where caption='%computername%' rename '%TestSystem%'
exit /b

:copyfilesfromNAS
rem Copies all required files from NAS to System

net stop workstation /y
net start workstation

net use T: \\%NASIP%\InstallationFiles /User:fusionx t2pmbHtg2.
net use Z: \\%NASIP%\pms_ip_msi

xcopy /y "T:\IPLabInstallation_Scripts\_IPSupportingTools\Hooks" C:\Test\
xcopy /y "T:\IPLabInstallation_Scripts\_IPSupportingTools\InstallScripts" F:\InstallScripts\
xcopy /y "T:\IPLabInstallation_Scripts\_IPSupportingTools\PasswStartStop" F:\PasswStartStop\
xcopy /y "T:\IPLabInstallation_Scripts\_IPSupportingTools\MailList" C:\Test\
if /i %Project%==TIGER xcopy /y "T:\Nunit\NUnit-2.6.4.msi" F:\TmpInstall\
if /i %Project%==LION xcopy /y "T:\Nunit\NUnit-2.5.9*.msi" F:\TmpInstall\
xcopy /y "T:\UltraEdit 16.30\UltraEdit.msi" F:\TmpInstall\
xcopy /y "T:\Sysinternals\Sysinternals Suite 100.msi" F:\TmpInstall\
xcopy /y "T:\DevOSPackage\OS_DevPackage_Tiger.msi" F:\TmpInstall\
xcopy /y "T:\VNC\tightvnc*.msi" F:\TmpInstall\
xcopy /y "Z:\%IPSWPROJECT%\PMS_IP_ModuleTest.msi" F:\TmpInstall\
xcopy /y "Z:\%IPSWPROJECT%\*.cmd" C:\Test\
xcopy /y "F:\InstallScripts\cleanup_msi.cmd" C:\Test\


net use Z: /d /y
net use T: /d /y
exit /b

:copyfilesfromPDC
rem Copy Files from PDC
call :copymsis PMS_Infra
call :copymsis PMS_OS
call :copymsis PMS_IPOS

for %%x in (%*) do (
if /i %%x==FV1 call :copymsis PMS_FlexVisionOS
if /i %%x==FV2 (
call :copymsis PMS_FVOS
call :copymsis PMS_FlexVisionOS
call :copymsis PMS_AUI
xcopy /y "\\%NASIP%\pms_aui_msi\Allura_Main_AUI_PreInt\*.cmd" C:\Test\
)
if /i %%x==TSM call :copymsis PMS_TSMBOS
)
exit /b

:copymsis
xcopy /y U:\ApplicationSoftware\SubSystems\"%1"*.msi F:\TmpInstall\
exit /b

:installMSIS
set INSTALL_DIR=F:\TmpInstall

call :install PMS
call :install NUnit
call :install Sysinternals
call :install UltraEdit
call :install tightvnc
call :install OS_Dev
exit /b

:install <msiNames>
if EXIST "%INSTALL_DIR%\%1*.msi" (
  echo try installation of: %1*
  
  for /F "tokens=*" %%A in ('dir /b "%INSTALL_DIR%\%1*.msi"') do echo found ["%INSTALL_DIR%\%%A"]
  echo ------------------
  for /F "tokens=*" %%A in ('dir /b "%INSTALL_DIR%\%1*.msi"') do call :installFile "%INSTALL_DIR%\%%A"  
) else (
  echo msi-group %1* not present, group skipped
)
if EXIST "C:\Program Files (x86)\PMS\Fusion\FlexVision_Prod" call "C:\Program Files (x86)\PMS\Fusion\FlexVision_Prod\PostInstallTool\Install PMS_FlexVision.lnk"
exit /b


:installFile <msiFileName>
if EXIST %1 (
  echo install file: [%1]
  echo /i %1 /qn /L*v %~dpn1.log
  msiexec /i %1 /qn /L*v "%~dpn1.log"  
) else (
  echo msi %1 not present, installation skipped
)
exit /b

