; BEGIN ISPPBUILTINS.ISS


; END ISPPBUILTINS.ISS

; This script was created by ISTool
; http://www.lerstad.com/istool/                                                                  

;Comment the line below to get an installer that is unsigned.
;#define Sign

[_ISTool]
UseAbsolutePaths=false

[Setup]
AppID=BridgeTimer 
AppName=Bridge Timer
AppVerName= 1.0.0.3
AppCopyright=
UsePreviousAppDir=False
DefaultDirName={commonpf}\Bridge Timer
DefaultGroupName={commonpf}\Bridge Timer
ExtraDiskSpaceRequired=10000000
AppPublisherURL=www.bridgemate.com
OutputDir=Installer
AppPublisher=Andr� Steffens
EnableDirDoesntExistWarning=true
AppVersion=1.0.0.3
UninstallDisplayName=Bridge Timer
WindowVisible=false
DisableStartupPrompt=false
FlatComponentsList=true
UsePreviousSetupType=true
OutputBaseFilename=Bridge Timer Setup
Compression=zip/9
MinVersion=0,6.0.6001
ShowLanguageDialog=yes
LicenseFile=licence.txt
InfoBeforeFile=infobefore.txt
InfoAfterFile=infoafter.txt
DirExistsWarning=no
VersionInfoVersion=1.0.0.3
VersionInfoCompany=Bridge Systems BV
ArchitecturesInstallIn64BitMode=x64
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
Source: ..\bin\Release\netcoreapp3.0\publish\winx64\*.*; DestDir: {app}; Flags:recursesubdirs
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
