namespace CVRPAnts.SolversLibrary;
public class AntColony2OptSolver(AntColonyParameters parameters, int seed, IProgressWriter? progressWriter)
    : AntColonyBaseSolver(parameters, seed, progressWriter)
{
    protected override CVRPSolution OptimizeSolution(CVRPSolution solution)
    {
        var optimizedRoutes = new List<Route>();

        foreach (var route in solution.Routes)
        {
            var optimizedRoute = Apply2Opt(route);
            optimizedRoutes.Add(optimizedRoute);
        }

        return new CVRPSolution(graph!, optimizedRoutes);
    }

    private Route Apply2Opt(Route route)
    {
        if (route.Vertices.Count <= 3)
        {
            return route;
        }

        bool improved;
        Route bestRoute = route.Clone();

        do
        {
            improved = false;
            double bestDistance = bestRoute.Length;

            // Skip the depot (index 0) in the outer loop
            for (int i = 1; i < bestRoute.Vertices.Count - 1; i++)
            {
                for (int j = i + 1; j < bestRoute.Vertices.Count; j++)
                {
                    Route newRoute = TwoOptSwap(bestRoute, i, j);

                    if (newRoute.IsValid && newRoute.Length < bestDistance)
                    {
                        bestRoute = newRoute;
                        bestDistance = newRoute.Length;
                        improved = true;
                    }
                }
            }
        } while (improved);

        return bestRoute;
    }

    private Route TwoOptSwap(Route route, int i, int j)
    {
        var newRoute = new Route(graph!, route.Capacity, route.MaxRouteLength);

        // Add vertices before the swap position
        for (int k = 1; k < i; k++)
        {
            newRoute.AddVertex(route.Vertices[k]);
        }

        // Add vertices in the swapped segment in reverse order
        for (int k = j; k >= i; k--)
        {
            newRoute.AddVertex(route.Vertices[k]);
        }

        // Add vertices after the swap position
        for (int k = j + 1; k < route.Vertices.Count; k++)
        {
            newRoute.AddVertex(route.Vertices[k]);
        }

        return newRoute;
    }
}