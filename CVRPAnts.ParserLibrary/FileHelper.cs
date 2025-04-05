using CVRPAnts.SolversLibrary;

namespace CVRPAnts.ParserLibrary;

/// <summary>
/// Utility class for file operations related to CVRP instances
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Loads all VRPLIB format files from a directory
    /// </summary>
    /// <param name="directoryPath">Directory containing .vrp files</param>
    /// <returns>Dictionary of instance names to VRPInstances</returns>
    public static Dictionary<string, CVRPInstance> LoadAllInstancesFromDirectory(
        string directoryPath,
        double maxRouteDistance)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        var instances = new Dictionary<string, CVRPInstance>();
        var files = Directory.GetFiles(directoryPath, "*.vrp");

        foreach (var file in files)
        {
            try
            {
                var instance = CVRPParser.ParseVRPFile(file, maxRouteDistance);
                instances[instance.Name] = instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing file {file}: {ex.Message}");
            }
        }

        return instances;
    }

    /// <summary>
    /// Loads a single VRPLIB format file
    /// </summary>
    /// <param name="filePath">Path to the .vrp file</param>
    /// <returns>A VRPInstance object</returns>
    public static CVRPInstance LoadInstanceFromFile(
        string filePath,
        double maxRouteDistance)
    {
        return CVRPParser.ParseVRPFile(filePath, maxRouteDistance);
    }

    /// <summary>
    /// Exports a graph to a simple text format
    /// </summary>
    /// <param name="instance">The VRP instance to export</param>
    /// <param name="filePath">Path where to save the exported file</param>
    public static void ExportInstance(CVRPInstance instance, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            // Write header
            writer.WriteLine($"# VRP Instance: {instance.Name}");
            writer.WriteLine($"# Vertices: {instance.Graph.VertexCount}");
            writer.WriteLine($"# Capacity: {instance.VehicleCapacity}");
            writer.WriteLine($"# Max Route Distance: {instance.MaxRouteDistance}");
            writer.WriteLine();

            // Write vertices
            writer.WriteLine("# Vertices (ID, X, Y, Demand)");
            foreach (var vertex in instance.Graph.Vertices.OrderBy(v => v.Id))
            {
                writer.WriteLine($"{vertex.Id} {vertex.X} {vertex.Y} {vertex.Demand}");
            }

            writer.WriteLine();

            // Write depot info
            writer.WriteLine("# Depot");
            writer.WriteLine(instance.Graph.Depot?.Id.ToString() ?? "None");
        }
    }
}