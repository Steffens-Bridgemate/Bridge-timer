; BEGIN ISPPBUILTINS.ISS




; END ISPPBUILTINS.ISS

; This script was created by ISTool
; http://www.lerstad.com/istool/                                                                  

;Comment the line below to get an installer that is unsigned.

[_ISTool]
UseAbsolutePaths=false

[Setup]
 SignTool=DigicertSHA1 $f
 SignTool=DigicertSHA256 $f
AppID=BridgeTimer
AppName=Bridge Timer
AppVerName= Bridge Timer
AppCopyright=
UsePreviousAppDir=False
DefaultDirName={commonpf}\Bridge Timer
DefaultGroupName=Bridge Timer
ExtraDiskSpaceRequired=10000000
AppPublisherURL=www.bridgemate.com
OutputDir=Installer
AppPublisher=André Steffens
EnableDirDoesntExistWarning=true
AppVersion=1.0.0.11
UninstallDisplayName=Bridge Timer
WindowVisible=false
DisableStartupPrompt=false
FlatComponentsList=true
UsePreviousSetupType=true
OutputBaseFilename=BridgeTimerSetup32
Compression=zip/9
MinVersion=0,6.0.6001
ShowLanguageDialog=yes
LicenseFile=licence.txt
InfoBeforeFile=infobefore.txt
InfoAfterFile=infoafter.txt
DirExistsWarning=no
VersionInfoVersion=1.0.0.11
VersionInfoCompany=Bridge Systems BV
VersionInfoDescription=Bridge Timer installer
AppendDefaultDirName=false
WizardImageFile=Bridge timer.bmp
PrivilegesRequired=admin

[Tasks]
 
[InstallDelete]

[Dirs]
Name: {app}\Sounds
Name: {app}\Images

[Files]
Source: ..\bin\Release\netcoreapp3.0\publish\winx86\*.*; DestDir: {app}; Flags:recursesubdirs
Source: ..\bin\Release\netcoreapp3.0\publish\winx86\BridgeTimer.exe; DestDir:{app}; Flags:sign
Source: InstallerSounds\*.*;DestDir:{userdocs}\Bridge Timer

[Icons]  
Name: {group}\Bridge Timer; Filename: {app}\BridgeTimer.exe; WorkingDir: {app}; IconFilename: {app}\clock.ico;

[Registry]

[Run]

[Languages]

[ThirdPartySettings]
CompileLogMethod=append

[Code]

[InnoIDE_Settings]
LogFileOverwrite=false
