param([switch]$Locked)
$ErrorActionPreference='Stop'
$restoreArgs=@()
if($Locked){$restoreArgs+='--locked-mode'}
foreach($project in @('desktop/BD2Sichuan.Desktop.csproj','tests/BD2Sichuan.Tests.csproj','compatibility-tests/BD2Sichuan.Compatibility.Tests.csproj')){
    & dotnet restore (Join-Path $PSScriptRoot $project) @restoreArgs --nologo
    if($LASTEXITCODE -ne 0){throw "Restore failed: $project"}
}
& dotnet build (Join-Path $PSScriptRoot 'desktop/BD2Sichuan.Desktop.csproj') -c Release --no-restore --nologo -v minimal
if($LASTEXITCODE -ne 0){throw 'Build failed'}
& dotnet run --project (Join-Path $PSScriptRoot 'tests/BD2Sichuan.Tests.csproj') -c Release --no-restore
if($LASTEXITCODE -ne 0){throw 'Sichuan regression failed'}
& dotnet run --project (Join-Path $PSScriptRoot 'compatibility-tests/BD2Sichuan.Compatibility.Tests.csproj') -c Release --no-restore
if($LASTEXITCODE -ne 0){throw 'Compatibility regression failed'}
