using System.Text.RegularExpressions;
using CVRPAnts.GraphLibrary;
using CVRPAnts.SolversLibrary;

namespace CVRPAnts.ParserLibrary;

public class CVRPSolutionParser
{
    /// <summary>
    /// Parse a CVRP solution file and return a CVRPSolution object
    /// </summary>
    /// <param name="filePath">Path to the solution file</param>
    /// <param name="graph">The graph associated with this solution</param>
    /// <returns>A CVRP Solution object</returns>
    public static CVRPSolution ParseSolutionFile(string filePath, Graph graph, int capacity, double maxRouteLength)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var lines = File.ReadAllLines(filePath);
        return ParseSolution(lines, graph, capacity, maxRouteLength);
    }

    /// <summary>
    /// Parse solution from the given text content
    /// </summary>
    /// <param name="content">Solution file content</param>
    /// <param name="graph">The graph associated with this solution</param>
    /// <returns>A CVRP Solution object</returns>
    public static CVRPSolution ParseSolutionContent(string content, Graph graph, int capacity, double maxRouteLength)
    {
        var lines = content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        return ParseSolution(lines, graph, capacity, maxRouteLength);
    }

    private static CVRPSolution ParseSolution(string[] lines, Graph graph, int capacity, double maxRouteLength)
    {
        if (graph == null)
        {
            throw new ArgumentNullException(nameof(graph));
        }

        if (graph.Depot is null)
        {
            throw new InvalidOperationException("Graph does not contain a depot vertex.");
        }

        var routes = new List<Route>();
        double? cost = null;

        // Regular expression to match route lines: "Route #X:" followed by numbers
        var routeRegex = new Regex(@"Route\s+#\s*(\d+)\s*:\s*(.+)", RegexOptions.IgnoreCase);
        // Regular expression to match cost line: "Cost" followed by a number
        var costRegex = new Regex(@"Cost\s*:?\s*(\d+(\.\d+)?)", RegexOptions.IgnoreCase);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("//"))
            {
                continue;
            }

            // Try to parse as a route
            var routeMatch = routeRegex.Match(trimmedLine);
            if (routeMatch.Success)
            {
                var vertices = new List<Vertex>
                {
                    graph.Depot
                };
                var routeIndices = routeMatch.Groups[2].Value.Split(
                    [' ', ',', '\t'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s.Trim()) + 1)
                    .ToList();

                foreach (var index in routeIndices)
                {
                    var vertex = graph.GetVertex(index)
                        ?? throw new FormatException($"Vertex with ID {index} not found in the graph.");
                    vertices.Add(vertex);
                }
                // Create the route
                routes.Add(Route.FromVertexIds(graph, vertices.Select(v => v.Id), capacity, maxRouteLength));
                continue;
            }

            // Try to parse as cost
            var costMatch = costRegex.Match(trimmedLine);
            if (costMatch.Success)
            {
                cost = double.Parse(costMatch.Groups[1].Value);
                continue;
            }
        }

        if (routes.Count == 0)
        {
            throw new FormatException("No valid routes found in the solution file.");
        }

        var solution = new CVRPSolution(graph, routes);

        if (!solution.IsValid)
        {
            throw new FormatException("Parsed solution is not valid.");
        }

        // Verify if the parsed cost matches the calculated cost
        if (cost.HasValue && cost.Value != solution.TotalCost)
        {
            throw new FormatException($"Parsed cost {cost.Value} does not match calculated cost {solution.TotalCost}.");
        }

        return solution;
    }
}
