[Setup]
AppName=Moser's Game of Life
AppVersion=1.0
DefaultDirName={autopf}\MoserLife
DefaultGroupName=Moser's Game of Life
OutputDir=.
OutputBaseFilename=MoserGameOfLifeSetup
Compression=lzma
SolidCompression=yes

[Files]
Source: "D:\Repositories\MosersGameOfLife\MosersGameOfLife\bin\Release\net8.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs


[Icons]
Name: "{autoprograms}\Moser's Game of Life"; Filename: "{app}\MosersGameOfLife.exe"
Name: "{autodesktop}\Moser's Game of Life"; Filename: "{app}\MosersGameOfLife.exe"

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
