using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CivilTopoPlugin.Models;

namespace CivilTopoPlugin.Generators;

public class VertexMarkerDrawer
{
    public void Desenhar(Database db, Transaction tr, List<Vertice> vertices)
    {
        CriarLayer(db, tr, "CIVIL_TOPO_VERTICES", 2);
        CriarLayer(db, tr, "CIVIL_TOPO_ANOTACOES", 3);
        CriarLayer(db, tr, "CIVIL_TOPO_LADOS", 1);

        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        var ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

        foreach (var v in vertices)
        {
            var p = new Point3d(v.Este, v.Norte, v.Altitude);
            var c = new Circle(p, Vector3d.ZAxis, 0.5) { Layer = "CIVIL_TOPO_VERTICES" };
            ms.AppendEntity(c); tr.AddNewlyCreatedDBObject(c, true);
            var t = new DBText { Position = p + new Vector3d(0.7, 0.7, 0), Height = 0.7, TextString = v.Id, Layer = "CIVIL_TOPO_ANOTACOES" };
            ms.AppendEntity(t); tr.AddNewlyCreatedDBObject(t, true);
        }
    }

    private static void CriarLayer(Database db, Transaction tr, string nome, short cor)
    {
        var lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
        if (lt.Has(nome)) return;
        lt.UpgradeOpen();
        var ltr = new LayerTableRecord { Name = nome, Color = Color.FromColorIndex(ColorMethod.ByAci, cor) };
        lt.Add(ltr); tr.AddNewlyCreatedDBObject(ltr, true);
    }
}
