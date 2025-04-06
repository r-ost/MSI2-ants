using CVRPAnts.GraphLibrary;

namespace CVRPAnts.SolversLibrary;

public class GreedySolver : ICVRPSolver
{
    public IProgressWriter? ProgressWriter { get; set; } = null;
    public int Seed { get; set; } = 0;

    public CVRPSolution Solve(CVRPInstance instance)
    {
        // w każdym kroku wybierz najblizszego klienta, dla ktorego trasa bedzie valid
        // powtarzaj az do obsluzenia wszystkich klientów

        if (instance.Graph.Depot is null)
        {
            throw new InvalidOperationException("Depot vertex not found in the graph");
        }

        var unvisitedCustomers = new HashSet<int>(
            instance.Graph.Vertices
                .Select(v => v.Id)
                .Where(id => id != instance.Graph.Depot!.Id)
        );

        var solutionRoutes = new List<Route>();
        var currentRoute = new Route(instance.Graph, instance.VehicleCapacity, instance.MaxRouteDistance);
        while (unvisitedCustomers.Count > 0)
        {
            var currentVertex = currentRoute.GetLastVertex();

            var unvisitedVertices = instance.Graph.Vertices
                .Where(v => unvisitedCustomers.Contains(v.Id) && v.Id != currentVertex.Id)
                .ToList();

            List<Vertex> unvisitedVerticesByDistance = [.. unvisitedVertices.OrderBy(v => instance.Graph.GetEdge(currentVertex.Id, v.Id).Weight)];
            var newVertexAddedToRoute = false;
            for (int i = 0; i < unvisitedVerticesByDistance.Count; i++)
            {
                var vertex = unvisitedVerticesByDistance[i];
                currentRoute.AddVertex(vertex);
                if (!currentRoute.IsValid)
                {
                    currentRoute.RemoveLastVertex();
                    continue;
                }
                unvisitedCustomers.Remove(vertex.Id);
                newVertexAddedToRoute = true;
                break;
            }

            if (!newVertexAddedToRoute)
            {
                // if no vertex was added, we need to finish the current route and start a new one
                solutionRoutes.Add(currentRoute);
                currentRoute = new Route(instance.Graph, instance.VehicleCapacity, instance.MaxRouteDistance);
            }
        }

        solutionRoutes.Add(currentRoute);
        return new CVRPSolution(instance.Graph, solutionRoutes);
    }
}