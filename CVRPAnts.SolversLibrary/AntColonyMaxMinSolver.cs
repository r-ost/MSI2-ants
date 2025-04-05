namespace CVRPAnts.SolversLibrary;

using CVRPAnts.GraphLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

public class AntColonyMaxMinSolver : AntColonyBaseSolver
{
    public double PheromoneMax { get; set; } = 10.0;
    public double PheromoneMin { get; set; } = 0.1;
    public bool OnlyBestUpdates { get; set; } = true;
    public int StagnationLimit { get; set; } = 20;

    private int iterationsWithoutImprovement;
    private CVRPSolution? iterationBestSolution;
    private CVRPSolution? globalBestSolution;

    public AntColonyMaxMinSolver(int? seed = null) : base(seed)
    {
    }

    public override CVRPSolution Solve(CVRPInstance instance)
    {
        this.graph = instance.Graph;
        this.capacity = instance.VehicleCapacity;
        this.maxRouteDistance = instance.MaxRouteDistance;
        this.depot = instance.Graph.Depot ?? throw new InvalidOperationException("Depot vertex not found");

        InitializePheromones();
        CalculatePheromoneMinMax();

        globalBestSolution = null!;
        double bestSolutionLength = double.MaxValue;
        iterationsWithoutImprovement = 0;

        // Main ACO loop
        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            var antSolutions = new List<CVRPSolution>();
            iterationBestSolution = null!;
            double iterationBestLength = double.MaxValue;

            // Each ant constructs a solution
            for (int ant = 0; ant < AntCount; ant++)
            {
                var solution = ConstructSolution();
                solution = OptimizeSolution(solution);
                antSolutions.Add(solution);

                // Update iteration best solution
                if (solution.TotalLength < iterationBestLength && solution.IsValid)
                {
                    iterationBestSolution = solution.Clone();
                    iterationBestLength = solution.TotalLength;
                }
            }

            // Update global best solution if needed
            if (iterationBestSolution != null && iterationBestLength < bestSolutionLength)
            {
                globalBestSolution = iterationBestSolution.Clone();
                bestSolutionLength = iterationBestLength;
                iterationsWithoutImprovement = 0;
                Console.WriteLine($"New best solution: {bestSolutionLength:F2} in iteration {iteration}");
            }
            else
            {
                iterationsWithoutImprovement++;
            }

            // Update pheromones using Max-Min approach
            UpdateMaxMinPheromones(iterationBestSolution!, globalBestSolution!);

            // Reset if stagnation detected
            if (iterationsWithoutImprovement >= StagnationLimit)
            {
                Console.WriteLine($"Stagnation detected after {iterationsWithoutImprovement} iterations, resetting pheromones");
                ResetPheromones();
                iterationsWithoutImprovement = 0;
            }
        }

        return globalBestSolution!;
    }

    private void CalculatePheromoneMinMax()
    {
        // These calculations can be refined based on problem-specific details
        PheromoneMax = 1.0 / (EvaporationRate * InitialPheromone);
        PheromoneMin = PheromoneMax * 0.1;
    }

    private void ResetPheromones()
    {
        foreach (var edge in graph!.Edges)
        {
            // Reset to initial value
            edge.SetPheromone(InitialPheromone);
        }
    }

    private void UpdateMaxMinPheromones(CVRPSolution iterationBest, CVRPSolution globalBest)
    {
        // Evaporation
        foreach (var edge in graph!.Edges)
        {
            edge.SetPheromone(edge.Pheromone * (1 - EvaporationRate));
        }

        // In MMAS, only the best ant deposits pheromone (either iteration best or global best)
        var bestSolution = OnlyBestUpdates ? globalBest : iterationBest;

        // Amount of pheromone depends on solution quality
        double pheromoneAmount = Q / bestSolution.TotalLength;

        foreach (var route in bestSolution.Routes)
        {
            for (int i = 0; i < route.Vertices.Count - 1; i++)
            {
                var edge = graph.GetEdge(route.Vertices[i].Id, route.Vertices[i + 1].Id);
                edge.UpdatePheromone(pheromoneAmount);
            }

            // Close the loop back to depot
            if (route.Vertices.Count > 1)
            {
                var lastEdge = graph.GetEdge(route.Vertices.Last().Id, depot!.Id);
                lastEdge.UpdatePheromone(pheromoneAmount);
            }
        }

        // Enforce min-max limits
        foreach (var edge in graph!.Edges)
        {
            double current = edge.Pheromone;
            if (current < PheromoneMin)
            {
                edge.SetPheromone(PheromoneMin);
            }
            else if (current > PheromoneMax)
            {
                edge.SetPheromone(PheromoneMax);
            }
        }
    }
}