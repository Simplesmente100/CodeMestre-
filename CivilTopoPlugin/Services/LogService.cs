using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.ApplicationServices;

namespace CivilTopoPlugin.Services;

public class LogService
{
    private readonly string _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CivilTopoPlugin", "plugin.log");

    public void Info(string msg)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);
        File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}{Environment.NewLine}");
        Application.DocumentManager.MdiActiveDocument?.Editor.WriteMessage($"\n[Topo] {msg}");
    }
}
