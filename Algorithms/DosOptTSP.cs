using traveling_salesman_problem_wasm.Models;

namespace traveling_salesman_problem_wasm.Algorithms;

internal class DosOptTSP : IResolver
{
    private readonly ProblemaTSP _problema;

    public DosOptTSP(ProblemaTSP problema)
    {
        ArgumentNullException.ThrowIfNull(problema);
        _problema = problema;
    }

    public SolucionTSP? Resolver()
    {
        List<int>? ruta = RutaVecinoMasCercano();

        if (ruta is null)
            return null;

        bool mejoro;

        do
        {
            mejoro = false;
            int mejorCosto = Costo(ruta);

            // Se eligen dos aristas y se invierte el tramo que queda entre ellas.
            for (int i = 1; i < ruta.Count - 2; i++)
            {
                for (int j = i + 1; j < ruta.Count - 1; j++)
                {
                    var candidata = new List<int>(ruta);
                    candidata.Reverse(i, j - i + 1);

                    int costo = Costo(candidata);

                    if (costo < mejorCosto)
                    {
                        ruta = candidata;
                        mejorCosto = costo;
                        mejoro = true;
                    }
                }
            }
        }
        while (mejoro);

        return new SolucionTSP(_problema, ruta, Costo(ruta));
    }

    private List<int>? RutaVecinoMasCercano()
    {
        int n = _problema.NumeroVertices;
        int inicio = _problema.VerticeInicial;

        var ruta = new List<int> { inicio };
        var visitados = new bool[n];
        visitados[inicio] = true;

        while (ruta.Count < n)
        {
            int actual = ruta[^1];
            int siguiente = -1;

            for (int candidato = 0; candidato < n; candidato++)
            {
                if (visitados[candidato] || !_problema.ExisteArista(actual, candidato))
                    continue;

                if (siguiente < 0 || _problema.ObtenerPeso(actual, candidato) < _problema.ObtenerPeso(actual, siguiente))
                    siguiente = candidato;
            }

            if (siguiente < 0)
                return null;

            ruta.Add(siguiente);
            visitados[siguiente] = true;
        }

        if (!_problema.ExisteArista(ruta[^1], inicio))
            return null;

        ruta.Add(inicio);
        return ruta;
    }

    private int Costo(List<int> ruta)
    {
        int total = 0;

        for (int i = 0; i < ruta.Count - 1; i++)
        {
            if (!_problema.ExisteArista(ruta[i], ruta[i + 1]))
                return int.MaxValue;

            total += _problema.ObtenerPeso(ruta[i], ruta[i + 1]);
        }

        return total;
    }
}
