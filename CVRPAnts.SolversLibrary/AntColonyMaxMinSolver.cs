namespace CVRPAnts.SolversLibrary;
public class AntColonyMaxMinSolver(AntColonyParameters parameters, int seed, IProgressWriter? progressWriter)
    : AntColonyBaseSolver(parameters, seed, progressWriter)
{
    public double PheromoneMax { get; set; } = 10.0;
    public double PheromoneMin { get; set; } = 0.1;
    public bool OnlyBestUpdates { get; set; } = true;
    public int StagnationLimit { get; set; } = 20;

    private int iterationsWithoutImprovement;
    private CVRPSolution? iterationBestSolution;
    private CVRPSolution? globalBestSolution;

    public override CVRPSolution Solve(CVRPInstance instance)
    {
        this.graph = instance.Graph;
        this.capacity = instance.VehicleCapacity;
        this.maxRouteDistance = instance.MaxRouteDistance;
        this.depot = instance.Graph.Depot ?? throw new InvalidOperationException("Depot vertex not found");

        this.InitializePheromones();

        this.globalBestSolution = null!;
        double bestSolutionLength = double.MaxValue;
        this.iterationsWithoutImprovement = 0;

        // Main ACO loop
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int iteration = 0; iteration < this.MaxIterations; iteration++)
        {
            var antSolutions = new List<CVRPSolution>();
            this.iterationBestSolution = null!;
            double iterationBestLength = double.MaxValue;

            // Each ant constructs a solution
            for (int ant = 0; ant < this.AntCount; ant++)
            {
                var solution = this.ConstructSolution();
                solution = this.OptimizeSolution(solution);
                antSolutions.Add(solution);

                // Update iteration best solution
                if (solution.TotalLength < iterationBestLength && solution.IsValid)
                {
                    this.iterationBestSolution = solution.Clone();
                    iterationBestLength = solution.TotalLength;
                }
            }

            // Update global best solution if needed
            if (this.iterationBestSolution != null && iterationBestLength < bestSolutionLength)
            {
                this.globalBestSolution = this.iterationBestSolution.Clone();
                bestSolutionLength = iterationBestLength;
                this.iterationsWithoutImprovement = 0;
                Console.WriteLine($"New best solution: {bestSolutionLength:F2} in iteration {iteration}");
            }
            else
            {
                this.iterationsWithoutImprovement++;
            }

            // Update pheromones using Max-Min approach
            this.UpdateMaxMinPheromones(this.iterationBestSolution!, this.globalBestSolution!);

            // Reset if stagnation detected
            if (this.iterationsWithoutImprovement >= this.StagnationLimit)
            {
                Console.WriteLine($"Stagnation detected after {this.iterationsWithoutImprovement} iterations, resetting pheromones");
                this.ResetPheromones();
                this.iterationsWithoutImprovement = 0;
            }

            this.progressWriter?.WriteProgress(
                iteration,
                stopwatch.ElapsedMilliseconds,
                this.globalBestSolution.TotalLength,
                this.globalBestSolution.RoutesCount);
        }

        return this.globalBestSolution!;
    }

    private void ResetPheromones()
    {
        foreach (var edge in this.graph!.Edges)
        {
            // Reset to initial value
            edge.SetPheromone(this.InitialPheromone);
        }
    }

    private void UpdateMaxMinPheromones(CVRPSolution iterationBest, CVRPSolution globalBest)
    {
        // Evaporation
        foreach (var edge in this.graph!.Edges)
        {
            edge.SetPheromone(edge.Pheromone * (1 - this.EvaporationRate));
        }

        // In MMAS, only the best ant deposits pheromone (either iteration best or global best)
        var bestSolution = this.OnlyBestUpdates ? globalBest : iterationBest;

        // Amount of pheromone depends on solution quality
        double pheromoneAmount = this.Q / bestSolution.TotalLength;

        foreach (var route in bestSolution.Routes)
        {
            for (int i = 0; i < route.Vertices.Count - 1; i++)
            {
                var edge = this.graph.GetEdge(route.Vertices[i].Id, route.Vertices[i + 1].Id);
                edge.UpdatePheromone(pheromoneAmount);
            }

            // Close the loop back to depot
            if (route.Vertices.Count > 1)
            {
                var lastEdge = this.graph.GetEdge(route.Vertices.Last().Id, this.depot!.Id);
                lastEdge.UpdatePheromone(pheromoneAmount);
            }
        }

        // Enforce min-max limits
        foreach (var edge in this.graph!.Edges)
        {
            double current = edge.Pheromone;
            if (current < this.PheromoneMin)
            {
                edge.SetPheromone(this.PheromoneMin);
            }
            else if (current > this.PheromoneMax)
            {
                edge.SetPheromone(this.PheromoneMax);
            }
        }
    }
}