@echo Off

set nuget=
if "%nuget%" == "" (
    set nuget=nuget
)

echo BUILD.BAT - NuGet package restore started.
"%nuget%" restore ".\src\OneCog.Io.Onkyo.sln" -OutputDirectory ".\src\packages"
echo BUILD.BAT - NuGet package restore finished.

echo BUILD.BAT - FAKE build started.
".\src\packages\FAKE.4.1.3\tools\Fake.exe" build.fsx
echo BUILD.BAT - FAKE build finished.