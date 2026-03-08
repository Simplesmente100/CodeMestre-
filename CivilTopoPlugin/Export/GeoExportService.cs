using CivilTopoPlugin.Models;
using System.Globalization;
using System.Text;

namespace CivilTopoPlugin.Export;

public class GeoExportService
{
    public void ExportarKml(string caminho, List<Vertice> vertices)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\"><Document>");
        sb.AppendLine("<Placemark><Polygon><outerBoundaryIs><LinearRing><coordinates>");
        foreach (var v in vertices) sb.AppendLine($"{v.Este.ToString(CultureInfo.InvariantCulture)},{v.Norte.ToString(CultureInfo.InvariantCulture)},0");
        var f = vertices[0];
        sb.AppendLine($"{f.Este.ToString(CultureInfo.InvariantCulture)},{f.Norte.ToString(CultureInfo.InvariantCulture)},0");
        sb.AppendLine("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
        foreach (var v in vertices) sb.AppendLine($"<Placemark><name>{v.Id}</name><Point><coordinates>{v.Este.ToString(CultureInfo.InvariantCulture)},{v.Norte.ToString(CultureInfo.InvariantCulture)},0</coordinates></Point></Placemark>");
        sb.AppendLine("</Document></kml>");
        File.WriteAllText(caminho, sb.ToString(), Encoding.UTF8);
    }

    public void ExportarGeoJson(string caminho, List<Vertice> vertices)
    {
        var coords = string.Join(",", vertices.Select(v => $"[{v.Este.ToString(CultureInfo.InvariantCulture)},{v.Norte.ToString(CultureInfo.InvariantCulture)}]"));
        var first = vertices[0];
        var json = $"{{\"type\":\"FeatureCollection\",\"crs\":{{\"type\":\"name\",\"properties\":{{\"name\":\"EPSG:4674\"}}}},\"features\":[{{\"type\":\"Feature\",\"geometry\":{{\"type\":\"Polygon\",\"coordinates\":[[{coords},[{first.Este.ToString(CultureInfo.InvariantCulture)},{first.Norte.ToString(CultureInfo.InvariantCulture)}]]]}}}}]}}";
        File.WriteAllText(caminho, json, Encoding.UTF8);
    }
}
