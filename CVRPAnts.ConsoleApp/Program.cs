using System.IO;
using CVRPAnts.GraphLibrary;

namespace CVRPAnts.ConsoleApp;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("CVRP Graph Library Demo");
        Console.WriteLine("======================");

        // create graph

        var graph = new Graph();
        graph.AddVertex(0, 0, 0); // Depot
        graph.AddVertex(1, 1, 2, 5); // Customer 1
        graph.AddVertex(2, 3, 4, 10); // Customer 2
        graph.AddVertex(3, 5, 6, 15); // Customer 3
        graph.AddVertex(4, 7, 8, 20); // Customer 4

        string plotFilePath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots", DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_graph.png");
        GraphPlotter.PlotGraph(graph, plotFilePath);


        // // Create test data directory if it doesn't exist
        // string testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        // if (!Directory.Exists(testDataDir))
        // {
        //     Directory.CreateDirectory(testDataDir);
        // }

        // // Generate a small sample instance
        // Console.WriteLine("Generating a small CVRP instance...");
        // var instance = DataLoader.GenerateRandomInstance(
        //     vertexCount: 15,   // 15 vertices (1 depot + 14 customers)
        //     vehicleCount: 3,   // 3 vehicles
        //     capacity: 30,      // Each vehicle can carry 30 units
        //     maxDemand: 10,     // Each customer demands at most 10 units
        //     maxRouteLength: 150 // Maximum route length constraint
        // );

        // var graph = instance.Graph;
        // int vehicleCapacity = instance.VehicleCapacity;
        // int vehicleCount = instance.VehicleCount;
        // double maxRouteLength = instance.MaxRouteLength;

        // // Display instance information
        // Console.WriteLine($"Generated graph with {graph.VertexCount} vertices (1 depot + {graph.VertexCount - 1} customers)");
        // Console.WriteLine($"Vehicle count: {vehicleCount}");
        // Console.WriteLine($"Vehicle capacity: {vehicleCapacity}");
        // Console.WriteLine($"Maximum route length: {maxRouteLength}");
        // Console.WriteLine();

        // // Print vertex information
        // Console.WriteLine("Vertex information:");
        // foreach (var vertex in graph.Vertices.OrderBy(v => v.Id))
        // {
        //     Console.WriteLine($"Vertex {vertex.Id}: ({vertex.X:F2}, {vertex.Y:F2}), Demand: {vertex.Demand}");
        // }
        // Console.WriteLine();

        // // Save the instance to a file
        // string instanceFile = Path.Combine(testDataDir, "sample_instance.txt");
        // DataLoader.SaveGraphToFile(graph, vehicleCapacity, instanceFile);
        // Console.WriteLine($"Saved instance to {instanceFile}");
        // Console.WriteLine();

        // // Create a sample solution manually (this would normally be done by an algorithm)
        // Console.WriteLine("Creating a simple sample solution...");
        // var solution = new Solution(graph, vehicleCount, vehicleCapacity, maxRouteLength);

        // // Manually assign vertices to routes in a simple way
        // var customerVertices = graph.Vertices.Where(v => !v.IsDepot).ToList();
        // int verticesPerRoute = (int)Math.Ceiling(customerVertices.Count / (double)vehicleCount);

        // for (int i = 0; i < customerVertices.Count; i++)
        // {
        //     int routeIndex = Math.Min(i / verticesPerRoute, vehicleCount - 1);
        //     var route = solution.Routes[routeIndex];

        //     // Try to add this vertex to the route
        //     bool added = route.AddVertex(customerVertices[i]);
        //     if (!added)
        //     {
        //         Console.WriteLine($"Warning: Could not add vertex {customerVertices[i].Id} to route {routeIndex + 1}. " +
        //                            "Capacity or route length constraint violated.");
        //     }
        // }

        // // Complete all routes by returning to depot
        // foreach (var route in solution.Routes)
        // {
        //     route.CompleteRoute();
        // }

        // // Print solution
        // Console.WriteLine("\nSample solution:");
        // Console.WriteLine(solution);

        // // Check if solution is valid
        // Console.WriteLine($"Solution is {(solution.IsValid ? "valid" : "not valid")}");

        // // Save some benchmark instances for testing algorithms
        // Console.WriteLine("\nGenerating benchmark instances...");
        // DataLoader.GenerateBenchmarkInstances(testDataDir);
        // Console.WriteLine($"Benchmark instances saved to {testDataDir}");

        // Console.WriteLine("\nPress any key to exit...");
        // Console.ReadKey();
    }
}