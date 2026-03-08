param(
    [Parameter(Mandatory = $true)]
    [string]$PluginDllPath,
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $PluginDllPath)) {
    throw "DLL não encontrada: $PluginDllPath"
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..")
$templateDir = Join-Path $projectRoot "BundleTemplate"
$outputRoot = Join-Path $projectRoot "artifacts"
$bundleDir = Join-Path $outputRoot "CivilTopoPlugin.bundle"
$contentsWindows = Join-Path $bundleDir "Contents\Windows"

if (Test-Path $bundleDir) {
    Remove-Item $bundleDir -Recurse -Force
}

New-Item -Path $contentsWindows -ItemType Directory -Force | Out-Null
Copy-Item (Join-Path $templateDir "PackageContents.xml") (Join-Path $bundleDir "PackageContents.xml") -Force
Copy-Item $PluginDllPath (Join-Path $contentsWindows "CivilTopoPlugin.dll") -Force

$deps = @("AeccDbMgd.dll","AeccLandMgd.dll","AcMgd.dll","AcDbMgd.dll","AcCoreMgd.dll")
foreach ($dep in $deps) {
    $depPath = Join-Path (Split-Path -Parent $PluginDllPath) $dep
    if (Test-Path $depPath) {
        Copy-Item $depPath (Join-Path $contentsWindows $dep) -Force
    }
}

# Atualiza versão no PackageContents
$packageXmlPath = Join-Path $bundleDir "PackageContents.xml"
[xml]$xml = Get-Content $packageXmlPath
$xml.ApplicationPackage.AppVersion = $Version
$xml.ApplicationPackage.FriendlyVersion = $Version
$xml.Save($packageXmlPath)

$zipPath = Join-Path $outputRoot ("CivilTopoPlugin_{0}.zip" -f $Version)
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}
Compress-Archive -Path $bundleDir -DestinationPath $zipPath -Force

Write-Host "Pacote gerado: $bundleDir"
Write-Host "ZIP para download: $zipPath"
