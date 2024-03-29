; This script was created by ISTool
; http://www.lerstad.com/istool/                                                                  

;Comment the line below to get an installer that is unsigned.
#define Sign
#define AppID "BridgeTimer"
#define Name="Bridge Timer"
#define ExeName "BridgeTimer.exe"
#define Version "1.0.0.11"
#define Publisher "Bridge Systems BV"

[_ISTool]
UseAbsolutePaths=false

[Setup]
#if defined Sign
 SignTool=DigicertSHA1 $f
 SignTool=DigicertSHA256 $f
#endif
AppID={#AppID}
AppName={#Name}
AppVerName= Bridge Timer
AppCopyright=
UsePreviousAppDir=False
DefaultDirName={commonpf}\{#Name}
DefaultGroupName={#Name}
ExtraDiskSpaceRequired=10000000
AppPublisherURL=www.bridgemate.com
OutputDir=Installer
AppPublisher=Andr� Steffens
EnableDirDoesntExistWarning=true
AppVersion={#Version}
UninstallDisplayName={#Name}
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
VersionInfoVersion={#Version}
VersionInfoCompany={#Publisher}
VersionInfoDescription={#Name} installer
AppendDefaultDirName=false
WizardImageFile=Bridge timer.bmp
PrivilegesRequired=admin

[Tasks]
 
[InstallDelete]

[Dirs]
Name: {app}\Sounds
Name: {app}\Images

[Files]
Source: ..\bin\Release\netcoreapp3.1\publish\x86\*.*; DestDir: {app}; Flags:recursesubdirs
Source: ..\bin\Release\netcoreapp3.1\publish\x86\BridgeTimer.exe; DestDir:{app}; Flags:sign
Source: InstallerSounds\*.*;DestDir:{userdocs}\{#Name}

[Icons]  
Name: {group}\{#Name}; Filename: {app}\{#ExeName}; WorkingDir: {app}; IconFilename: {app}\clock.ico;

[Registry]

[Run]

[Languages]

[ThirdPartySettings]
CompileLogMethod=append

[Code]

[InnoIDE_Settings]
LogFileOverwrite=false
#expr SaveToFile("debug.iss")
