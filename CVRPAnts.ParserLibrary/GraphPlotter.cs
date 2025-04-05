using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using CVRPAnts.GraphLibrary;
using CVRPAnts.SolversLibrary;

namespace CVRPAnts.ParserLibrary;

/// <summary>
/// Utility for plotting graphs visually
/// </summary>
public static class GraphPlotter
{
    /// <summary>
    /// Plots a graph to an image file
    /// </summary>
    /// <param name="graph">Graph to plot</param>
    /// <param name="filePath">Where to save the image</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    [SupportedOSPlatform("windows")]
    public static void PlotGraph(Graph graph, string filePath, int width = 1200, int height = 1200)
    {
        if (graph == null || graph.VertexCount == 0)
        {
            throw new ArgumentException("Graph is empty or null");
        }

        // Create directory if it doesn't exist
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Calculate bounds of the graph
        int minX = graph.Vertices.Min(v => v.X);
        int minY = graph.Vertices.Min(v => v.Y);
        int maxX = graph.Vertices.Max(v => v.X);
        int maxY = graph.Vertices.Max(v => v.Y);

        // Add a margin
        int margin = Math.Max(maxX - minX, maxY - minY) / 20;
        minX -= margin;
        minY -= margin;
        maxX += margin;
        maxY += margin;

        // Create a bitmap and graphics object
        using Bitmap bitmap = new(width, height);
        using Graphics graphics = Graphics.FromImage(bitmap);

        // Set up the graphics object
        graphics.Clear(Color.White);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        // Calculate scaling to fit the graph in the image
        double scaleX = (width - 2 * margin) / (double)(maxX - minX);
        double scaleY = (height - 2 * margin) / (double)(maxY - minY);
        double scale = Math.Min(scaleX, scaleY);

        // Draw vertices
        DrawVertices(graph, graphics, minX, minY, scale, margin, height);

        // Save the image to the specified file
        bitmap.Save(filePath, ImageFormat.Png);
    }

    /// <summary>
    /// Plots a VRPInstance to an image file
    /// </summary>
    /// <param name="instance">VRP instance to plot</param>
    /// <param name="filePath">Where to save the image</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    [SupportedOSPlatform("windows")]
    public static void PlotInstance(CVRPInstance instance, string filePath, int width = 1200, int height = 1200)
    {
        PlotGraph(instance.Graph, filePath, width, height);
    }

