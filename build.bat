@echo Off

echo BUILD.BAT - NuGet package restore started.
".\src\.nuget\NuGet.exe" "install" "FAKE" -Version "4.64.12" -OutputDirectory ".\tools"
".\src\.nuget\NuGet.exe" "install" "NUnit.Console" -Version "3.8.0" -OutputDirectory ".\tools"
".\src\.nuget\NuGet.exe" "restore" ".\src\OneCog.Io.Onkyo.sln" -OutputDirectory ".\src\packages"
echo BUILD.BAT - NuGet package restore finished.

echo BUILD.BAT - FAKE build started.
".\tools\FAKE.4.64.12\tools\Fake.exe" build.fsx
echo BUILD.BAT - FAKE build finished.