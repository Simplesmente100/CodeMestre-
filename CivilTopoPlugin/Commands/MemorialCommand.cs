using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using CivilTopoPlugin.Generators;
using CivilTopoPlugin.Geometry;
using CivilTopoPlugin.Models;
using CivilTopoPlugin.Services;

namespace CivilTopoPlugin.Commands;

public class MemorialCommand
{
    private readonly PolylineReaderService _reader = new();
    private readonly MemorialDescritivoGenerator _generator = new();
    private readonly TabelaAutoCADGenerator _tabela = new();

    [CommandMethod("GERAR_MEMORIAL")]
    public void GerarMemorial()
    {
        var civilDoc = CivilApplication.ActiveDocument;
        var db = civilDoc.Database;
        var id = _reader.SolicitarSelecaoPolilinha();
        if (id.IsNull) return;
        var dados = ColetarDados();
        using var tr = db.TransactionManager.StartTransaction();
        var ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
        var verts = GeometryEngine.ProcessarPoligonal(_reader.ExtrairVertices(ent, tr));
        dados.AreaCalculada = GeometryEngine.CalcularAreaGauss(verts);
        dados.Perimetro = verts.Sum(v => v.Distancia);
        var txt = _generator.Gerar(dados, verts);
        var pasta = PathHelper.CriarPasta(dados.NomeImovel);
        File.WriteAllText(Path.Combine(pasta, $"memorial_{dados.NomeImovel}_{DateTime.Now:yyyyMMdd_HHmm}.txt"), txt);
        _tabela.InserirTabela(db, _reader.SolicitarPontoInsercao(), verts, tr);
        tr.Commit();
    }

    private DadosImovel ColetarDados() => new()
    {
        NomeImovel = _reader.SolicitarTexto("Nome do imóvel"),
        NomeProprietario = _reader.SolicitarTexto("Nome do proprietário"),
        Municipio = _reader.SolicitarTexto("Município"),
        Estado = _reader.SolicitarTexto("UF"),
        NomeResponsavel = _reader.SolicitarTexto("Responsável técnico"),
        CreaResponsavel = _reader.SolicitarTexto("CREA")
    };
}

internal static class PathHelper
{
    public static string CriarPasta(string nomeImovel)
    {
        var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CivilTopo", nomeImovel);
        Directory.CreateDirectory(basePath);
        return basePath;
    }
}
