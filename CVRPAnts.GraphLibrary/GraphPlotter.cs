using System.IO;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace CVRPAnts.GraphLibrary;

public class GraphPlotter
{
    public static void PlotGraph(Graph graph, string filePath)
    {
        var plot = new PlotModel { Title = "Customers" };
        foreach (var vertex in graph.Vertices)
        {
            plot.Series.Add(new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                ItemsSource = new List<ScatterPoint> { new ScatterPoint(vertex.X, vertex.Y) }
            });
        }

        // add legend
        plot.Legends.Add(new Legend
        {
            LegendPlacement = LegendPlacement.Outside,
            LegendPosition = LegendPosition.RightTop,
            LegendOrientation = LegendOrientation.Vertical
        });

        // write to file
        using var stream = new FileStream(filePath, FileMode.Create);
        var pngExporter = new PngExporter { Width = 800, Height = 600 };

        plot.Background = OxyColors.White;
        pngExporter.Export(plot, stream);
    }
}