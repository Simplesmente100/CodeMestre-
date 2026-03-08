using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using CivilTopoPlugin.Export;
using CivilTopoPlugin.Generators;
using CivilTopoPlugin.Geometry;
using CivilTopoPlugin.Models;
using CivilTopoPlugin.Services;

namespace CivilTopoPlugin.Commands;

public class ExportCommand
{
    private readonly PolylineReaderService _reader = new();

    [CommandMethod("EXPORTAR_CSV")]
    public void ExportarCsv()
    {
        var db = CivilApplication.ActiveDocument.Database;
        var id = _reader.SolicitarSelecaoPolilinha(); if (id.IsNull) return;
        using var tr = db.TransactionManager.StartTransaction();
        var verts = GeometryEngine.ProcessarPoligonal(_reader.ExtrairVertices((Entity)tr.GetObject(id, OpenMode.ForRead), tr));
        var dados = new DadosImovel { NomeImovel = _reader.SolicitarTexto("Nome do imóvel") };
        var pasta = PathHelper.CriarPasta(dados.NomeImovel);
        new CsvExportService().Exportar(Path.Combine(pasta, $"csv_{dados.NomeImovel}_{DateTime.Now:yyyyMMdd_HHmm}.csv"), dados, verts);
        tr.Commit();
    }

    [CommandMethod("TABELA_VERTICES")]
    public void TabelaVertices()
    {
        var db = CivilApplication.ActiveDocument.Database;
        var id = _reader.SolicitarSelecaoPolilinha(); if (id.IsNull) return;
        using var tr = db.TransactionManager.StartTransaction();
        var verts = GeometryEngine.ProcessarPoligonal(_reader.ExtrairVertices((Entity)tr.GetObject(id, OpenMode.ForRead), tr));
        new TabelaAutoCADGenerator().InserirTabela(db, _reader.SolicitarPontoInsercao(), verts, tr);
        tr.Commit();
    }
}
