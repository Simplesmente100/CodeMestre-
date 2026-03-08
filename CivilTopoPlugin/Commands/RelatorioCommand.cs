using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using CivilTopoPlugin.Generators;
using CivilTopoPlugin.Geometry;
using CivilTopoPlugin.Models;
using CivilTopoPlugin.Services;

namespace CivilTopoPlugin.Commands;

public class RelatorioCommand
{
    [CommandMethod("GERAR_RELATORIO")]
    public void GerarRelatorio()
    {
        var reader = new PolylineReaderService();
        var db = CivilApplication.ActiveDocument.Database;
        var id = reader.SolicitarSelecaoPolilinha(); if (id.IsNull) return;
        using var tr = db.TransactionManager.StartTransaction();
        var verts = GeometryEngine.ProcessarPoligonal(reader.ExtrairVertices((Entity)tr.GetObject(id, OpenMode.ForRead), tr));
        var dados = new DadosImovel { NomeImovel = reader.SolicitarTexto("Nome do imóvel"), AreaCalculada = GeometryEngine.CalcularAreaGauss(verts), Perimetro = verts.Sum(v => v.Distancia) };
        var txt = new RelatorioLevantamentoGenerator().Gerar(dados, verts);
        var pasta = PathHelper.CriarPasta(dados.NomeImovel);
        File.WriteAllText(Path.Combine(pasta, $"relatorio_{dados.NomeImovel}_{DateTime.Now:yyyyMMdd_HHmm}.txt"), txt);
        tr.Commit();
    }
}
