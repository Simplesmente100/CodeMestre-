using CivilTopoPlugin.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace CivilTopoPlugin.Export;

public class CsvExportService
{
    public void Exportar(string caminho, DadosImovel dados, List<Vertice> vertices)
    {
        using var sw = new StreamWriter(caminho, false, new UTF8Encoding(true));
        sw.WriteLine($"# Imovel: {dados.NomeImovel}");
        sw.WriteLine($"# Proprietario: {dados.NomeProprietario}");
        var cfg = new CsvConfiguration(new CultureInfo("pt-BR")) { Delimiter = ";" };
        using var csv = new CsvWriter(sw, cfg);
        csv.WriteRecords(vertices.Select(v => new { v.Id, v.Este, v.Norte, v.Altitude, v.AzimuteFormatado, v.Distancia, v.Confrontante }));
    }
}
