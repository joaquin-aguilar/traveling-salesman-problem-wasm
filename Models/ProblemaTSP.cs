namespace traveling_salesman_problem_wasm.Models;

/// <summary>Un grafo válido junto con el vértice desde el que parte el recorrido.</summary>
internal class ProblemaTSP
{
    private readonly GrafoPonderado _grafo;

    public int VerticeInicial { get; }
    public int NumeroVertices => _grafo.NumeroVertices;
    public bool EsDirigido => _grafo.EsDirigido;

    public ProblemaTSP(GrafoPonderado grafo, int verticeInicial)
    {
        ArgumentNullException.ThrowIfNull(grafo);

        if(verticeInicial < 0 || verticeInicial >= grafo.NumeroVertices)
            throw new ArgumentOutOfRangeException(nameof(verticeInicial), "El grafo no contiene un vértice con este índice.");

        if(!grafo.EsGrafoValido())
            throw new ArgumentException("El grafo no es válido: revisa la conectividad y el grado de los vértices.", nameof(grafo));

        _grafo = grafo;
        VerticeInicial = verticeInicial;
    }

    public int ObtenerPeso(int origen, int destino)
    {
        return _grafo.ObtenerPeso(origen, destino);
    }
    public bool ExisteArista(int origen, int destino) 
    {
        return _grafo.ExisteArista(origen, destino);
    }
}
