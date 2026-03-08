using CivilTopoPlugin.Geometry;
using CivilTopoPlugin.Models;
using System.Globalization;
using System.Text;

namespace CivilTopoPlugin.Generators;

public class RelatorioLevantamentoGenerator
{
    public string Gerar(DadosImovel dados, List<Vertice> vertices)
    {
        var v = PoligonalValidator.Validar(vertices);
        var c = new CultureInfo("pt-BR");
        var sb = new StringBuilder();
        sb.AppendLine("RELATÓRIO TÉCNICO DE LEVANTAMENTO");
        sb.AppendLine($"Imóvel: {dados.NomeImovel}");
        sb.AppendLine($"Área: {dados.AreaCalculada.ToString("N2", c)} m²");
        sb.AppendLine($"Perímetro: {dados.Perimetro.ToString("N2", c)} m");
        sb.AppendLine($"Erro linear: {v.erroLinear.ToString("N4", c)} m");
        sb.AppendLine($"Relação de fechamento: 1:{v.relacao.ToString("N0", c)} ({v.classe})");
        sb.AppendLine("Declaro responsabilidade técnica sobre os dados acima. ART: ___________");
        return sb.ToString();
    }
}
