namespace CVRPAnts.GraphLibrary;

public class Graph
{
    private readonly Dictionary<int, Vertex> vertices;
    private readonly Dictionary<(int, int), Edge> edges;
    private int depotId = 0;

    public IReadOnlyCollection<Vertex> Vertices => this.vertices.Values;

    public IReadOnlyCollection<Edge> Edges => this.edges.Values;

    public Vertex? Depot => this.vertices.TryGetValue(this.depotId, out Vertex? value) ? value : null;

    public int VertexCount => this.vertices.Count;

    public int EdgeCount => this.edges.Count;

    public Graph()
    {
        this.vertices = new Dictionary<int, Vertex>();
        this.edges = new Dictionary<(int, int), Edge>();
    }

    public Vertex AddVertex(int id, int x, int y, int demand = 0, bool isDepot = false)
    {
        if (this.vertices.ContainsKey(id))
        {
            throw new ArgumentException($"Vertex with ID {id} already exists");
        }

        if (isDepot)
        {
            if (demand != 0)
            {
                throw new ArgumentException("Depot vertex must have zero demand");
            }
            this.depotId = id; // Update depot ID if this is a depot vertex
        }

        var vertex = new Vertex(id, x, y, demand);
        this.vertices[id] = vertex;

        // For full graphs, add edges to all existing vertices
        foreach (var existingVertex in this.vertices.Values.Where(v => v.Id != id))
        {
            this.AddEdge(vertex.Id, existingVertex.Id);
        }

        return vertex;
    }

    public Vertex? GetVertex(int id)
    {
        return this.vertices.TryGetValue(id, out var vertex) ? vertex : null;
    }

    public Edge AddEdge(int vertexId1, int vertexId2)
    {
        if (vertexId1 == vertexId2)
        {
            throw new ArgumentException("Cannot create an edge between the same vertex");
        }

        // Ensure the smaller ID is always first to maintain uniqueness for undirected edges
        if (vertexId1 > vertexId2)
        {
            (vertexId1, vertexId2) = (vertexId2, vertexId1);
        }

        if (!this.vertices.TryGetValue(vertexId1, out Vertex? vertex1) || !this.vertices.ContainsKey(vertexId2))
        {
            throw new ArgumentException("One or both vertices do not exist in the graph");
        }

        var key = (vertexId1, vertexId2);

        // Return existing edge if it exists
        if (this.edges.TryGetValue(key, out var existingEdge))
        {
            return existingEdge;
        }

        var vertex2 = this.vertices[vertexId2];

        // Calculate Euclidean distance as the weight
        var weight = vertex1.DistanceTo(vertex2);
        if (weight <= 0)
        {
            throw new ArgumentException("Weight must be positive");
        }

        var edge = new Edge(vertex1, vertex2, weight);
        this.edges[key] = edge;

        return edge;
    }

    public Edge? GetEdge(int vertexId1, int vertexId2)
    {
        // Ensure the smaller ID is always first for consistency
        if (vertexId1 > vertexId2)
        {
            (vertexId1, vertexId2) = (vertexId2, vertexId1);
        }

        var key = (vertexId1, vertexId2);
        return this.edges.TryGetValue(key, out var edge) ? edge : null;
    }

    public double GetDistance(int vertexId1, int vertexId2)
    {
        var edge = this.GetEdge(vertexId1, vertexId2);
        return edge?.Weight ?? double.MaxValue;
    }

    public IEnumerable<Vertex> GetAdjacentVertices(int vertexId)
    {
        if (!this.vertices.ContainsKey(vertexId))
        {
            return [];
        }

        // In a full graph, all vertices are adjacent except the vertex itself
        return this.vertices.Values.Where(v => v.Id != vertexId);
    }
}
