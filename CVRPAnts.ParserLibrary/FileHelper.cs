using CVRPAnts.SolversLibrary;

namespace CVRPAnts.ParserLibrary;

public static class FileHelper
{
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
                var instance = CVRPInstanceParser.ParseVRPFile(file, maxRouteDistance);
                instances[instance.Name] = instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing file {file}: {ex.Message}");
            }
        }

        return instances;
    }

    public static CVRPInstance LoadInstanceFromFile(
        string filePath,
        double maxRouteDistance)
    {
        return CVRPInstanceParser.ParseVRPFile(filePath, maxRouteDistance);
    }

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
