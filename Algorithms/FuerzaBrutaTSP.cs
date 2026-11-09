using traveling_salesman_problem_wasm.Algorithms.Utilidades;
using traveling_salesman_problem_wasm.Models;

namespace traveling_salesman_problem_wasm.Algorithms;

internal class FuerzaBrutaTSP : IResolver
{
    private readonly ProblemaTSP _problema;

    private int[] _rutaActual = Array.Empty<int>();
    private bool[] _visitado = Array.Empty<bool>();
    private int[] _mejorRuta = Array.Empty<int>();
    private int _mejorCosto;

    public FuerzaBrutaTSP(ProblemaTSP problema)
    {
        ArgumentNullException.ThrowIfNull(problema);
        _problema = problema;
    }

    public SolucionTSP? Resolver()
    {
        int n = _problema.NumeroVertices;
        int inicio = _problema.VerticeInicial;

        _rutaActual = new int[n];
        _visitado = new bool[n];
        _mejorRuta = new int[n];
        _mejorCosto = int.MaxValue;

        _rutaActual[0] = inicio;
        _visitado[inicio] = true;

        Buscar(posicion: 1, costoAcumulado: 0);

        if (_mejorCosto == int.MaxValue)
            return null;

        var ruta = _mejorRuta.ToList();
        ruta.Add(inicio);

        return new SolucionTSP(_problema, ruta, _mejorCosto);
    }

    public long CiclosPosibles()
    {
        long ciclos = CalculosMatematica.Factorial(_problema.NumeroVertices - 1);
        return _problema.EsDirigido ? ciclos : ciclos / 2;
    }

    private void Buscar(int posicion, int costoAcumulado)
    {
        if (costoAcumulado >= _mejorCosto)
            return;

        int n = _problema.NumeroVertices;
        int anterior = _rutaActual[posicion - 1];

        if (posicion == n)
        {
            int inicio = _rutaActual[0];

            if (!_problema.ExisteArista(anterior, inicio))
                return;

            int costoTotal = costoAcumulado + _problema.ObtenerPeso(anterior, inicio);

            if (costoTotal < _mejorCosto)
            {
                _mejorCosto = costoTotal;
                Array.Copy(_rutaActual, _mejorRuta, n);
            }

            return;
        }

        for (int ciudad = 0; ciudad < n; ciudad++)
        {
            if (_visitado[ciudad] || !_problema.ExisteArista(anterior, ciudad))
                continue;

            _visitado[ciudad] = true;
            _rutaActual[posicion] = ciudad;

            Buscar(posicion + 1, costoAcumulado + _problema.ObtenerPeso(anterior, ciudad));

            _visitado[ciudad] = false;
        }
    }
}
