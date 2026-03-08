using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;

[assembly: CommandClass(typeof(CivilTopoPlugin.Commands.MemorialCommand))]
[assembly: CommandClass(typeof(CivilTopoPlugin.Commands.ExportCommand))]
[assembly: CommandClass(typeof(CivilTopoPlugin.Commands.RelatorioCommand))]
[assembly: CommandClass(typeof(CivilTopoPlugin.Commands.AdvancedCommands))]

namespace CivilTopoPlugin;

public class PluginApp : IExtensionApplication
{
    public void Initialize()
    {
        var civilDoc = CivilApplication.ActiveDocument;
        Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage(
            "\nCivilTopoPlugin carregado. Use: GERAR_MEMORIAL, EXPORTAR_CSV, TABELA_VERTICES, GERAR_RELATORIO, MEMORIAL_COMPLETO, MARCAR_VERTICES, EXPORTAR_KML, EXPORTAR_GEOJSON, VALIDAR_FECHAMENTO, CONVERTER_UTM_GEO");
    }

    public void Terminate() { }
}
