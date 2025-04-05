namespace CVRPAnts.SolversLibrary;

using CVRPAnts.GraphLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

public class AntColonySolver : ICVRPSolver
{
    private readonly Random random = new(1337);

    // ACO parameters
    public int AntCount { get; set; } = 10;
    public int MaxIterations { get; set; } = 100;
    public double Alpha { get; set; } = 1.0;  // Pheromone importance
    public double Beta { get; set; } = 2.0;   // Heuristic importance
    public double Gamma { get; set; } = 1.0;  // Distance importance
    public double EvaporationRate { get; set; } = 0.5;
    public double Q { get; set; } = 100;      // Pheromone deposit factor
    public double InitialPheromone { get; set; } = 0.1;

    private Graph? graph;
    private int capacity;
    private double maxRouteDistance;
    private Vertex? depot;

    public AntColonySolver(int? seed = null)
    {
        this.random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public CVRPSolution Solve(CVRPInstance instance)
    {
        this.graph = instance.Graph;
        this.capacity = instance.VehicleCapacity;
        this.maxRouteDistance = instance.MaxRouteDistance;
        this.depot = instance.Graph.Depot ?? throw new InvalidOperationException("Depot vertex not found");

        InitializePheromones();

        CVRPSolution bestSolution = null!;
        double bestSolutionLength = double.MaxValue;

        // Main ACO loop
        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            var antSolutions = new List<CVRPSolution>();

            // Each ant constructs a solution
            for (int ant = 0; ant < AntCount; ant++)
            {
                var solution = ConstructSolution();
                antSolutions.Add(solution);

                // Update best solution if needed
                if (solution.TotalLength < bestSolutionLength && solution.IsValid)
                {
                    bestSolution = solution.Clone();
                    bestSolutionLength = solution.TotalLength;
                    Console.WriteLine($"New best solution: {bestSolution.TotalLength:F2} in iteration {iteration}");
                }
            }

            // Update pheromones
            UpdatePheromones(antSolutions);
        }

        return bestSolution;
    }

    private void InitializePheromones()
    {
        foreach (var edge in graph!.Edges)
        {
            edge.SetPheromone(InitialPheromone);
        }
    }

    private CVRPSolution ConstructSolution()
    {
        var unvisitedCustomers = new HashSet<int>(
            graph!.Vertices
                .Where(v => v.Id != depot!.Id)
                .Select(v => v.Id)
        );

        var routes = new List<Route>();

        while (unvisitedCustomers.Count > 0)
        {
            var route = BuildRoute(unvisitedCustomers);
            routes.Add(route);
        }

        return new CVRPSolution(graph!, routes);
    }

    private Route BuildRoute(HashSet<int> unvisitedCustomers)
    {
        var route = new Route(graph!, capacity, maxRouteDistance);

        // add random vertex as beginning of the route
        var randomCustomerId = unvisitedCustomers.ElementAt(random.Next(unvisitedCustomers.Count));
        var randomCustomer = graph!.GetVertex(randomCustomerId);
        route.AddVertex(randomCustomer);
        unvisitedCustomers.Remove(randomCustomerId);
        var currentVertex = randomCustomer;

        while (true)
        {
            var nextVertex = SelectNextVertex(currentVertex!, route, unvisitedCustomers);

            if (nextVertex is null)
            {
                // Can't add more customers to this route
                break;
            }

            route.AddVertex(nextVertex);
            unvisitedCustomers.Remove(nextVertex.Id);
            currentVertex = nextVertex;
        }

        return route;
    }

    private Vertex? SelectNextVertex(Vertex current, Route route, HashSet<int> unvisitedCustomers)
    {
        if (unvisitedCustomers.Count == 0)
        {
            return null;
        }

        var candidates = unvisitedCustomers
            .Select(id => graph!.GetVertex(id))
            .ToList();

        // Filter candidates that would make the route invalid
        var validCandidates = new List<(Vertex vertex, double probability)>();
        double totalProbability = 0;

        foreach (var candidate in candidates)
        {
            // Check if adding this customer would exceed capacity or max distance
            route.AddVertex(candidate);
            if (!route.IsValid)
            {
                route.RemoveLastVertex();
                continue;
            }
            route.RemoveLastVertex();

            var edge = graph!.GetEdge(current.Id, candidate.Id);
            double pheromone = Math.Pow(edge.Pheromone, Alpha);
            double heuristic = Math.Pow(1.0 / edge.Weight, Beta);

            double probability = pheromone * heuristic;

            validCandidates.Add((candidate, probability));
            totalProbability += probability;
        }

        if (validCandidates.Count == 0)
        {
            return null;
        }

        // Select next vertex using roulette wheel selection
        double randomValue = random.NextDouble() * totalProbability;
        double cumulativeProbability = 0;

        foreach (var candidate in validCandidates)
        {
            cumulativeProbability += candidate.probability;
            if (cumulativeProbability >= randomValue)
            {
                return candidate.vertex;
            }
        }

        // Fallback to first candidate if roulette wheel fails (shouldn't happen)
        return validCandidates[0].vertex;
    }

    private void UpdatePheromones(List<CVRPSolution> solutions)
    {
        // Evaporation
        foreach (var edge in graph!.Edges)
        {
            edge.SetPheromone(edge.Pheromone * (1 - EvaporationRate));
        }

        // Deposit new pheromones
        foreach (var solution in solutions)
        {
            // Amount of pheromone depends on solution quality
            double pheromoneAmount = Q / solution.TotalLength;

            foreach (var route in solution.Routes)
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
        }
    }
}