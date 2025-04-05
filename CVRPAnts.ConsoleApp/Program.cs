using System.Runtime.Versioning;
using CVRPAnts.ParserLibrary;
using CVRPAnts.SolversLibrary;

namespace CVRPAnts.ConsoleApp;

internal class Program
{
    [STAThread]
    [SupportedOSPlatform("windows")]
    private static void Main(string[] args)
    {
        Console.WriteLine("CVRP Graph Library Demo");
        Console.WriteLine("======================");

        // 1. Parse a CVRP instance from a VRPLIB file
        Console.WriteLine("\n2. Parsing a CVRP instance from VRPLIB file:");

        CVRPInstance? vrpInstance = null;
        try
        {
            string testFilePath = @"D:\Projects\MSI2-ants\TestData\X-n101-k25.vrp";
            vrpInstance = FileHelper.LoadInstanceFromFile(testFilePath, 1000);

            Console.WriteLine($"Loaded: {vrpInstance}");
            Console.WriteLine($"Depot: Vertex {vrpInstance.Graph.Depot?.Id}");
            Console.WriteLine($"Customer count: {vrpInstance.Graph.VertexCount - 1}");

            // Plot the loaded graph
            string vrpPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Instances",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}.png");
            GraphPlotter.PlotGraph(vrpInstance.Graph, vrpPlotPath);

            Console.WriteLine($"Graph plotted to: {vrpPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading VRP file: {ex.Message}");
        }

        // 2. Parse a CVRP solution from a text file
        Console.WriteLine("\n3. Parsing a CVRP solution from text file:");
        try
        {
            string solutionFilePath = @"D:\Projects\MSI2-ants\TestData\X-n101-k25.sol";
            var solution = CVRPSolutionParser.ParseSolutionFile(solutionFilePath, vrpInstance!.Graph, 1000, int.MaxValue);

            Console.WriteLine($"Parsed solution: {solution}");

            var solutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}.png");
            GraphPlotter.PlotSolution(solution, solutionPlotPath);

            Console.WriteLine($"Solution plotted to: {solutionPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading solution file: {ex.Message}");
        }
    }
}