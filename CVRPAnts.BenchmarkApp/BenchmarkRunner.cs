using System.Diagnostics;
using CVRPAnts.BenchmarkApp.Models;
using CVRPAnts.ParserLibrary;
using CVRPAnts.SolversLibrary;

namespace CVRPAnts.BenchmarkApp;

public class BenchmarkRunner(string testDataDir)
{
    private readonly List<(string, ICVRPSolver)> solvers = [];
    private readonly List<string> datasets = [];
    private readonly Dictionary<string, double> optimalCosts = new();
    private readonly Dictionary<string, int> optimalRoutes = new();

    private readonly string testDataDir = testDataDir;
    public string TestName { get; set; } = "BenchmarkTest";

    public void AddSolver(string name, ICVRPSolver solver)
    {
        solvers.Add((name, solver));
    }

    public void AddDataset(string name, double optimalCost, int optimalRoutes)
    {
        datasets.Add(name);
        this.optimalCosts[name] = optimalCost;
        this.optimalRoutes[name] = optimalRoutes;
    }

    public ICVRPSolver GetSolver(string name)
    {
        return solvers.FirstOrDefault(s => s.Item1 == name).Item2;
    }

    public List<BenchmarkResult> RunBenchmarks()
    {
        var results = new List<BenchmarkResult>();
        var date = DateTimeOffset.UtcNow;

        foreach (var datasetName in datasets)
        {
            // Load the dataset
            var datasetPath = Path.Combine(testDataDir, $"{datasetName}.vrp");
            Console.WriteLine($"Loading dataset: {datasetPath}");

            CVRPInstance? instance;
            try
            {
                instance = FileHelper.LoadInstanceFromFile(datasetPath, double.MaxValue);
                Console.WriteLine($"Loaded dataset with {instance.Graph.VertexCount} vertices");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dataset: {ex.Message}");
                continue;
            }

            foreach (var (solverName, solver) in solvers)
            {
                Console.WriteLine($"\nRunning solver {solverName} on dataset {datasetName}");

                var sw = Stopwatch.StartNew();
                CVRPSolution? solution;
                try
                {
                    if (solver is AntColonyBaseSolver antColonySolver)
                    {
                        antColonySolver.AntCount = instance.Graph.VertexCount;
                        solution = antColonySolver.Solve(instance);
                        sw.Stop();
                    }
                    else
                    {
                        solution = solver.Solve(instance);
                        sw.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error running solver: {ex.Message}");
                    continue;
                }

                double optimalCost = optimalCosts.TryGetValue(datasetName, out double value) ? value : 0;
                int optimalRoutesCount = optimalRoutes.TryGetValue(datasetName, out int count) ? count : 0;
                double costDifference = optimalCost > 0
                    ? (solution.TotalLength - optimalCost) / optimalCost * 100
                    : 0;

                // Calculate average route utilization (total demand / total capacity)
                var sumRouteUtilization = 0.0;
                foreach (var route in solution.Routes)
                {
                    sumRouteUtilization += (double)route.TotalDemand / instance.VehicleCapacity * 100;
                }
                var averageRouteUtilization = sumRouteUtilization / solution.RoutesCount;

                var result = new BenchmarkResult
                {
                    TestName = TestName,
                    Date = date,
                    Solver = solverName,
                    TestData = datasetName,
                    Cost = solution.TotalLength,
                    RoutesCount = solution.RoutesCount,
                    Time = (long)sw.Elapsed.TotalMilliseconds,
                    OptimalCost = optimalCost,
                    OptimalRoutesCount = optimalRoutesCount,
                    CostDifference = costDifference,
                    Seed = solver.Seed,
                    AverageRouteUtilization = averageRouteUtilization,
                };

                Console.WriteLine($"Result: Cost={result.Cost:F2}, Vehicles={result.RoutesCount}, Time={result.Time:F3}s");
                results.Add(result);
                solver.ProgressWriter?.SaveProgress(result.TestId);
            }
        }

        return results;
    }
}