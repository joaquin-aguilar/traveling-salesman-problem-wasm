using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using traveling_salesman_problem_wasm.Models;

namespace traveling_salesman_problem_wasm.Services;

internal class ServicioRutasOsrm
{
    private const string UrlBase = "https://router.project-osrm.org";
    private readonly HttpClient _http;
    public ServicioRutasOsrm(HttpClient http)
    {
        _http = http;
    }

    public async Task<int[][]> ObtenerMatrizDistanciasKmAsync(IReadOnlyList<CiudadOSM> ciudades)
    {
        string url = $"{UrlBase}/table/v1/driving/{FormatearCoordenadas(ciudades)}?annotations=distance";
        var respuesta = await _http.GetFromJsonAsync<RespuestaTabla>(url);

        if (respuesta?.Code != "Ok" || respuesta.Distances is null)
            throw new InvalidOperationException("El servicio de rutas no devolvió distancias.");

        int n = ciudades.Count;
        int[][] matriz = Enumerable.Range(0, n).Select(_ => new int[n]).ToArray();

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                double? ida = respuesta.Distances[i][j];
                double? vuelta = respuesta.Distances[j][i];

                if (ida is null || vuelta is null)
                    throw new InvalidOperationException($"No hay ruta por carretera entre {ciudades[i].Nombre} y {ciudades[j].Nombre}.");

                int km = Math.Max(1, (int)Math.Round((ida.Value + vuelta.Value) / 2 / 1000));
                matriz[i][j] = km;
                matriz[j][i] = km;
            }
        }

        return matriz;
    }
    public async Task<double[][]> ObtenerTrazadoAsync(IReadOnlyList<CiudadOSM> ciudades, IReadOnlyList<int> ruta)
    {
        var paradas = ruta.Select(indice => ciudades[indice]).ToList();
        string url = $"{UrlBase}/route/v1/driving/{FormatearCoordenadas(paradas)}?overview=full&geometries=geojson";

        var respuesta = await _http.GetFromJsonAsync<RespuestaRuta>(url);
        var coordenadas = respuesta?.Routes?.FirstOrDefault()?.Geometry?.Coordinates;

        if (respuesta?.Code != "Ok" || coordenadas is null)
            throw new InvalidOperationException("El servicio de rutas no devolvió el trazado.");

        return coordenadas.Select(punto => new[] { punto[1], punto[0] }).ToArray();
    }

    private static string FormatearCoordenadas(IEnumerable<CiudadOSM> ciudades) 
        => string.Join(";", ciudades.Select(c 
        => string.Create(CultureInfo.InvariantCulture, $"{c.Longitud:F6},{c.Latitud:F6}")));

    private sealed class RespuestaTabla
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
        [JsonPropertyName("distances")]
        public double?[][]? Distances { get; set; }
    }

    private sealed class RespuestaRuta
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
        [JsonPropertyName("routes")]
        public List<RutaOsrm>? Routes { get; set; }
    }
    
    private sealed class RutaOsrm
    {
        [JsonPropertyName("geometry")]
        public GeometriaOsrm? Geometry { get; set; }
    }

    private sealed class GeometriaOsrm
    {
        [JsonPropertyName("coordinates")]
        public double[][]? Coordinates { get; set; }
    }
}
