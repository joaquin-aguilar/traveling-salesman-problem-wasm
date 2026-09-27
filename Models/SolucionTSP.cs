namespace traveling_salesman_problem_wasm.Models;

internal class SolucionTSP
{
    private readonly ProblemaTSP _problemaTSP;
    public List<int> Ruta {get;}
    public int CostoTotal {get;}
    
    public SolucionTSP(ProblemaTSP problemaTSP, List<int> ruta, int costoTotal)
    {
        if(problemaTSP is null)
            throw new ArgumentNullException("Una solucion no puede existir sin un problema.");

        if(ruta is null or [])
            throw new ArgumentException("La ruta no puede ser nula.");
        
        if(ruta.First() != ruta.Last())
            throw new ArgumentException("La ruta no inicia y finaliza en el mismo vertice, por lo cual no cumple con un ciclo hamiltoniano.");
        
        List<int> indicesVertices = Enumerable.Range(0, problemaTSP.NumeroVertices).ToList();
        HashSet<int> rutaHash = new HashSet<int>(ruta);
        bool contieneTodos = indicesVertices.All(num => rutaHash.Contains(num));
        List<int> cicloHamiltoniano = ruta.Skip(1).Take(ruta.Count - 2).ToList();
        if(cicloHamiltoniano.Count != cicloHamiltoniano.Distinct().Count() || !contieneTodos)
            throw new ArgumentException("La ruta no representa un cilo hamiltoniano.");

        if(ruta.First() != problemaTSP.VericeInicial)
            throw new ArgumentException("La ruta no tiene como primer elemento al primer vertice.");

        _problemaTSP = problemaTSP;
        Ruta = ruta;
        CostoTotal = costoTotal;
    }
}