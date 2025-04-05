using CVRPAnts.GraphLibrary;

namespace CVRPAnts.ParserLibrary;

/// <summary>
/// Parser for CVRPLIB format files
/// </summary>
public class CVRPParser
{
    /// <summary>
    /// Parse a CVRPLIB format file and return a VRPInstance
    /// </summary>
    /// <param name="filePath">Path to the CVRPLIB file</param>
    /// <returns>A VRPInstance containing the graph and problem parameters</returns>
    public static CVRPInstance ParseVRPFile(string filePath, int vehicleCount, double maxRouteDistance)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var lines = File.ReadAllLines(filePath);

        // Parse the header specifications
        var specifications = ParseSpecifications(lines);

        // Extract key information
        int dimension = GetIntSpecification(specifications, "DIMENSION");
        int capacity = GetIntSpecification(specifications, "CAPACITY");

        // Create graph
        var graph = new Graph();

        // Parse the data sections
        var nodes = ParseNodeCoordinateSection(lines);
        var demands = ParseDemandSection(lines);
        var depotId = ParseDepotSection(lines);

        // Add vertices to the graph
        foreach (var nodeId in nodes.Keys)
        {
            var (x, y) = nodes[nodeId];
            int demand = demands.ContainsKey(nodeId) ? demands[nodeId] : 0;
            bool isDepot = depotId == nodeId;

            graph.AddVertex(nodeId, x, y, demand, isDepot);
        }

        var instance = new CVRPInstance
        {
            Graph = graph,
            VehicleCapacity = capacity,
            VehicleCount = vehicleCount,
            MaxRouteDistance = maxRouteDistance,
            Name = GetSpecification(specifications, "NAME")?.Trim() ?? Path.GetFileNameWithoutExtension(filePath)
        };

        return instance;
    }

    private static Dictionary<string, string> ParseSpecifications(string[] lines)
    {
        var specs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("//"))
            {
                continue;
            }

            // Stop when we reach a section header
            if (trimmedLine.Contains("_SECTION"))
            {
                break;
            }

            // Parse specification lines
            var colonIndex = trimmedLine.IndexOf(':');
            if (colonIndex > 0)
            {
                var key = trimmedLine.Substring(0, colonIndex).Trim();
                var value = trimmedLine.Substring(colonIndex + 1).Trim();
                specs[key] = value;
            }
        }

        return specs;
    }

    private static Dictionary<int, (int x, int y)> ParseNodeCoordinateSection(string[] lines)
    {
        var nodes = new Dictionary<int, (int x, int y)>();
        bool inSection = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Check if we're entering the section
            if (trimmedLine == "NODE_COORD_SECTION")
            {
                inSection = true;
                continue;
            }

            // Check if we're leaving the section
            if (inSection && (trimmedLine.Contains("_SECTION") || trimmedLine == "EOF"))
            {
                break;
            }

            // Parse node coordinates
            if (inSection && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                var parts = trimmedLine.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3 && int.TryParse(parts[0], out int nodeId))
                {
                    if (int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                    {
                        nodes[nodeId] = (x, y);
                    }
                }
            }
        }

        return nodes;
    }

    private static Dictionary<int, int> ParseDemandSection(string[] lines)
    {
        var demands = new Dictionary<int, int>();
        bool inSection = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Check if we're entering the section
            if (trimmedLine == "DEMAND_SECTION")
            {
                inSection = true;
                continue;
            }

            // Check if we're leaving the section
            if (inSection && (trimmedLine.Contains("_SECTION") || trimmedLine == "EOF"))
            {
                break;
            }

            // Parse demand values
            if (inSection && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                var parts = trimmedLine.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && int.TryParse(parts[0], out int nodeId))
                {
                    if (int.TryParse(parts[1], out int demand))
                    {
                        demands[nodeId] = demand;
                    }
                }
            }
        }

        return demands;
    }

    private static int ParseDepotSection(string[] lines)
    {
        var depots = new HashSet<int>();
        bool inSection = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Check if we're entering the section
            if (trimmedLine == "DEPOT_SECTION")
            {
                inSection = true;
                continue;
            }

            // Check if we're leaving the section
            if (inSection && (trimmedLine.Contains("_SECTION") || trimmedLine == "EOF"))
            {
                break;
            }

            // Parse depot IDs, ignoring -1 which marks the end of the depot section
            if (inSection && !string.IsNullOrWhiteSpace(trimmedLine))
            {
                if (int.TryParse(trimmedLine, out int depotId) && depotId != -1)
                {
                    depots.Add(depotId);
                }
            }
        }

        if (depots.Count == 0)
        {
            throw new FormatException("No depot found in the DEPOT_SECTION.");
        }

        if (depots.Count > 1)
        {
            throw new FormatException("Multiple depots found in the DEPOT_SECTION. Only one depot is allowed.");
        }

        return depots.First();
    }

    private static string? GetSpecification(Dictionary<string, string> specs, string key)
    {
        return specs.TryGetValue(key, out var value) ? value : null;
    }

    private static int GetIntSpecification(Dictionary<string, string> specs, string key)
    {
        if (specs.TryGetValue(key, out var value) && int.TryParse(value, out int result))
        {
            return result;
        }
        return 0;
    }

    private static double GetDoubleSpecification(Dictionary<string, string> specs, string key)
    {
        if (specs.TryGetValue(key, out var value) && double.TryParse(value, out double result))
        {
            return result;
        }
        return 0;
    }
}