using CVRPAnts.BenchmarkApp.Models;
using CVRPAnts.SolversLibrary;

namespace CVRPAnts.BenchmarkApp;

public class ProgressWriter : IProgressWriter
{
    private readonly List<IterationResult> iterationResults = [];

    public List<IterationResult> IterationResults => iterationResults;
    public string ResultsDirectory { get; }

    public ProgressWriter(string resultsDirectory)
    {
        this.ResultsDirectory = resultsDirectory;
    }

    public void WriteProgress(int iteration, double elapsedTime, double bestSolutionLength, int routesCount)
    {
        var result = new IterationResult
        {
            Iteration = iteration,
            Cost = bestSolutionLength,
            RoutesCount = routesCount,
            Time = (long)elapsedTime
        };
        iterationResults.Add(result);
    }

    public void SaveProgress(string testId)
    {
        ResultsLogger.SaveIterationResults(iterationResults, ResultsDirectory, testId);
    }
}