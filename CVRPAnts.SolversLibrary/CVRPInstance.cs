using CVRPAnts.GraphLibrary;

namespace CVRPAnts.SolversLibrary;

public class CVRPInstance
{
    public Graph Graph { get; set; }

    public int VehicleCapacity { get; set; }

    public double MaxRouteDistance { get; set; } = double.MaxValue;

    public string Name { get; set; }

    public CVRPInstance()
    {
        Graph = new Graph();
        Name = string.Empty;
    }

    public override string ToString()
    {
        return $"VRP Instance '{Name}': {Graph.VertexCount} vertices, capacity {VehicleCapacity}, max distance {MaxRouteDistance}";
    }
}
