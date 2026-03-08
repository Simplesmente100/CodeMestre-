using Autodesk.AutoCAD.Geometry;
using CivilTopoPlugin.Geometry;
using CivilTopoPlugin.Models;

namespace CivilTopoPlugin.Tests;

public class GeometryEngineTests
{
    [Fact]
    public void Distancia_345() => Assert.Equal(5, GeometryEngine.CalcularDistancia(0, 0, 3, 4), 6);

    [Fact]
    public void Azimutes_Cardinais()
    {
        Assert.Equal(0, GeometryEngine.CalcularAzimuteDec(0, 0, 0, 1), 6);
        Assert.Equal(90, GeometryEngine.CalcularAzimuteDec(0, 0, 1, 0), 6);
        Assert.Equal(180, GeometryEngine.CalcularAzimuteDec(0, 0, 0, -1), 6);
        Assert.Equal(270, GeometryEngine.CalcularAzimuteDec(0, 0, -1, 0), 6);
    }

    [Fact]
    public void Dms_Formatado() => Assert.Equal("132°45'12\"", GeometryEngine.AzimutePara_DMS(132.753333));

    [Fact]
    public void Area_Gauss()
    {
        var quad = new List<Vertice> { new() { Este = 0, Norte = 0 }, new() { Este = 10, Norte = 0 }, new() { Este = 10, Norte = 10 }, new() { Este = 0, Norte = 10 } };
        Assert.Equal(100, GeometryEngine.CalcularAreaGauss(quad), 6);
        var tri = new List<Vertice> { new() { Este = 0, Norte = 0 }, new() { Este = 6, Norte = 0 }, new() { Este = 0, Norte = 4 } };
        Assert.Equal(12, GeometryEngine.CalcularAreaGauss(tri), 6);
    }

    [Fact]
    public void ProcessarPoligonal_Quadrado()
    {
        var pts = new List<Point3d> { new(0, 0, 0), new(10, 0, 0), new(10, 10, 0), new(0, 10, 0) };
        var r = GeometryEngine.ProcessarPoligonal(pts);
        Assert.Equal("V01", r[0].Id);
        Assert.Equal(10, r[0].Distancia, 6);
        Assert.Equal(40, r.Sum(x => x.Distancia), 6);
    }

    [Fact]
    public void FormatarArea_M2_Ha()
    {
        Assert.Contains("m²", GeometryEngine.FormatarArea(9999));
        Assert.Contains("ha", GeometryEngine.FormatarArea(10000));
    }
}
