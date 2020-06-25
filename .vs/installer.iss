; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "FIFA20 Ultimate Team AutoBuyer"
#define MyAppVersion "1.0"
#define MyAppExeName "FIFA20 Ultimate Team AutoBuyer.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C896D77F-3F9D-4871-B3AE-8FE17C454C8E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
OutputDir=C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\output
OutputBaseFilename=FIFA20 Ultimate Team AutoBuyer Installer
SetupIconFile=C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\FIFA20 Ultimate Team AutoBuyer\coins.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\FIFA20 Ultimate Team AutoBuyer\bin\x86\Release\FIFA20 Ultimate Team AutoBuyer.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\FIFA20 Ultimate Team AutoBuyer\bin\x86\Release\FIFA20 Ultimate Team AutoBuyer.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\FIFA20 Ultimate Team AutoBuyer\bin\x86\Release\FIFA20 Ultimate Team AutoBuyer.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\FIFA20 Ultimate Team AutoBuyer\bin\x86\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\FIFA20 Ultimate Team AutoBuyer\bin\x86\Release\Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\jordan\Desktop\FIFA20 Ultimate Team AutoBuyer\FIFA20 Ultimate Team AutoBuyer\bin\x86\Release\players.json"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

