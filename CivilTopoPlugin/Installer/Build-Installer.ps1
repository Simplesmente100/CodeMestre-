param(
    [Parameter(Mandatory = $true)]
    [string]$PluginDllPath,
    [string]$Version = "1.0.0",
    [string]$IsccPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "[1/2] Gerando pacote .bundle..."
& (Join-Path $scriptDir "Build-Package.ps1") -PluginDllPath $PluginDllPath -Version $Version
if ($LASTEXITCODE -ne 0) { throw "Falha ao gerar pacote .bundle" }

if (-not (Test-Path $IsccPath)) {
    throw "ISCC.exe não encontrado em '$IsccPath'. Instale o Inno Setup 6 ou informe -IsccPath."
}

Write-Host "[2/2] Compilando instalador Inno Setup..."
& $IsccPath (Join-Path $scriptDir "CivilTopoPlugin.iss")
if ($LASTEXITCODE -ne 0) { throw "Falha ao compilar instalador" }

Write-Host "Instalador gerado em CivilTopoPlugin/artifacts"
