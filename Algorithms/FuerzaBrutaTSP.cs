namespace traveling_salesman_problem_wasm.Algorithms;

using traveling_salesman_problem_wasm.Models;
using traveling_salesman_problem_wasm.Algorithms.Utilidades;

internal class FuerzaBrutaTSP
{
    private ProblemaTSP _problemaTSP;
    

    public FuerzaBrutaTSP(ProblemaTSP problemaTSP)
    {
        if(problemaTSP is null)
            throw new ArgumentNullException("Para resolver un TSP con fuerza bruta se necesita un problema valido.");

        _problemaTSP = problemaTSP;
    }
    
    public SolucionTSP Resolver()
    {
        
        int[] rutaActual = new int[_problemaTSP.NumeroVertices];
        bool[] ciudadVisitada = new bool[_problemaTSP.NumeroVertices];

        int[] mejorRuta = new int[_problemaTSP.NumeroVertices];
        int mejorCosto = int.MaxValue;

        rutaActual[0] = _problemaTSP.VericeInicial;
        ciudadVisitada[_problemaTSP.VericeInicial] = true;

        void Buscar(int posicionEnRuta, int costoAcumulado)
        {
            bool rutaCompleta = posicionEnRuta == _problemaTSP.NumeroVertices;
            if (rutaCompleta)
            {
                int ultimaCiudad = rutaActual[_problemaTSP.NumeroVertices - 1];
                int primeraCiudad = rutaActual[0];
                int costoTotal = costoAcumulado + _problemaTSP.ObtenerPeso(ultimaCiudad, primeraCiudad);

                bool esMejorQueLaAnterior = costoTotal < mejorCosto;
                if (esMejorQueLaAnterior)
                {
                    mejorCosto = costoTotal;
                    Array.Copy(rutaActual, mejorRuta, _problemaTSP.NumeroVertices);
                }

                return;
            }

            for (int ciudad = 0; ciudad < _problemaTSP.NumeroVertices; ciudad++)
            {
                if (ciudadVisitada[ciudad])
                    continue;

                int ciudadAnterior = rutaActual[posicionEnRuta - 1];
                int costoDelTramo = _problemaTSP.ObtenerPeso(ciudadAnterior, ciudad);

                ciudadVisitada[ciudad] = true;
                rutaActual[posicionEnRuta] = ciudad;
                Buscar(posicionEnRuta + 1, costoAcumulado + costoDelTramo);
                ciudadVisitada[ciudad] = false;
            }
        }

        Buscar(posicionEnRuta: 1, costoAcumulado: 0);
        List<int> rutaFinal = mejorRuta.ToList();
        rutaFinal.Add(_problemaTSP.VericeInicial);
        SolucionTSP solucion = new SolucionTSP(_problemaTSP, rutaFinal, mejorCosto);
        return solucion;
    }
    public int CiclosPosibles()
    {
        return MathUtils.Factorial(_problemaTSP.NumeroVertices - 1) / 2;
    }
}