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

        var instanceName = "X-n209-k16";

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

        // // Solve the CVRP instance using the greedy solver
        // Console.WriteLine($"\n2. Solving the CVRP instance using the greedy solver {DateTimeOffset.UtcNow}:");
        // try
        // {
        //     if (vrpInstance == null)
        //     {
        //         throw new InvalidOperationException("VRP instance is not loaded.");
        //     }

        //     var greedySolver = new GreedySolver();
        //     var greedySolution = greedySolver.Solve(vrpInstance);
        //     Console.WriteLine($"Greedy solution: {greedySolution}");

        //     // Plot the greedy solution
        //     var greedySolutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
        //         DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}_greedy.png");
        //     GraphPlotter.PlotSolution(greedySolution, greedySolutionPlotPath);

        //     Console.WriteLine($"Greedy solution plotted to: {greedySolutionPlotPath}");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Error solving VRP instance with greedy solver: {ex.Message}");
        // }

        // // Solve the CVRP instance using the standard ant colony solver
        // Console.WriteLine($"\n3. Solving the CVRP instance using the standard ant colony solver {DateTimeOffset.UtcNow}:");
        // try
        // {
        //     if (vrpInstance == null)
        //     {
        //         throw new InvalidOperationException("VRP instance is not loaded.");
        //     }

        //     var acoSolver = new AntColonySolver
        //     {
        //         AntCount = 200,
        //         MaxIterations = 200,
        //         Alpha = 1.0,
        //         Beta = 5.0,
        //         EvaporationRate = 0.1,
        //         Q = 100,
        //         InitialPheromone = 0.1
        //     };

        //     var acoSolution = acoSolver.Solve(vrpInstance);
        //     Console.WriteLine($"Standard Ant Colony solution: {acoSolution}");

        //     // Plot the ACO solution
        //     var acoSolutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
        //         DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}_aco.png");
        //     GraphPlotter.PlotSolution(acoSolution, acoSolutionPlotPath);

        //     Console.WriteLine($"Ant Colony solution plotted to: {acoSolutionPlotPath}");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Error solving VRP instance with ant colony solver: {ex.Message}");
        // }

        // Solve the CVRP instance using the 2-opt ant colony solver
        Console.WriteLine($"\n4. Solving the CVRP instance using the 2-opt ant colony solver {DateTimeOffset.UtcNow}:");
        try
        {
            if (vrpInstance == null)
            {
                throw new InvalidOperationException("VRP instance is not loaded.");
            }

            var aco2OptSolver = new AntColony2OptSolver
            {
                AntCount = 400,
                MaxIterations = 100,
                Alpha = 1.0,
                Beta = 5.0,
                EvaporationRate = 0.1,
                Q = 100,
                InitialPheromone = 0.1
            };

            var aco2OptSolution = aco2OptSolver.Solve(vrpInstance);
            Console.WriteLine($"Ant Colony 2-Opt solution: {aco2OptSolution}");

            // Plot the ACO 2-Opt solution
            var aco2OptSolutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}_aco2opt.png");
            GraphPlotter.PlotSolution(aco2OptSolution, aco2OptSolutionPlotPath);

            Console.WriteLine($"Ant Colony 2-Opt solution plotted to: {aco2OptSolutionPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error solving VRP instance with ant colony 2-opt solver: {ex.Message}");
        }

        // Solve the CVRP instance using the Max-Min ant colony solver
        Console.WriteLine($"\n5. Solving the CVRP instance using the Max-Min ant colony solver {DateTimeOffset.UtcNow}:");
        try
        {
            if (vrpInstance == null)
            {
                throw new InvalidOperationException("VRP instance is not loaded.");
            }

            var acoMaxMinSolver = new AntColonyMaxMinSolver
            {
                AntCount = 400,
                MaxIterations = 100,
                Alpha = 1.0,
                Beta = 5.0,
                EvaporationRate = 0.1,
                Q = 100,
                InitialPheromone = 0.1,
                PheromoneMin = 0.01,
                PheromoneMax = 10.0,
                OnlyBestUpdates = true,
                StagnationLimit = 10
            };

            var acoMaxMinSolution = acoMaxMinSolver.Solve(vrpInstance);
            Console.WriteLine($"Max-Min Ant Colony solution: {acoMaxMinSolution}");

            // Plot the ACO Max-Min solution
            var acoMaxMinSolutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance.Name}_acomaxmin.png");
            GraphPlotter.PlotSolution(acoMaxMinSolution, acoMaxMinSolutionPlotPath);

            Console.WriteLine($"Max-Min Ant Colony solution plotted to: {acoMaxMinSolutionPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error solving VRP instance with Max-Min ant colony solver: {ex.Message}");
        }

        // Parse a CVRP solution from a text file
        Console.WriteLine($"\n6. Parsing an optimal CVRP solution from text file: {solutionFilePath}, {DateTimeOffset.UtcNow}");
        try
        {
            var solution = CVRPSolutionParser.ParseSolutionFile(solutionFilePath, vrpInstance!.Graph, vrpInstance!.VehicleCapacity, vrpInstance!.MaxRouteDistance);

            Console.WriteLine($"Parsed solution: {solution}");

            var solutionPlotPath = Path.Combine(@"D:\Projects\MSI2-ants\CVRPAnts.ConsoleApp\Plots\Solutions",
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + $"_{vrpInstance!.Name}_optimal.png");
            GraphPlotter.PlotSolution(solution, solutionPlotPath);

            Console.WriteLine($"Solution plotted to: {solutionPlotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading solution file: {ex.Message}");
        }
    }
}
