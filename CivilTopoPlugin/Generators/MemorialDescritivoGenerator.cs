using CivilTopoPlugin.Models;
using System.Globalization;
using System.Text;

namespace CivilTopoPlugin.Generators;

public class MemorialDescritivoGenerator
{
    public string Gerar(DadosImovel dados, List<Vertice> vertices)
    {
        var sb = new StringBuilder();
        var c = new CultureInfo("pt-BR");
        sb.AppendLine("MEMORIAL DESCRITIVO");
        sb.AppendLine($"Imóvel: {dados.NomeImovel}");
        sb.AppendLine($"Proprietário: {dados.NomeProprietario}");
        sb.AppendLine($"Município/UF: {dados.Municipio}/{dados.Estado}");
        sb.AppendLine();
        for (int i = 0; i < vertices.Count; i++)
        {
            var v = vertices[i];
            var prox = vertices[(i + 1) % vertices.Count];
            sb.AppendLine($"Do vértice {v.Id} segue com azimute de {v.AzimuteFormatado} e distância de {v.Distancia.ToString("N2", c)} m até o vértice {prox.Id}, confrontando com {v.Confrontante}.");
        }
        sb.AppendLine("\nTabela de Vértices:");
        sb.AppendLine("ID;Este;Norte;Altitude");
        foreach (var v in vertices) sb.AppendLine($"{v.Id};{v.Este.ToString("N3", c)};{v.Norte.ToString("N3", c)};{v.Altitude.ToString("N3", c)}");
        sb.AppendLine($"\nResponsável Técnico: {dados.NomeResponsavel} - CREA: {dados.CreaResponsavel}");
        return sb.ToString();
    }
}
