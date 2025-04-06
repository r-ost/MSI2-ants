using CVRPAnts.SolversLibrary;

namespace CVRPAnts.BenchmarkApp.Models;

public class BenchmarkResult
{
    public string TestName { get; set; } = string.Empty;
    public int Seed { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Solver { get; set; } = string.Empty;
    public string TestData { get; set; } = string.Empty;
    public double Cost { get; set; }
    public int RoutesCount { get; set; }
    public long Time { get; set; }
    public double OptimalCost { get; set; }
    public int OptimalRoutesCount { get; set; }
    public double CostDifference { get; set; }

    public string TestId => $"{TestName}_{Solver}_{TestData}";

    public override string ToString()
    {
        return $"{TestId},{Date.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ},{Time},{Cost},{OptimalCost},{(int)CostDifference},{RoutesCount},{OptimalRoutesCount}";
    }

    public static string GetCsvHeader()
    {
        return "TestId,Date,Time,Cost,OptimalCost,CostDifference,RoutesCount,OptimalRoutesCount";
    }
}