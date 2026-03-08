using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;

namespace CivilTopoPlugin.Services;

public class PolylineReaderService
{
    public ObjectId SolicitarSelecaoPolilinha()
    {
        var ed = Application.DocumentManager.MdiActiveDocument.Editor;
        var opts = new PromptEntityOptions("\nSelecione polilinha: ");
        opts.AddAllowedClass(typeof(Polyline), true);
        opts.AddAllowedClass(typeof(Polyline2d), true);
        opts.AddAllowedClass(typeof(Polyline3d), true);
        var res = ed.GetEntity(opts);
        return res.Status == PromptStatus.OK ? res.ObjectId : ObjectId.Null;
    }

    public List<Point3d> ExtrairVertices(Entity ent, Transaction tr)
    {
        var pts = new List<Point3d>();
        if (ent is Polyline pl)
            for (int i = 0; i < pl.NumberOfVertices; i++) pts.Add(pl.GetPoint3dAt(i));
        else if (ent is Polyline2d pl2)
            foreach (ObjectId id in pl2) pts.Add(((Vertex2d)tr.GetObject(id, OpenMode.ForRead)).Position);
        else if (ent is Polyline3d pl3)
            foreach (ObjectId id in pl3) pts.Add(((PolylineVertex3d)tr.GetObject(id, OpenMode.ForRead)).Position);
        return pts;
    }

    public Point3d SolicitarPontoInsercao()
    {
        var ed = Application.DocumentManager.MdiActiveDocument.Editor;
        var res = ed.GetPoint("\nPonto de inserção: ");
        return res.Status == PromptStatus.OK ? res.Value : Point3d.Origin;
    }

    public string SolicitarTexto(string mensagem)
    {
        var ed = Application.DocumentManager.MdiActiveDocument.Editor;
        var res = ed.GetString($"\n{mensagem}: ");
        return res.Status == PromptStatus.OK ? res.StringResult : string.Empty;
    }
}
