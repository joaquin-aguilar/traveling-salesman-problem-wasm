namespace traveling_salesman_problem_wasm.Models;

public class CiudadOSM
{
    public string Nombre { get; }
    public double Latitud { get; }
    public double Longitud { get; }

    public CiudadOSM(string nombre, double latitud, double longitud)
    {
        Nombre = nombre;
        Latitud = latitud;
        Longitud = longitud;
    }
}
