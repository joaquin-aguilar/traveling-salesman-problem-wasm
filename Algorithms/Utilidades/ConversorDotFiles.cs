using System.Text;

namespace traveling_salesman_problem_wasm.Algorithms.Utilidades;

public static class ConversorDotFiles
{
    public static string CrearDotFile(int[][]? matriz, int VerticeInicio = 0, string nombreGrafico = "G", IReadOnlyList<int>? camino = null)
    {
        if (matriz is null || matriz.Length == 0)
            return string.Empty;

        int n = matriz.Length;

        Validar(matriz, n);
        var aristasRecorrido = CrearAristasRecorrido(camino, matriz, n, VerticeInicio);

        var constructorDot = new StringBuilder();
        constructorDot.AppendLine($"graph {nombreGrafico} {{");
        constructorDot.AppendLine("  layout=circo;");
        constructorDot.AppendLine("  normalize=true;");
        constructorDot.AppendLine("  overlap=false;");
        constructorDot.AppendLine("  splines=true;");
        constructorDot.AppendLine("  node [shape=circle];");

        for (int i = 0; i < n; i++)
            constructorDot.AppendLine($"  {(char)('A' + i)};");

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                int peso = matriz[i][j];

                if (peso == VerticeInicio)
                    continue;

                bool estaEnRecorrido = aristasRecorrido.Contains((i, j));
                string attrs;
                if (estaEnRecorrido)
                    attrs = $"label=\"{peso}\", color=\"red\", fontcolor=\"red\", penwidth=2.5";
                else
                    attrs = $"label=\"{peso}\"";

                constructorDot.AppendLine($"  {(char)('A' + i)} -- {(char)('A' + j)} [{attrs}];");
            }
        }

        constructorDot.AppendLine("}");
        return constructorDot.ToString();
    }

    private static void Validar(int[][] matriz, int fila)
    {
        for (int i = 0; i < fila; i++)
        {
            if (matriz[i] is null)
                throw new ArgumentException($"La fila {i} es null.", nameof(matriz));

            if (matriz[i].Length != fila)
                throw new ArgumentException($"La matriz debe ser cuadrada: la fila {i} tiene {matriz[i].Length} columnas y se esperaban {fila}.", nameof(matriz));
        }
    }

    private static HashSet<(int, int)> CrearAristasRecorrido(IReadOnlyList<int>? cliclo, int[][] matriz, int tamano, int noEdgeValue)
    {
        var aristas = new HashSet<(int, int)>();

        if (cliclo is null || cliclo.Count < 2)
            return aristas;

        foreach (int v in cliclo)
            if (v < 0 || v >= tamano)
                throw new ArgumentException($"El camino contiene el nodo {v}, fuera de rango (0..{tamano - 1}).", nameof(cliclo));

        int _tamano = cliclo.Count;
        bool cicloYaCerrado = cliclo[_tamano - 1] == cliclo[0];

        if (cicloYaCerrado)
            _tamano = _tamano - 1;

        if (_tamano < 2)
            return aristas;

        for (int i = 0; i < _tamano; i++)
        {
            int a = cliclo[i];
            int b;

            if (i == _tamano - 1)
                b = cliclo[0];
            else
                b = cliclo[i + 1];

            int u = a;
            int v = b;

            if (a > b)
            {
                u = b;
                v = a;
            }

            if (u == v || matriz[u][v] == noEdgeValue)
                throw new ArgumentException($"El camino usa la arista {u}-{v}, que no existe en la matriz.", nameof(cliclo));

            aristas.Add((u, v));
        }
        return aristas;
    }
}