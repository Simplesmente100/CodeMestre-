namespace CivilTopoPlugin.Models;

public class Vertice
{
    public string Id { get; set; } = string.Empty;
    public double Este { get; set; }
    public double Norte { get; set; }
    public double Altitude { get; set; }
    public double AzimuteDec { get; set; }
    public string AzimuteFormatado { get; set; } = string.Empty;
    public double Distancia { get; set; }
    public double PerimetroAcumulado { get; set; }
    public string Confrontante { get; set; } = string.Empty;
}
