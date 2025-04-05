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
        Console.WriteLine("CVRP Demo");
        Console.WriteLine("======================");

        var instanceName = "X-n101-k25";

        var testDirPath = @"D:\Projects\MSI2-ants\TestData";

        var instanceFilePath = Path.Combine(testDirPath, instanceName + ".vrp");
        var solutionFilePath = Path.Combine(testDirPath, instanceName + ".sol");

        Console.WriteLine($"\n1. Parsing a CVRP instance from VRPLIB file: {instanceFilePath}, {DateTimeOffset.UtcNow}");
        CVRPInstance? vrpInstance = null;
        try
        {
            vrpInstance = FileHelper.LoadInstanceFromFile(instanceFilePath, int.MaxValue);

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

        // Solve the CVRP instance using the greedy solver
        Console.WriteLine($"\n2. Solving the CVRP instance using the greedy solver {DateTimeOffset.UtcNow}:");
        try
        {
            if (vrpInstance == null)
            {
                throw new InvalidOperationException("VRP instance is not loaded.");
            }

            var greedySolver = new GreedySolver();
            var greedySolution = greedySolver.Solve(vrpInstance);
            Console.WriteLine($"Greedy solution: {greedySolution}");

            // Plot the greedy solution
            var greedySolutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}_greedy.png");
            GraphPlotter.PlotSolution(greedySolution, greedySolutionPlotPath);

            Console.WriteLine($"Greedy solution plotted to: {greedySolutionPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error solving VRP instance with greedy solver: {ex.Message}");
        }

        // Solve the CVRP instance using the ant colony solver
        Console.WriteLine($"\n3. Solving the CVRP instance using the ant colony solver {DateTimeOffset.UtcNow}:");
        try
        {
            if (vrpInstance == null)
            {
                throw new InvalidOperationException("VRP instance is not loaded.");
            }

            var acoSolver = new AntColonySolver
            {
                AntCount = 200,
                MaxIterations = 200,
                Alpha = 1.0,
                Beta = 5.0,
                EvaporationRate = 0.2,
                Q = 100,
                InitialPheromone = 0.1
            };

            var acoSolution = acoSolver.Solve(vrpInstance);
            Console.WriteLine($"Ant Colony solution: {acoSolution}");

            // Plot the ACO solution
            var acoSolutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}_aco.png");
            GraphPlotter.PlotSolution(acoSolution, acoSolutionPlotPath);

            Console.WriteLine($"Ant Colony solution plotted to: {acoSolutionPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error solving VRP instance with ant colony solver: {ex.Message}");
        }

        // Parse a CVRP solution from a text file
        Console.WriteLine($"\n4. Parsing an optimal CVRP solution from text file: {solutionFilePath}, {DateTimeOffset.UtcNow}");
        try
        {
            var solution = CVRPSolutionParser.ParseSolutionFile(solutionFilePath, vrpInstance!.Graph, 1000, int.MaxValue);

            Console.WriteLine($"Parsed solution: {solution}");

            var solutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}_optimal.png");
            GraphPlotter.PlotSolution(solution, solutionPlotPath);

            Console.WriteLine($"Solution plotted to: {solutionPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading solution file: {ex.Message}");
        }
    }
}
