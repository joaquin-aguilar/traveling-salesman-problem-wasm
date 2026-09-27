namespace traveling_salesman_problem_wasm.Models;

internal class GrafoPonderado
{
    private readonly int [,] _matrizPesos;
    public int NumeroVertices {get;}

    public GrafoPonderado(int numeroVertices)
    {
        if(numeroVertices < 5)
            throw new ArgumentException("Se requiere un minimo de 5 vertices.");
        
        NumeroVertices = numeroVertices;
        _matrizPesos = new int[NumeroVertices, NumeroVertices];
    }
    public void AsingarPeso(int verticeOrigen, int verticeDestino, int peso)
    {
        if(peso <= 0)
            throw new ArgumentException("No se admiten pesos nulos o negativos.");

        ValidarVertice(verticeOrigen, verticeDestino);
        _matrizPesos[verticeOrigen, verticeDestino] = peso;
        _matrizPesos[verticeDestino, verticeOrigen] = peso;
    }
    public int ObtenerPeso(int verticeOrigen, int verticeDestino)
    {
        ValidarVertice(verticeOrigen, verticeDestino);
        return _matrizPesos[verticeOrigen, verticeDestino];
    }

    public bool EsGrafoValido()
    {
        for(int filas = 0; filas < NumeroVertices; filas++)
            for(int columnas = 0; columnas < NumeroVertices; columnas++)
            {
                if(filas  == columnas)
                    continue;
                if(_matrizPesos[filas, columnas] <= 0)
                    return false;
            }
        return true;
    }
    private void ValidarVertice(int verticeOrigen, int verticeDestino)
    {
        if(verticeOrigen == verticeDestino)
            throw new ArgumentException("No se admiten bucles, un vertice no puede apuntarse a si mismo.");

        if(verticeOrigen < 0 || verticeOrigen >=  NumeroVertices ||
           verticeDestino < 0 || verticeDestino >=  NumeroVertices)
            throw new ArgumentOutOfRangeException("Se intenta acceder a un vertice invalido.");

    }
}