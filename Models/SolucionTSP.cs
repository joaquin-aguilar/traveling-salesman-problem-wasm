namespace traveling_salesman_problem_wasm.Models;

internal class SolucionTSP
{
    public List<int> Ruta { get; }
    public int CostoTotal { get; }

    public SolucionTSP(ProblemaTSP problema, List<int> ruta, int costoTotal)
    {
        ArgumentNullException.ThrowIfNull(problema);
        ArgumentNullException.ThrowIfNull(ruta);

        int n = problema.NumeroVertices;

        if (ruta.Count != n + 1)
            throw new ArgumentException("La ruta debe contener todos los vértices y volver al inicio.", nameof(ruta));

        if (ruta[0] != problema.VerticeInicial || ruta[^1] != problema.VerticeInicial)
            throw new ArgumentException("La ruta debe empezar y terminar en el vértice inicial.", nameof(ruta));

        // Los primeros n vértices deben ser todos distintos y estar en rango.
        if (ruta.Take(n).Distinct().Count() != n || ruta.Any(v => v < 0 || v >= n))
            throw new ArgumentException("La ruta debe visitar cada vértice exactamente una vez.", nameof(ruta));

        for (int i = 0; i < n; i++)
            if (!problema.ExisteArista(ruta[i], ruta[i + 1]))
                throw new ArgumentException($"La ruta usa la arista {ruta[i]} -> {ruta[i + 1]}, que no existe.", nameof(ruta));

        Ruta = ruta;
        CostoTotal = costoTotal;
    }
}