    /// <summary>
    /// Plots a CVRP solution with routes in different colors
    /// </summary>
    /// <param name="solution">The CVRP solution to plot</param>
    /// <param name="filePath">Where to save the image</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    [SupportedOSPlatform("windows")]
    public static void PlotSolution(CVRPSolution solution, string filePath, int width = 1200, int height = 1200)
    {
        if (solution == null || solution.Graph == null || solution.Graph.VertexCount == 0)
        {
            throw new ArgumentException("Solution or graph is empty or null");
        }

        Graph graph = solution.Graph;

        // Create directory if it doesn't exist
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Calculate bounds of the graph
        int minX = graph.Vertices.Min(v => v.X);
        int minY = graph.Vertices.Min(v => v.Y);
        int maxX = graph.Vertices.Max(v => v.X);
        int maxY = graph.Vertices.Max(v => v.Y);

        // Add a margin
        int margin = Math.Max(maxX - minX, maxY - minY) / 20;
        minX -= margin;
        minY -= margin;
        maxX += margin;
        maxY += margin;

        // Create a bitmap and graphics object
        using Bitmap bitmap = new(width, height);
        using Graphics graphics = Graphics.FromImage(bitmap);

        // Set up the graphics object
        graphics.Clear(Color.White);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        // Calculate scaling to fit the graph in the image
        double scaleX = (width - 2 * margin) / (double)(maxX - minX);
        double scaleY = (height - 2 * margin) / (double)(maxY - minY);
        double scale = Math.Min(scaleX, scaleY);

        // Define route colors
        Color[] routeColors = {
            Color.Blue, Color.Green, Color.Purple, Color.Orange,
            Color.Brown, Color.Magenta, Color.Teal, Color.Navy,
            Color.Olive, Color.Maroon, Color.DarkGreen, Color.DarkViolet,
            Color.DarkOrange, Color.DarkCyan, Color.DeepPink, Color.Indigo
        };

        // Draw routes first (so they're behind the vertices)
        int depotId = graph.Depot?.Id ?? -1;
        using var pen = new Pen(Color.Black, 2);

        // Draw each route with a different color
        for (int i = 0; i < solution.Routes.Count; i++)
        {
            var route = solution.Routes[i];
            Color routeColor = routeColors[i % routeColors.Length];
            pen.Color = routeColor;

            // Draw route edges, skipping connections directly to/from depot
            var vertices = route.Vertices.ToList();
            for (int j = 1; j < vertices.Count - 1; j++)
            {
                var v1 = vertices[j];
                var v2 = vertices[j + 1];

                // Skip if either vertex is the depot
                if (v1.Id == depotId || v2.Id == depotId)
                {
                    continue;
                }

                // Calculate scaled coordinates with Y-axis flipped
                int x1 = (int)((v1.X - minX) * scale) + margin;
                int y1 = height - ((int)((v1.Y - minY) * scale) + margin);
                int x2 = (int)((v2.X - minX) * scale) + margin;
                int y2 = height - ((int)((v2.Y - minY) * scale) + margin);

                // Draw the edge
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        // Draw vertices
        DrawVertices(graph, graphics, minX, minY, scale, margin, height);

        // Draw a legend for the routes
        using Font legendFont = new("Arial", 10);
        using Brush legendTextBrush = new SolidBrush(Color.Black);
        int legendX = 20;
        int legendY = 20;

        graphics.DrawString("Routes:", legendFont, legendTextBrush, legendX, legendY);
        legendY += 20;

        for (int i = 0; i < solution.Routes.Count; i++)
        {
            Color routeColor = routeColors[i % routeColors.Length];
            using var legendPen = new Pen(routeColor, 2);

            // Draw color line sample
            graphics.DrawLine(legendPen, legendX, legendY + 5, legendX + 30, legendY + 5);

            // Draw route number and info
            string routeText = $"Route {i + 1}: {solution.Routes[i].Vertices.Count - 2} customers, demand: {solution.Routes[i].TotalDemand}";
            graphics.DrawString(routeText, legendFont, legendTextBrush, legendX + 40, legendY);

            legendY += 20;
        }

        // Save the image to the specified file
        bitmap.Save(filePath, ImageFormat.Png);
    }

    /// <summary>
    /// Helper method to draw vertices on a graph
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static void DrawVertices(Graph graph, Graphics graphics, int minX, int minY, double scale, int margin, int height)
    {
        foreach (var vertex in graph.Vertices)
        {
            // Calculate scaled coordinates with Y-axis flipped
            int x = (int)((vertex.X - minX) * scale) + margin;
            int y = height - ((int)((vertex.Y - minY) * scale) + margin); // Flip Y

            // Determine vertex size and color based on whether it's a depot
            int size = vertex.Id == graph.Depot?.Id ? 12 : 8;
            Color color = vertex.Id == graph.Depot?.Id ? Color.Red : Color.Black;

            // Draw the vertex
            using (Brush brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
            }

            // Draw vertex ID and demand if not depot
            using Font font = new("Arial", 8);
            using Brush textBrush = new SolidBrush(Color.Black);
            string label = vertex.Id.ToString();
            if (vertex.Id != graph.Depot?.Id && vertex.Demand > 0)
            {
                label += $" ({vertex.Demand})";
            }

            graphics.DrawString(label, font, textBrush, x + size / 2, y + size / 2);
        }
    }
}