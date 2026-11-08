namespace traveling_salesman_problem_wasm.Models;

internal class GrafoPonderado
{
    private static readonly int _grado_minimo = 2;
    private readonly int [,] _matrizPesos;
    public int NumeroVertices {get;}
    public bool EsDirigido { get; }

    public GrafoPonderado(int numeroVertices, bool esDirigido = false)
    {
        if(numeroVertices < GrafoPonderado._grado_minimo)
            throw new ArgumentException($"Se requiere un minimo de {GrafoPonderado._grado_minimo} vertices.");
        
        NumeroVertices = numeroVertices;
        _matrizPesos = new int[NumeroVertices, NumeroVertices];
        EsDirigido = esDirigido;
    }
    public void AsignarPeso(int verticeOrigen, int verticeDestino, int peso)
    {
        if(verticeOrigen == verticeDestino)
            throw new Exception("No se admiten bucles");

        ValidarVertice(verticeOrigen);
        ValidarVertice(verticeDestino);

        _matrizPesos[verticeOrigen, verticeDestino] = peso;

        if(!EsDirigido)
            _matrizPesos[verticeDestino, verticeOrigen] = peso;
    }
    public int ObtenerPeso(int verticeOrigen, int verticeDestino)
    {
        ValidarVertice(verticeOrigen);
        ValidarVertice(verticeDestino);
        return _matrizPesos[verticeOrigen, verticeDestino];
    }
    public bool ExisteArista(int origen, int destino)
    {
        return (origen != destino) && ObtenerPeso(origen, destino) > 0;
    }
    public int GradoSalida(int vertice)
    {
        ValidarVertice(vertice);
        int grado = 0;
        for (int otro = 0; otro < NumeroVertices; otro++)
            if(ExisteArista(vertice, otro))
                grado++;

        return grado;
    }
    public int GradoEntrada(int vertice)
    {
        ValidarVertice(vertice);
        int grado = 0;
        for (int otro = 0; otro < NumeroVertices; otro++)
            if(ExisteArista(otro, vertice))
                grado++;

        return grado;
    }
    public bool EsGrafoValido()
    {
        for(int filas = 0; filas < NumeroVertices; filas++)
        {   
            if(EsDirigido)
                if(GradoSalida(filas) == 0 || GradoEntrada(filas) == 0)
                    return false;

            else if (GradoSalida(filas) < GrafoPonderado._grado_minimo)
                return false;
        }
        return AlcanzaATodos(seguirAristasInversas: false) && (!EsDirigido || AlcanzaATodos(seguirAristasInversas: true));
    }
    public bool LimitarGrado(int gradoMaximo)
    {
        if (gradoMaximo < GrafoPonderado._grado_minimo)
            throw new ArgumentOutOfRangeException(nameof(gradoMaximo), "El grado máximo debe ser al menos 2.");

        if (gradoMaximo >= NumeroVertices - 1)
            return true;

        List<int>? ciclo = BuscarCicloHamiltoniano();

        if (ciclo is null)
            return false;

        int[,] pesosOriginales = (int[,])_matrizPesos.Clone();
        Array.Clear(_matrizPesos);

        for (int i = 0; i < ciclo.Count; i++)
        {
            int origen = ciclo[i];
            int destino = ciclo[(i + 1) % ciclo.Count];
            AsignarPeso(origen, destino, pesosOriginales[origen, destino]);
        }
        var aristas = new List<(int Origen, int Destino, int Peso)>();

        for (int origen = 0; origen < NumeroVertices; origen++)
        {
            for (int destino = 0; destino < NumeroVertices; destino++)
            {
                bool esRepetida = !EsDirigido && destino < origen;

                if (origen != destino && !esRepetida && pesosOriginales[origen, destino] > 0)
                    aristas.Add((origen, destino, pesosOriginales[origen, destino]));
            }
        }

        var aristasOrdenadas = aristas
            .OrderBy(a => a.Peso)
            .ThenBy(a => a.Origen)
            .ThenBy(a => a.Destino);

        foreach (var (origen, destino, peso) in aristasOrdenadas)
        {
            if (ExisteArista(origen, destino))
                continue;

            bool hayEspacio = GradoSalida(origen) < gradoMaximo
                && (EsDirigido ? GradoEntrada(destino) : GradoSalida(destino)) < gradoMaximo;

            if (hayEspacio)
                AsignarPeso(origen, destino, peso);
        }

        return true;
    }
    public int[][] ObtenerMatriz()
    {
        int[][] matriz = new int[NumeroVertices][];

        for (int i = 0; i < NumeroVertices; i++)
        {
            matriz[i] = new int[NumeroVertices];

            for (int j = 0; j < NumeroVertices; j++)
                matriz[i][j] = _matrizPesos[i, j];
        }

        return matriz;
    }
    private bool AlcanzaATodos(bool seguirAristasInversas)
    {
        bool[] visitados = new bool[NumeroVertices];
        var cola = new Queue<int>();

        cola.Enqueue(0);
        visitados[0] = true;
        int totalVisitados = 1;

        while (cola.Count > 0)
        {
            int actual = cola.Dequeue();

            for (int siguiente = 0; siguiente < NumeroVertices; siguiente++)
            {
                bool hayArista = seguirAristasInversas
                    ? ExisteArista(siguiente, actual)
                    : ExisteArista(actual, siguiente);

                if (visitados[siguiente] || !hayArista)
                    continue;

                visitados[siguiente] = true;
                totalVisitados++;
                cola.Enqueue(siguiente);
            }
        }

        return totalVisitados == NumeroVertices;
    }

    private List<int>? BuscarCicloHamiltoniano()
    {
        List<int>? mejorCiclo = null;
        int mejorCosto = int.MaxValue;

        for (int inicio = 0; inicio < NumeroVertices; inicio++)
        {
            var ciclo = new List<int> { inicio };
            bool[] visitados = new bool[NumeroVertices];
            visitados[inicio] = true;

            int actual = inicio;
            int costo = 0;

            while (ciclo.Count < NumeroVertices)
            {
                int siguiente = -1;

                for (int candidato = 0; candidato < NumeroVertices; candidato++)
                {
                    if (visitados[candidato] || !ExisteArista(actual, candidato))
                        continue;

                    if (siguiente < 0 || _matrizPesos[actual, candidato] < _matrizPesos[actual, siguiente])
                        siguiente = candidato;
                }

                if (siguiente < 0)
                    break;

                costo += _matrizPesos[actual, siguiente];
                ciclo.Add(siguiente);
                visitados[siguiente] = true;
                actual = siguiente;
            }

            bool esCompleto = ciclo.Count == NumeroVertices && ExisteArista(actual, inicio);

            if (!esCompleto)
                continue;

            costo += _matrizPesos[actual, inicio];

            if (costo < mejorCosto)
            {
                mejorCosto = costo;
                mejorCiclo = ciclo;
            }
        }

        return mejorCiclo;
    }
    private void ValidarVertice(int vertice)
    {
        if (vertice < 0 || vertice >= NumeroVertices)
            throw new ArgumentOutOfRangeException(nameof(vertice), "Se intenta acceder a un vértice inválido.");
    }
}