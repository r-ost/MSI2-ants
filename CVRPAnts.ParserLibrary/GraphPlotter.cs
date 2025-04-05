using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using CVRPAnts.GraphLibrary;

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
    public static void PlotGraph(Graph graph, string filePath, int width = 1200, int height = 1200, bool drawEdges = false)
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

        if (drawEdges)
        {
            // Draw edges
            using (Pen edgePen = new(Color.LightGray, 1))
            {
                foreach (var edge in graph.Edges)
                {
                    // Calculate scaled coordinates
                    int x1 = (int)((edge.Vertex1.X - minX) * scale) + margin;
                    int y1 = (int)((edge.Vertex1.Y - minY) * scale) + margin;
                    int x2 = (int)((edge.Vertex2.X - minX) * scale) + margin;
                    int y2 = (int)((edge.Vertex2.Y - minY) * scale) + margin;

                    // Draw the edge
                    graphics.DrawLine(edgePen, x1, y1, x2, y2);
                }
            }
        }

        // Draw vertices
        foreach (var vertex in graph.Vertices)
        {
            // Calculate scaled coordinates
            int x = (int)((vertex.X - minX) * scale) + margin;
            int y = (int)((vertex.Y - minY) * scale) + margin;

            // Determine vertex size and color based on whether it's a depot
            int size = vertex.Id == graph.Depot?.Id ? 12 : 8;
            Color color = vertex.Id == graph.Depot?.Id ? Color.Red : Color.Blue;

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
}