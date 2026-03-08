#define MyAppName "CivilTopoPlugin"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "CivilTopoPlugin"
#define MyAppExeName "CivilTopoPlugin.dll"

[Setup]
AppId={{B79D69C6-388F-4EF4-A958-4C0A6BDF7E11}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\artifacts
OutputBaseFilename=CivilTopoPlugin_Installer_{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Files]
Source: "..\artifacts\CivilTopoPlugin.bundle\*"; DestDir: "{commonappdata}\Autodesk\ApplicationPlugins\CivilTopoPlugin.bundle"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{group}\Desinstalar CivilTopoPlugin"; Filename: "{uninstallexe}"

[Run]
Filename: "{cmd}"; Parameters: "/C echo Plugin instalado em: {commonappdata}\Autodesk\ApplicationPlugins\CivilTopoPlugin.bundle"; Flags: runhidden

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
end;
