namespace CVRPAnts.BenchmarkApp.Models;

public class IterationResult
{
    public int Iteration { get; set; }
    public double Cost { get; set; }
    public int RoutesCount { get; set; }
    public long Time { get; set; }

    public override string ToString()
    {
        return $"{Iteration},{Cost},{RoutesCount},{Time}";
    }

    public static string GetCsvHeader()
    {
        return "Iteration,Cost,RoutesCount,Time";
    }
}