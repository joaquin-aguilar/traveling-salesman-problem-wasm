using System.Text;

namespace traveling_salesman_problem_wasm.Algorithms.Utilidades;

/// <summary>Convierte una matriz de pesos en texto DOT (Graphviz).</summary>
public static class ConversorDotFiles
{
    public static string CrearDotFile(
        int[][]? matriz,
        int verticeInicio = 0,
        string nombreGrafico = "G",
        IReadOnlyList<int>? camino = null,
        bool esDirigido = false,
        IReadOnlyList<string>? etiquetas = null)
    {
        if (matriz is null || matriz.Length == 0)
            return string.Empty;

        int n = matriz.Length;

        if (matriz.Any(fila => fila is null || fila.Length != n))
            throw new ArgumentException("La matriz debe ser cuadrada.", nameof(matriz));

        HashSet<(int, int)> aristasRecorrido = CrearAristasRecorrido(camino, matriz, esDirigido);

        string tipoGrafo = esDirigido ? "digraph" : "graph";
        string conector = esDirigido ? "->" : "--";

        var dot = new StringBuilder();
        dot.AppendLine($"{tipoGrafo} {nombreGrafico} {{");
        dot.AppendLine("  layout=circo;");
        dot.AppendLine("  normalize=true;");
        dot.AppendLine("  overlap=false;");
        dot.AppendLine("  splines=true;");
        dot.AppendLine("  node [shape=circle, style=filled, fillcolor=\"#eef3fb\", color=\"#315b8a\", fontname=\"Helvetica\"];");
        dot.AppendLine("  edge [fontname=\"Helvetica\", fontsize=10];");

        // Vértices
        for (int i = 0; i < n; i++)
        {
            var atributos = new List<string>();

            if (etiquetas is not null && i < etiquetas.Count && !string.IsNullOrWhiteSpace(etiquetas[i]))
                atributos.Add($"label=\"{EscaparEtiqueta(etiquetas[i])}\"");

            if (i == verticeInicio)
                atributos.Add("fillcolor=\"#b7e4c7\", color=\"#2d6a4f\", penwidth=2");

            dot.AppendLine(atributos.Count == 0
                ? $"  {NombreVertice(i)};"
                : $"  {NombreVertice(i)} [{string.Join(", ", atributos)}];");
        }

        // Aristas. En un grafo no dirigido se recorre solo la mitad superior de la matriz.
        for (int i = 0; i < n; i++)
        {
            for (int j = esDirigido ? 0 : i + 1; j < n; j++)
            {
                int peso = matriz[i][j];

                if (i == j || peso <= 0)
                    continue;

                string atributos = aristasRecorrido.Contains((i, j))
                    ? $"label=\"{peso}\", color=\"#d63384\", fontcolor=\"#d63384\", penwidth=2.5"
                    : $"label=\"{peso}\"";

                dot.AppendLine($"  {NombreVertice(i)} {conector} {NombreVertice(j)} [{atributos}];");
            }
        }

        dot.AppendLine("}");
        return dot.ToString();
    }

    private static string NombreVertice(int indice)
        => indice < 26 ? ((char)('A' + indice)).ToString() : $"V{indice}";

    private static string EscaparEtiqueta(string etiqueta)
        => etiqueta.Replace("\\", "\\\\").Replace("\"", "\\\"");

    /// <summary>
    /// Aristas que forman el camino. Si el grafo no es dirigido se guardan
    /// siempre como (menor, mayor) para poder buscarlas sin importar el sentido.
    /// </summary>
    private static HashSet<(int, int)> CrearAristasRecorrido(IReadOnlyList<int>? camino, int[][] matriz, bool esDirigido)
    {
        var aristas = new HashSet<(int, int)>();

        if (camino is null)
            return aristas;

        for (int i = 0; i < camino.Count - 1; i++)
        {
            int origen = camino[i];
            int destino = camino[i + 1];

            if (origen < 0 || origen >= matriz.Length || destino < 0 || destino >= matriz.Length)
                throw new ArgumentException("El camino contiene un vértice fuera de rango.", nameof(camino));

            if (matriz[origen][destino] <= 0)
                throw new ArgumentException($"El camino usa la arista {origen}-{destino}, que no existe en la matriz.", nameof(camino));

            aristas.Add(esDirigido || origen < destino ? (origen, destino) : (destino, origen));
        }

        return aristas;
    }
}
