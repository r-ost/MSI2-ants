using CVRPAnts.GraphLibrary;

namespace CVRPAnts.SolversLibrary;

/// <summary>
/// Represents a Vehicle Routing Problem instance with all required parameters
/// </summary>
public class CVRPInstance
{
    /// <summary>
    /// Gets or sets the graph representing the CVRP problem
    /// </summary>
    public Graph Graph { get; set; }

    /// <summary>
    /// Gets or sets the capacity of each vehicle
    /// </summary>
    public int VehicleCapacity { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed route distance (optional)
    /// </summary>
    public double MaxRouteDistance { get; set; } = double.MaxValue;

    /// <summary>
    /// Gets or sets the name of the instance
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Creates a new empty VRP instance
    /// </summary>
    public CVRPInstance()
    {
        Graph = new Graph();
        Name = string.Empty;
    }

    /// <summary>
    /// Returns a string summary of the VRP instance
    /// </summary>
    public override string ToString()
    {
        return $"VRP Instance '{Name}': {Graph.VertexCount} vertices, capacity {VehicleCapacity}, max distance {MaxRouteDistance}";
    }
}