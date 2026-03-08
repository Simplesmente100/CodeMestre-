using Autodesk.AutoCAD.Geometry;
using CivilTopoPlugin.Models;
using System.Globalization;

namespace CivilTopoPlugin.Geometry;

public static class GeometryEngine
{
    public static double CalcularDistancia(double e1, double n1, double e2, double n2)
        => Math.Sqrt(Math.Pow(e2 - e1, 2) + Math.Pow(n2 - n1, 2));

    public static double CalcularAzimuteDec(double e1, double n1, double e2, double n2)
    {
        var ang = Math.Atan2(e2 - e1, n2 - n1) * 180.0 / Math.PI;
        return (ang + 360.0) % 360.0;
    }

    public static string AzimutePara_DMS(double grausDecimais)
    {
        var g = (int)Math.Floor(grausDecimais);
        var mFloat = (grausDecimais - g) * 60.0;
        var m = (int)Math.Floor(mFloat);
        var s = (int)Math.Round((mFloat - m) * 60.0);
        if (s == 60) { s = 0; m++; }
        if (m == 60) { m = 0; g++; }
        return $"{g:000}°{m:00}'{s:00}\"";
    }

    public static List<Vertice> ProcessarPoligonal(List<Point3d> pontos)
    {
        var vertices = new List<Vertice>();
        if (pontos.Count < 2) return vertices;
        double perAc = 0;
        for (int i = 0; i < pontos.Count; i++)
        {
            var atual = pontos[i];
            var prox = pontos[(i + 1) % pontos.Count];
            var dist = CalcularDistancia(atual.X, atual.Y, prox.X, prox.Y);
            var azi = CalcularAzimuteDec(atual.X, atual.Y, prox.X, prox.Y);
            perAc += dist;
            vertices.Add(new Vertice
            {
                Id = $"V{i + 1:00}",
                Este = atual.X,
                Norte = atual.Y,
                Altitude = atual.Z,
                Distancia = dist,
                AzimuteDec = azi,
                AzimuteFormatado = AzimutePara_DMS(azi),
                PerimetroAcumulado = perAc
            });
        }
        return vertices;
    }

    public static double CalcularAreaGauss(List<Vertice> vertices)
    {
        if (vertices.Count < 3) return 0;
        double soma = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            var a = vertices[i];
            var b = vertices[(i + 1) % vertices.Count];
            soma += (a.Este * b.Norte) - (b.Este * a.Norte);
        }
        return Math.Abs(soma) / 2.0;
    }

    public static string FormatarArea(double areaM2)
    {
        var cultura = new CultureInfo("pt-BR");
        if (areaM2 >= 10000)
            return $"{(areaM2 / 10000.0).ToString("N4", cultura)} ha ({areaM2.ToString("N2", cultura)} m²)";
        return $"{areaM2.ToString("N2", cultura)} m²";
    }
}
