using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using CivilTopoPlugin.Civil3D;
using CivilTopoPlugin.Export;
using CivilTopoPlugin.Generators;
using CivilTopoPlugin.Geometry;
using CivilTopoPlugin.Services;

namespace CivilTopoPlugin.Commands;

public class AdvancedCommands
{
    [CommandMethod("MEMORIAL_COMPLETO")]
    public void MemorialCompleto() { new MemorialCommand().GerarMemorial(); }

    [CommandMethod("MARCAR_VERTICES")]
    public void MarcarVertices()
    {
        var reader = new PolylineReaderService();
        var db = CivilApplication.ActiveDocument.Database;
        var id = reader.SolicitarSelecaoPolilinha(); if (id.IsNull) return;
        using var tr = db.TransactionManager.StartTransaction();
        var verts = GeometryEngine.ProcessarPoligonal(reader.ExtrairVertices((Entity)tr.GetObject(id, OpenMode.ForRead), tr));
        new VertexMarkerDrawer().Desenhar(db, tr, verts);
        tr.Commit();
    }

    [CommandMethod("EXPORTAR_KML")]
    public void ExportarKml() => ExportarGeo(true);

    [CommandMethod("EXPORTAR_GEOJSON")]
    public void ExportarGeoJson() => ExportarGeo(false);

    [CommandMethod("VALIDAR_FECHAMENTO")]
    public void ValidarFechamento()
    {
        var reader = new PolylineReaderService();
        var db = CivilApplication.ActiveDocument.Database;
        var id = reader.SolicitarSelecaoPolilinha(); if (id.IsNull) return;
        using var tr = db.TransactionManager.StartTransaction();
        var verts = GeometryEngine.ProcessarPoligonal(reader.ExtrairVertices((Entity)tr.GetObject(id, OpenMode.ForRead), tr));
        var r = PoligonalValidator.Validar(verts);
        new LogService().Info($"Erro linear={r.erroLinear:F4}m | 1:{r.relacao:F0} | Classe={r.classe}");
        tr.Commit();
    }

    [CommandMethod("CONVERTER_UTM_GEO")]
    public void ConverterUtmGeo()
    {
        var reader = new PolylineReaderService();
        var e = double.Parse(reader.SolicitarTexto("Este"));
        var n = double.Parse(reader.SolicitarTexto("Norte"));
        var f = int.Parse(reader.SolicitarTexto("Fuso"));
        var r = CoordinateConverter.UtmParaGeografico(e, n, f, true);
        new LogService().Info($"Lat={r.Latitude:F8}, Lon={r.Longitude:F8}");
    }

    private void ExportarGeo(bool kml)
    {
        var reader = new PolylineReaderService();
        var db = CivilApplication.ActiveDocument.Database;
        var id = reader.SolicitarSelecaoPolilinha(); if (id.IsNull) return;
        using var tr = db.TransactionManager.StartTransaction();
        var verts = GeometryEngine.ProcessarPoligonal(reader.ExtrairVertices((Entity)tr.GetObject(id, OpenMode.ForRead), tr));
        var nome = reader.SolicitarTexto("Nome do imóvel");
        var pasta = PathHelper.CriarPasta(nome);
        var geo = new GeoExportService();
        if (kml) geo.ExportarKml(Path.Combine(pasta, $"kml_{nome}_{DateTime.Now:yyyyMMdd_HHmm}.kml"), verts);
        else geo.ExportarGeoJson(Path.Combine(pasta, $"geojson_{nome}_{DateTime.Now:yyyyMMdd_HHmm}.geojson"), verts);
        tr.Commit();
    }
}
