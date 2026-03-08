namespace CivilTopoPlugin.Services;

public class ConfrontanteService
{
    private readonly PolylineReaderService _reader = new();
    public string SolicitarConfrontante(int indice)
    {
        var txt = _reader.SolicitarTexto($"Confrontante lado {indice + 1} (1=Remanescente,2=Via pública,3=Rio,4=Particular,5=APP)");
        return txt switch
        {
            "1" => "Remanescente",
            "2" => "Via pública",
            "3" => "Rio",
            "4" => "Particular",
            "5" => "APP",
            _ => txt
        };
    }
}
