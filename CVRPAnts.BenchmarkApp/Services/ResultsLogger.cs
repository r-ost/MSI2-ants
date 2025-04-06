using CVRPAnts.BenchmarkApp.Models;

namespace CVRPAnts.BenchmarkApp.Services;

public static class ResultsLogger
{
    public static void SaveResultsWithTimestamp(List<BenchmarkResult> results, string dirPath, string testName)
    {
        if (results.Count == 0)
        {
            return;
        }

        Directory.CreateDirectory(dirPath);

        var date = DateTimeOffset.UtcNow;
        var fileName = $"{testName}_{date:yyyyMMddHHmmss}.csv";
        var filePath = Path.Combine(dirPath, fileName);
        bool fileExists = File.Exists(filePath);

        using var writer = new StreamWriter(filePath, true);

        // Only write header if file doesn't already exist
        if (!fileExists)
        {
            writer.WriteLine(BenchmarkResult.GetCsvHeader());
        }

        foreach (var result in results)
        {
            writer.WriteLine(result.ToString());
        }

        Console.WriteLine($"Results saved to: {filePath}");
    }

    public static void SaveIterationResults(List<IterationResult> results, string dirPath, string testId)
    {
        if (results.Count == 0)
        {
            return;
        }

        Directory.CreateDirectory(dirPath);

        var date = DateTimeOffset.UtcNow;
        var fileName = $"{testId}_{date:yyyyMMddHHmmss}_iterations.csv";
        var filePath = Path.Combine(dirPath, fileName);
        bool fileExists = File.Exists(filePath);

        using var writer = new StreamWriter(filePath, true);

        // Only write header if file doesn't already exist
        if (!fileExists)
        {
            writer.WriteLine(IterationResult.GetCsvHeader());
        }

        foreach (var result in results)
        {
            writer.WriteLine(result.ToString());
        }

        Console.WriteLine($"Iteration results saved to: {filePath}");
    }
}