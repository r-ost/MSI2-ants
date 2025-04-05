namespace CVRPAnts.GraphLibrary;

/// <summary>
/// Represents an edge in the graph between two vertices
/// </summary>
public class Edge
{
    /// <summary>
    /// Gets the first vertex of the edge
    /// </summary>
    public Vertex Vertex1 { get; }

    /// <summary>
    /// Gets the second vertex of the edge
    /// </summary>
    public Vertex Vertex2 { get; }

    /// <summary>
    /// Gets the weight (distance) of the edge
    /// </summary>
    public double Weight { get; }

    /// <summary>
    /// Gets the pheromone level on this edge
    /// </summary>
    public double Pheromone { get; private set; }

    /// <summary>
    /// Creates a new edge
    /// </summary>
    /// <param name="vertex1">The first vertex</param>
    /// <param name="vertex2">The second vertex</param>
    /// <param name="weight">The weight (distance)</param>
    /// <param name="initialPheromone">Initial pheromone level (default 0.1)</param>
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

    /// <summary>
    /// Updates the pheromone level on this edge
    /// </summary>
    /// <param name="amount">The amount to add (can be negative for evaporation)</param>
    public void UpdatePheromone(double amount)
    {
        this.Pheromone += amount;
        if (this.Pheromone < 0)
        {
            this.Pheromone = 0;
        }
    }

    /// <summary>
    /// Sets the pheromone level on this edge
    /// </summary>
    /// <param name="value">The new pheromone value</param>
    public void SetPheromone(double value)
    {
        this.Pheromone = Math.Max(0, value);
    }

    /// <summary>
    /// Checks if a vertex is incident to this edge
    /// </summary>
    /// <param name="vertex">The vertex to check</param>
    /// <returns>True if the vertex is incident to this edge</returns>
    public bool IsIncident(Vertex vertex)
    {
        return this.Vertex1.Id == vertex.Id || this.Vertex2.Id == vertex.Id;
    }

    /// <summary>
    /// Gets the other vertex connected to this edge
    /// </summary>
    /// <param name="vertex">One vertex of the edge</param>
    /// <returns>The other vertex</returns>
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

    /// <summary>
    /// Returns a string representation of this edge
    /// </summary>
    public override string ToString()
    {
        return $"Edge {this.Vertex1.Id}-{this.Vertex2.Id}, Weight: {this.Weight:F2}, Pheromone: {this.Pheromone:F4}";
    }

    /// <summary>
    /// Determines whether this edge is equal to another object
    /// </summary>
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

    /// <summary>
    /// Returns a hash code for this edge
    /// </summary>
    public override int GetHashCode()
    {
        // For undirected edges, the hash should be the same regardless of vertex order
        return this.Vertex1.Id.GetHashCode() ^ this.Vertex2.Id.GetHashCode();
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Edge left, Edge right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Edge left, Edge right)
    {
        return !(left == right);
    }
}