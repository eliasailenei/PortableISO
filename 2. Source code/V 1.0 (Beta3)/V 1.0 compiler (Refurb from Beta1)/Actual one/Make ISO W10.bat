@echo off
fsutil dirty query %systemdrive%  >nul 2>&1 || (
echo ==== ERROR ====
echo This script require administrator privileges.
echo To do so, right click on this script and select 'Run as administrator'
echo.
echo Press any key to exit...
pause >nul
exit
)
echo Adding drives....
rmdir /S /Q C:\WinPE_x64
rmdir /S /Q C:\Mount
mkdir C:\WinPE_x64
mkdir C:\Mount
set /P desktop=Please enter your desktop directory:
rmdir /S /Q %desktop%WIN10
echo Making image ...
imagex /cleanup
copy "C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Windows Preinstallation Environment\amd64\en-us\winpe.wim" C:\WinPE_x64\winpe.wim
cd /d "C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Deployment Tools\amd64\DISM"
Imagex /mountrw C:\WinPE_x64\winpe.wim 1 C:\Mount
cd /d C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Windows Preinstallation Environment\amd64\WinPE_OCs
dism /image:C:\Mount /add-package /packagepath:"WinPE-WMI.cab"
cd en-us
dism /image:C:\Mount /add-package /packagepath:"WinPE-WMI_en-us.cab"
cd ..
dism /image:C:\Mount /add-package /packagepath:"WinPE-NetFx.cab"
cd en-us
dism /image:C:\Mount /add-package /packagepath:"WinPE-NetFx_en-us.cab"
cd ..
dism /image:C:\Mount /add-package /packagepath:"WinPE-Scripting.cab"
cd en-us
dism /image:C:\Mount /add-package /packagepath:"WinPE-Scripting_en-us.cab"
cd ..
dism /image:C:\Mount /add-package /packagepath:"WinPE-PowerShell.cab"
cd en-us
dism /image:C:\Mount /add-package /packagepath:"WinPE-PowerShell_en-us.cab"
cd ..
dism /image:C:\Mount /add-package /packagepath:"WinPE-DismCmdlets.cab"
cd en-us
dism /image:C:\Mount /add-package /packagepath:"WinPE-DismCmdlets_en-us.cab"
cd ..
dism /image:C:\Mount /add-package /packagepath:"WinPE-EnhancedStorage.cab"
cd en-us
dism /image:C:\Mount /add-package /packagepath:"WinPE-EnhancedStorage_en-us.cab"
cd ..
dism /image:C:\Mount /add-package /packagepath:"WinPE-StorageWMI.cab"
cd en-us
dism /image:C:\Mount /add-package /packagepath:"WinPE-StorageWMI_en-us.cab"
cd ..
start C:\Mount
echo Insert nessesary files NOW...
pause
cd /d "C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Deployment Tools\amd64\DISM"
Imagex /unmount /commit C:\Mount
echo Making ISO....
cd /d %desktop%
start /wait %desktop%script.exe
echo Making ISO....
copy /y C:\WinPE_x64\winpe.wim %desktop%WIN10\media\sources\boot.wim 
echo Making ISO...
set /P c=Name your ISO:
call "C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Deployment Tools\DandISetEnv.bat"
cd /d "C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Windows Preinstallation Environment" && MakeWinPEMedia /ISO %desktop%WIN10 %desktop%%c%.iso