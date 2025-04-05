using CVRPAnts.GraphLibrary;

namespace CVRPAnts.SolversLibrary;

public class CVRPSolution
{
    private readonly List<Route> routes;
    private readonly Graph graph;

    /// <summary>
    /// Gets all routes in the solution
    /// </summary>
    public IReadOnlyList<Route> Routes => this.routes;

    /// <summary>
    /// Gets the total cost of the solution (sum of route lengths)
    /// </summary>
    public double TotalCost => this.routes.Sum(r => r.Length);

    /// <summary>
    /// Gets the graph associated with this solution
    /// </summary>
    public Graph Graph => this.graph;

    /// <summary>
    /// Gets the number of vehicles (routes) in this solution
    /// </summary>
    public int RoutesCount => this.routes.Count;

    /// <summary>
    /// Gets the total demand serviced by all routes
    /// </summary>
    public int TotalDemand => this.routes.Sum(r => r.TotalDemand);

    /// <summary>
    /// Checks if the solution is valid (all routes are valid and all customers are visited exactly once)
    /// </summary>
    public bool IsValid
    {
        get
        {
            if (this.routes.Any(r => !r.IsValid))
            {
                return false;
            }

            var depotVertexId = this.graph.Depot?.Id ?? throw new InvalidOperationException("Depot vertex not found in the graph");
            // Check if all non-depot vertices are visited exactly once
            var visitedVertices = new HashSet<int>();
            foreach (var route in this.routes)
            {
                foreach (var vertex in route.Vertices)
                {
                    // Skip depot
                    if (vertex.Id == depotVertexId)
                    {
                        continue;
                    }

                    if (visitedVertices.Contains(vertex.Id))
                    {
                        throw new InvalidOperationException($"Vertex {vertex.Id} visited multiple times in different routes");
                    }

                    visitedVertices.Add(vertex.Id);
                }
            }

            // Check if all non-depot vertices in the graph are visited
            return this.graph.Vertices.Count(v => v.Id != depotVertexId) == visitedVertices.Count;
        }
    }

    /// <summary>
    /// Creates a solution from a list of routes
    /// </summary>
    /// <param name="graph">The graph</param>
    /// <param name="routes">The routes</param>
    public CVRPSolution(Graph graph, IEnumerable<Route> routes)
    {
        this.graph = graph ?? throw new ArgumentNullException(nameof(graph));
        this.routes = [.. routes ?? throw new ArgumentNullException(nameof(routes))];
    }

    /// <summary>
    /// Creates a deep clone of this solution
    /// </summary>
    /// <returns>The cloned solution</returns>
    public CVRPSolution Clone()
    {
        return new CVRPSolution(this.graph, this.routes.Select(r => r.Clone()));
    }

    /// <summary>
    /// Returns a string representation of the solution
    /// </summary>
    public override string ToString()
    {
        var result = $"Solution with {this.routes.Count} routes, total cost: {this.TotalCost:F2}\n";
        for (int i = 0; i < this.routes.Count; i++)
        {
            result += $"Route {i + 1}: {this.routes[i]}\n";
        }
        return result;
    }
}