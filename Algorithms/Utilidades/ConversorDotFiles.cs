using System.Text;

namespace traveling_salesman_problem_wasm.Algorithms.Utilidades;
public static class ConversorDotFiles
{
    public static string CrearDotFile(
        int[][]? matriz,
        int verticeInicio = 0,
        string nombreGrafico = "G",
        IReadOnlyList<int>? camino = null,
        bool esDirigido = false,
        IReadOnlyList<string>? etiquetas = null,
        bool ocultarOtrasAristas = false)
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
        dot.AppendLine("  start=1;");
        dot.AppendLine("  overlap=scale;");
        dot.AppendLine("  sep=\"+35\";");
        dot.AppendLine("  splines=true;");
        dot.AppendLine("  node [shape=circle, width=0.7, style=filled, fillcolor=\"#eef3fb\", color=\"#315b8a\", fontname=\"Helvetica\", fontsize=14];");
        dot.AppendLine("  edge [fontname=\"Helvetica\", fontsize=12];");

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

        (int pesoMin, int pesoMax) = RangoPesos(matriz, esDirigido);

        for (int i = 0; i < n; i++)
        {
            for (int j = esDirigido ? 0 : i + 1; j < n; j++)
            {
                int peso = matriz[i][j];

                if (i == j || peso <= 0)
                    continue;

                string longitud = LongitudArista(peso, pesoMin, pesoMax);
                bool enCamino = aristasRecorrido.Contains((i, j));

                if (ocultarOtrasAristas && camino is not null && !enCamino)
                {
                    dot.AppendLine($"  {NombreVertice(i)} {conector} {NombreVertice(j)} [label=\"{peso}\", len={longitud}, style=invis];");
                    continue;
                }

                string atributos = enCamino
                    ? $"label=\"{peso}\", len={longitud}, color=\"#d63384\", fontcolor=\"#d63384\", penwidth=2.5"
                    : $"label=\"{peso}\", len={longitud}";

                dot.AppendLine($"  {NombreVertice(i)} {conector} {NombreVertice(j)} [{atributos}];");
            }
        }

        dot.AppendLine("}");
        return dot.ToString();
    }

    private static (int Min, int Max) RangoPesos(int[][] matriz, bool esDirigido)
    {
        int min = int.MaxValue, max = int.MinValue;

        for (int i = 0; i < matriz.Length; i++)
            for (int j = 0; j < matriz.Length; j++)
            {
                if (i == j || matriz[i][j] <= 0 || (!esDirigido && j < i))
                    continue;

                min = Math.Min(min, matriz[i][j]);
                max = Math.Max(max, matriz[i][j]);
            }

        return min == int.MaxValue ? (1, 1) : (min, max);
    }

    private static string LongitudArista(int peso, int min, int max)
    {
        double t = max == min ? 0.5 : (double)(peso - min) / (max - min);
        return (2.5 + 4.0 * t).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string NombreVertice(int indice)
        => indice < 26 ? ((char)('A' + indice)).ToString() : $"V{indice}";

    private static string EscaparEtiqueta(string etiqueta)
        => etiqueta.Replace("\\", "\\\\").Replace("\"", "\\\"");

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
