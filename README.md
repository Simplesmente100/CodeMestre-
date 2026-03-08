# CivilTopoPlugin

Plugin para Autodesk Civil 3D (2022–2025) focado em topografia brasileira, memorial descritivo, validação de poligonal e exportações técnicas.

## Como baixar e instalar

### Opção 1 — Instalador `.exe` (recomendado)
1. Na seção de releases, baixe `CivilTopoPlugin_Installer_<versão>.exe`.
2. Execute como administrador.
3. O instalador copiará automaticamente para:
   - `%ProgramData%\Autodesk\ApplicationPlugins\CivilTopoPlugin.bundle`
4. Abra o Civil 3D e rode `APPLOAD` (se necessário) ou reinicie o Civil 3D.

### Opção 2 — Instalação manual via ZIP
1. Baixe `CivilTopoPlugin_<versão>.zip`.
2. Extraia a pasta `CivilTopoPlugin.bundle` em:
   - `%ProgramData%\Autodesk\ApplicationPlugins\`
3. Reinicie o Civil 3D.

## Comandos disponíveis
- `GERAR_MEMORIAL`
- `EXPORTAR_CSV`
- `TABELA_VERTICES`
- `GERAR_RELATORIO`
- `MEMORIAL_COMPLETO`
- `MARCAR_VERTICES`
- `EXPORTAR_KML`
- `EXPORTAR_GEOJSON`
- `VALIDAR_FECHAMENTO`
- `CONVERTER_UTM_GEO`

## Gerar pacote e instalador (para distribuição)

### Pré-requisitos
- DLL compilada: `CivilTopoPlugin.dll`
- PowerShell 5+
- Inno Setup 6 (para gerar `.exe`)

### Gerar `.bundle` + `.zip`
```powershell
cd CivilTopoPlugin\Installer
.\Build-Package.ps1 -PluginDllPath "C:\caminho\bin\Release\CivilTopoPlugin.dll" -Version "1.0.0"
```
Saída em: `CivilTopoPlugin/artifacts/`

### Gerar instalador `.exe`
```powershell
cd CivilTopoPlugin\Installer
.\Build-Installer.ps1 -PluginDllPath "C:\caminho\bin\Release\CivilTopoPlugin.dll" -Version "1.0.0"
```
Saída em: `CivilTopoPlugin/artifacts/`


## Automação de release com GitHub Actions

Foi adicionado o workflow `.github/workflows/release-installer.yml` para gerar automaticamente os artefatos de distribuição:
- `CivilTopoPlugin_<versão>.zip`
- `CivilTopoPlugin_Installer_<versão>.exe`

### Como dispara
- **Automático:** ao publicar uma Release no GitHub.
- **Manual:** em *Actions* → *Build Installer (ZIP + EXE)* → *Run workflow*.

### Observação importante
O runner padrão do GitHub pode não possuir as DLLs do Civil 3D necessárias para compilar o plugin. Se isso ocorrer, o job falha na etapa de build da DLL e exibe mensagem clara no log.
