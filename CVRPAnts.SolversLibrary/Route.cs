using CVRPAnts.GraphLibrary;

namespace CVRPAnts.SolversLibrary;

public class Route
{
    private readonly List<Vertex> vertices;
    private readonly Graph graph;
    public IReadOnlyList<Vertex> Vertices => this.vertices;

    public double Length
    {
        get
        {
            var result = 0.0;
            for (int i = 0; i < this.vertices.Count - 1; i++)
            {
                var edge = this.graph.GetEdge(this.vertices[i].Id, this.vertices[i + 1].Id)
                    ?? throw new InvalidOperationException($"Edge between {this.vertices[i].Id} and {this.vertices[i + 1].Id} not found in the graph");
                result += edge.Weight;
            }

            // add return trip to depot
            if (this.vertices.Count > 1)
            {
                if (this.graph.Depot is null)
                {
                    throw new InvalidOperationException("Depot vertex not found in the graph");
                }

                var returnEdge = this.graph.GetEdge(this.vertices.Last().Id, this.graph.Depot.Id)
                    ?? throw new InvalidOperationException($"Return edge from {this.vertices.Last().Id} to depot not found in the graph");
                result += returnEdge.Weight;
            }
            return result;
        }
    }

    public int TotalDemand
    {
        get
        {
            return this.vertices.Sum(v => v.Demand);
        }
    }

    public int Capacity { get; }

    public double MaxRouteLength { get; }

    public bool IsValid => this.TotalDemand <= this.Capacity && this.Length <= this.MaxRouteLength;

    public Route(Graph graph, int capacity, double maxRouteLength)
    {
        this.graph = graph ?? throw new ArgumentNullException(nameof(graph));
        this.vertices = new List<Vertex>();
        this.Capacity = capacity;
        this.MaxRouteLength = maxRouteLength;

        // Add depot as the starting point
        if (graph.Depot is null)
        {
            throw new ArgumentException("Graph must contain a depot vertex");
        }
        this.vertices.Add(graph.Depot);
    }

    public static Route FromVertexIds(Graph graph, IEnumerable<int> vertexIds, int capacity, double maxRouteLength)
    {
        var route = new Route(graph, capacity, maxRouteLength);
        if (graph.Depot is null)
        {
            throw new ArgumentException("Graph must contain a depot vertex");
        }

        // Skip the first depot (already added in constructor)
        bool isFirst = true;
        foreach (var id in vertexIds)
        {
            if (isFirst)
            {
                if (id != graph.Depot.Id)
                {
                    throw new ArgumentException("Route must start from depot");
                }
                isFirst = false;
                continue;
            }

            var vertex = graph.GetVertex(id) ?? throw new ArgumentException($"Vertex with ID {id} does not exist in the graph");
            route.AddVertex(vertex);
        }
        return route;
    }

    public void AddVertex(Vertex vertex)
    {
        if (this.vertices.Contains(vertex))
        {
            throw new ArgumentException($"Vertex {vertex.Id} is already in the route");
        }
        this.vertices.Add(vertex);
    }

    public double CapacityUtilization => this.Capacity > 0 ? (double)this.TotalDemand / this.Capacity : 0;

    public double LengthUtilization => this.MaxRouteLength > 0 ? this.Length / this.MaxRouteLength : 0;

    public override string ToString()
    {
        var vertexIds = string.Join(" -> ", this.vertices.Select(v => v.Id));
        return $"Route: {vertexIds}, Length: {this.Length:F2}, Demand: {this.TotalDemand}/{this.Capacity}, Utilization: {this.CapacityUtilization:P2}, Length Utilization: {this.LengthUtilization:P2}";
    }

    public Route Clone()
    {
        var clone = new Route(this.graph, this.Capacity, this.MaxRouteLength);
        clone.vertices.Clear(); // Remove depot added by constructor

        foreach (var vertex in this.vertices)
        {
            clone.vertices.Add(vertex);
        }
        return clone;
    }
}
