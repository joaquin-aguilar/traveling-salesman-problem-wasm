namespace traveling_salesman_problem_wasm.Algorithms.Utilidades;

internal static class CalculosGeograficos
{
    private const double RadioTierraKm = 6371.0088;

    /// <summary>
    /// Distancia de círculo máximo entre dos coordenadas (fórmula de Haversine).
    /// </summary>
    public static double DistanciaKm(double latitud1, double longitud1, double latitud2, double longitud2)
    {
        double latitud1Radianes = GradosARadianes(latitud1);
        double latitud2Radianes = GradosARadianes(latitud2);
        double diferenciaLatitud = GradosARadianes(latitud2 - latitud1);
        double diferenciaLongitud = GradosARadianes(longitud2 - longitud1);

        double a = Math.Pow(Math.Sin(diferenciaLatitud / 2), 2)
            + Math.Cos(latitud1Radianes) * Math.Cos(latitud2Radianes)
            * Math.Pow(Math.Sin(diferenciaLongitud / 2), 2);

        double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

        return RadioTierraKm * c;
    }

    private static double GradosARadianes(double grados)
        => grados * Math.PI / 180.0;
}
