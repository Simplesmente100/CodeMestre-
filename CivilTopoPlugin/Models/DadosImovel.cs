namespace CivilTopoPlugin.Models;

public class DadosImovel
{
    public string NomeImovel { get; set; } = string.Empty;
    public string NomeProprietario { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string CartorioRegistro { get; set; } = string.Empty;
    public string SistemaReferencia { get; set; } = "SIRGAS 2000";
    public int Fuso { get; set; }
    public string NomeResponsavel { get; set; } = string.Empty;
    public string CreaResponsavel { get; set; } = string.Empty;
    public double AreaCalculada { get; set; }
    public double Perimetro { get; set; }
}
