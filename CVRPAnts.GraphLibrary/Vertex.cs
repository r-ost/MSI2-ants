namespace CVRPAnts.GraphLibrary;

/// <summary>
/// Represents a vertex in the graph with a position and demand
/// </summary>
/// <remarks>
/// Creates a new vertex
/// </remarks>
/// <param name="id">The unique identifier</param>
/// <param name="x">X coordinate</param>
/// <param name="y">Y coordinate</param>
/// <param name="demand">Demand value (default 0)</param>
public class Vertex(int id, int x, int y, int demand = 0)
{
    /// <summary>
    /// Gets the unique identifier of the vertex
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets the X coordinate of the vertex
    /// </summary>
    public int X { get; } = x;

    /// <summary>
    /// Gets the Y coordinate of the vertex
    /// </summary>
    public int Y { get; } = y;

    /// <summary>
    /// Gets the demand of this vertex
    /// </summary>
    public int Demand { get; } = demand;

    /// <summary>
    /// Calculates the Euclidean distance to another vertex
    /// </summary>
    /// <param name="other">The other vertex</param>
    /// <returns>The distance between vertices</returns>
    public double DistanceTo(Vertex other)
    {
        return Math.Sqrt(Math.Pow(this.X - other.X, 2) + Math.Pow(this.Y - other.Y, 2));
    }

    /// <summary>
    /// Returns a string representation of this vertex
    /// </summary>
    public override string ToString()
    {
        return $"Vertex {this.Id} at ({this.X:F2}, {this.Y:F2}), Demand: {this.Demand}";
    }

    /// <summary>
    /// Determines whether this vertex is equal to another object
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is Vertex other && this.Id == other.Id;
    }

    /// <summary>
    /// Returns a hash code for this vertex
    /// </summary>
    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Vertex left, Vertex right)
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
    public static bool operator !=(Vertex left, Vertex right)
    {
        return !(left == right);
    }
}