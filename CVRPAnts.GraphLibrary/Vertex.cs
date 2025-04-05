namespace CVRPAnts.GraphLibrary;

public class Vertex(int id, int x, int y, int demand = 0)
{

    public int Id { get; } = id;

    public int X { get; } = x;

    public int Y { get; } = y;

    public int Demand { get; } = demand;

    public double DistanceTo(Vertex other)
    {
        var res = Math.Sqrt(Math.Pow(this.X - other.X, 2) + Math.Pow(this.Y - other.Y, 2));
        return Math.Round(res);
    }

    public override string ToString()
    {
        return $"Vertex {this.Id} at ({this.X:F2}, {this.Y:F2}), Demand: {this.Demand}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Vertex other && this.Id == other.Id;
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public static bool operator ==(Vertex left, Vertex right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Vertex left, Vertex right)
    {
        return !(left == right);
    }
}
