@echo OFF
set /P c=Please enter your desktop directory(C:\Users\x\Desktop\ format):
call "C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Deployment Tools\DandISetEnv.bat"
cd /d "C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Windows Preinstallation Environment" && copype amd64 %c%WIN10