set username=QUIDGEST
set password=ZPH2LAB

::Restore all the NuGet packages to prevent erros in msbuild
dotnet restore

::Navigate into project folder
cd AdminCLI

::Build project
WHERE msbuild >nul 2>nul
IF %ERRORLEVEL% EQU 0 (
msbuild AdminCLI.csproj /p:Configuration=Release
) ELSE (
\\jenkinsvm\Share\BuildTools\MSBuild\Current\Bin\msbuild AdminCLI.csproj /p:Configuration=Release
)

::Move into the release folder
cd .\bin\Release\net8.0

::Reindex the database with default credentials
.\AdminCLI.exe reindex -u %username% -p %password%

PAUSE