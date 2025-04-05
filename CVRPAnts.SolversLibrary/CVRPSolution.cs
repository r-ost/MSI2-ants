using CVRPAnts.GraphLibrary;

namespace CVRPAnts.SolversLibrary;

public class CVRPSolution
{
    private readonly List<Route> routes;
    private readonly Graph graph;

    public IReadOnlyList<Route> Routes => this.routes;

    public double TotalLength => this.routes.Sum(r => r.Length);

    public Graph Graph => this.graph;

    public int RoutesCount => this.routes.Count;

    public int TotalDemand => this.routes.Sum(r => r.TotalDemand);

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

    public CVRPSolution(Graph graph, IEnumerable<Route> routes)
    {
        this.graph = graph ?? throw new ArgumentNullException(nameof(graph));
        this.routes = [.. routes ?? throw new ArgumentNullException(nameof(routes))];
    }

    public CVRPSolution Clone()
    {
        return new CVRPSolution(this.graph, this.routes.Select(r => r.Clone()));
    }

    public override string ToString()
    {
        var result = $"Solution with {this.routes.Count} routes, total cost: {this.TotalLength:F2}\n";
        for (int i = 0; i < this.routes.Count; i++)
        {
            result += $"Route {i + 1}: {this.routes[i]}\n";
        }
        return result;
    }
}
