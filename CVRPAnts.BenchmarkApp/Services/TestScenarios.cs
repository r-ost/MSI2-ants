using CVRPAnts.SolversLibrary;

namespace CVRPAnts.BenchmarkApp.Services;

public static class TestScenarios
{
    private static readonly AntColonyParameters standardAcoParams = new AntColonyParameters
    {
        AntCount = 200,
        MaxIterations = 10,
        Alpha = 2.0,
        Beta = 5.0,
        EvaporationRate = 0.2,
        Q = 100,
        InitialPheromone = 0.1
    };

    private static readonly int[] seeds = [2020, 1337, 42];

    public static BenchmarkRunner CreateHypothesis1Test(
        string testDataDir,
        string resultsDir)
    {
        var runner = new BenchmarkRunner(testDataDir)
        {
            TestName = "Hypothesis1_AntColony2OptVsClassic"
        };

        foreach (var seed in seeds)
        {
            var standardAco = new AntColonySolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony_{seed}", standardAco);

            var aco2opt = new AntColony2OptSolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony2Opt_{seed}", aco2opt);
        }

        runner.AddDataset("X-n101-k25", 27591, 26);
        // runner.AddDataset("X-n228-k23", 25742);

        return runner;
    }

    public static BenchmarkRunner CreateHypothesis2Test(
        string testDataDir,
        string resultsDir)
    {
        var runner = new BenchmarkRunner(testDataDir)
        {
            TestName = "Hypothesis2_MaxMinVsClassic"
        };

        foreach (var seed in seeds)
        {
            var standardAco = new AntColonySolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony_{seed}", standardAco);

            var maxMinAco = new AntColonyMaxMinSolver(standardAcoParams, seed, new ProgressWriter(resultsDir))
            {
                PheromoneMin = 0.01,
                PheromoneMax = 10.0,
                OnlyBestUpdates = true,
                StagnationLimit = 10
            };
            runner.AddSolver($"MaxMinAntColony_{seed}", maxMinAco);
        }
        runner.AddDataset("X-n101-k25", 27591, 26);
        // runner.AddDataset("X-n420-k130", 107798);
        // runner.AddDataset("X-n367-k17", 22814);

        return runner;
    }

    public static BenchmarkRunner CreateHypothesis3Test(
        string testDataDir,
        string resultsDir)
    {
        var runner = new BenchmarkRunner(testDataDir)
        {
            TestName = "Hypothesis3_GreedyVsAntColony"
        };

        // Add solvers
        runner.AddSolver("Greedy", new GreedySolver());

        foreach (var seed in seeds)
        {
            var standardAco = new AntColonySolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony_{seed}", standardAco);
        }

        // Add datasets with varying client counts (sorted by increasing vertex count)
        runner.AddDataset("X-n101-k25", 27591, 26);

        // runner.AddDataset("X-n190-k8", 16980);
        // runner.AddDataset("X-n228-k23", 25742);
        // runner.AddDataset("X-n367-k17", 22814);
        // runner.AddDataset("X-n420-k130", 107798);

        return runner;
    }

    public static BenchmarkRunner CreateHypothesis4Test(
        string testDataDir,
        string resultsDir)
    {
        var runner = new BenchmarkRunner(testDataDir)
        {
            TestName = "Hypothesis4_2OptVsClassicWithDispersion"
        };

        // Add solvers
        foreach (var seed in seeds)
        {
            var standardAco = new AntColonySolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony_{seed}", standardAco);

            var aco2opt = new AntColony2OptSolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony2Opt_{seed}", aco2opt);
        }


        // Datasets will be analyzed by client dispersion in the main program
        runner.AddDataset("X-n101-k25", 27591, 26);

        // runner.AddDataset("X-n190-k8", 16980);
        // runner.AddDataset("X-n228-k23", 25742);
        // runner.AddDataset("X-n367-k17", 22814);
        // runner.AddDataset("X-n420-k130", 107798);

        return runner;
    }
}