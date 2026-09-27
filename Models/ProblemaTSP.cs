using System.Data;

namespace traveling_salesman_problem_wasm.Models;

internal class ProblemaTSP
{
    private readonly GrafoPonderado _grafoPonderado;
    public int VericeInicial {get;}
    public int NumeroVertices {get;}

    public ProblemaTSP(GrafoPonderado grafoPonderado, int vericeInicial)
    {
        if(grafoPonderado == null)
            throw new ArgumentNullException("El grafo no puede ser nulo.");

        if(vericeInicial < 0 || vericeInicial >= grafoPonderado.NumeroVertices)
            throw new ArgumentOutOfRangeException("El grafo no contiene un vertice con este indice.");

        _grafoPonderado = grafoPonderado;
        VericeInicial = vericeInicial;
        NumeroVertices = grafoPonderado.NumeroVertices;
    }

    public int ObtenerPeso(int verticeOrigen, int verticeDestino)
    {
        if(!_grafoPonderado.EsGrafoValido())
            throw new EvaluateException("El grafo no puede evaularse por que es invalido.");

        return _grafoPonderado.ObtenerPeso(verticeOrigen, verticeDestino);
    }
}