namespace CVRPAnts.GraphLibrary;

public class Edge
{
    public Vertex Vertex1 { get; }

    public Vertex Vertex2 { get; }

    public double Weight { get; }


    public double Pheromone { get; private set; }

    public Edge(Vertex vertex1, Vertex vertex2, double weight, double initialPheromone = 0.1)
    {
        if (vertex1 == vertex2)
        {
            throw new ArgumentException("Cannot create an edge between the same vertex");
        }

        // Always store vertices in order of their IDs to maintain consistency
        if (vertex1.Id < vertex2.Id)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
        }
        else
        {
            this.Vertex1 = vertex2;
            this.Vertex2 = vertex1;
        }

        this.Weight = weight > 0 ? weight : throw new ArgumentException("Weight must be positive");
        this.Pheromone = initialPheromone;
    }

    public void UpdatePheromone(double amount)
    {
        this.Pheromone += amount;
        if (this.Pheromone < 0)
        {
            this.Pheromone = 0;
        }
    }

    public void SetPheromone(double value)
    {
        this.Pheromone = Math.Max(0, value);
    }

    public bool IsIncident(Vertex vertex)
    {
        return this.Vertex1.Id == vertex.Id || this.Vertex2.Id == vertex.Id;
    }

    public Vertex GetOtherVertex(Vertex vertex)
    {
        if (this.Vertex1.Id == vertex.Id)
        {
            return this.Vertex2;
        }
        else if (this.Vertex2.Id == vertex.Id)
        {
            return this.Vertex1;
        }
        else
        {
            throw new ArgumentException("The provided vertex is not incident to this edge");
        }
    }

    public override string ToString()
    {
        return $"Edge {this.Vertex1.Id}-{this.Vertex2.Id}, Weight: {this.Weight:F2}, Pheromone: {this.Pheromone:F4}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Edge other)
        {
            return false;
        }

        // Undirected edges are equal if they connect the same vertices, regardless of order
        return this.Vertex1.Id == other.Vertex1.Id && this.Vertex2.Id == other.Vertex2.Id ||
               this.Vertex1.Id == other.Vertex2.Id && this.Vertex2.Id == other.Vertex1.Id;
    }

    public override int GetHashCode()
    {
        // For undirected edges, the hash should be the same regardless of vertex order
        return this.Vertex1.Id.GetHashCode() ^ this.Vertex2.Id.GetHashCode();
    }

    public static bool operator ==(Edge left, Edge right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Edge left, Edge right)
    {
        return !(left == right);
    }
}
