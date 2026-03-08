using CivilTopoPlugin.Models;

namespace CivilTopoPlugin.Geometry;

public static class PoligonalValidator
{
    public static (double erroLinear, double relacao, string classe) Validar(List<Vertice> vertices)
    {
        if (vertices.Count < 3) return (0, 0, "Insuficiente");
        double sumE = 0, sumN = 0, perimetro = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            var a = vertices[i];
            var b = vertices[(i + 1) % vertices.Count];
            var dE = b.Este - a.Este;
            var dN = b.Norte - a.Norte;
            sumE += dE; sumN += dN;
            perimetro += Math.Sqrt(dE * dE + dN * dN);
        }
        var erro = Math.Sqrt(sumE * sumE + sumN * sumN);
        var rel = erro == 0 ? double.PositiveInfinity : perimetro / erro;
        var classe = rel >= 10000 ? "Excellent" : rel >= 5000 ? "Good" : rel >= 3000 ? "Acceptable" : "Poor";
        return (erro, rel, classe);
    }

    public static List<Vertice> AplicarBowditch(List<Vertice> vertices)
    {
        var result = vertices.Select(v => new Vertice
        {
            Id = v.Id,
            Este = v.Este,
            Norte = v.Norte,
            Altitude = v.Altitude,
            Confrontante = v.Confrontante
        }).ToList();

        double erroE = 0, erroN = 0, per = 0;
        var lados = new List<double>();
        for (int i = 0; i < result.Count; i++)
        {
            var a = result[i]; var b = result[(i + 1) % result.Count];
            var dE = b.Este - a.Este; var dN = b.Norte - a.Norte;
            erroE += dE; erroN += dN;
            var l = Math.Sqrt(dE * dE + dN * dN);
            lados.Add(l); per += l;
        }

        double acum = 0;
        for (int i = 0; i < result.Count; i++)
        {
            acum += lados[i == 0 ? result.Count - 1 : i - 1];
            var corrE = -erroE * (acum / per);
            var corrN = -erroN * (acum / per);
            result[i].Este += corrE;
            result[i].Norte += corrN;
        }
        return result;
    }
}
