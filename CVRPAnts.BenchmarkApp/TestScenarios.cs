using CVRPAnts.SolversLibrary;

namespace CVRPAnts.BenchmarkApp;

public static class TestScenarios
{
    private static readonly AntColonyParameters standardAcoParams = new AntColonyParameters
    {
        // AntCount is set dynamically to number of vertices
        MaxIterations = 150,
        Alpha = 1.0,
        Beta = 5.0,
        EvaporationRate = 0.2,
        Q = 10,
        InitialPheromone = 0.1,
    };

    private static readonly int[] seeds = [2020, 1337, 42];

    private static readonly IEnumerable<(string, int, int)> datasets = new[]
    {
        ("X-n101-k25", 27591, 26), // test name, optimal cost, optimal routes
        ("X-n172-k51", 45607, 53),
        ("X-n214-k11", 10856, 11),
    };

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

        foreach (var dataset in datasets)
        {
            runner.AddDataset(dataset.Item1, dataset.Item2, dataset.Item3);
        }

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

            var maxMinAcoParameters = standardAcoParams.Clone();

            var pheromoneMax = 10.0;
            maxMinAcoParameters.InitialPheromone = pheromoneMax;
            maxMinAcoParameters.EvaporationRate = 0.1;
            var maxMinAco = new AntColonyMaxMinSolver(maxMinAcoParameters, seed, new ProgressWriter(resultsDir))
            {
                PheromoneMin = 0.01,
                PheromoneMax = pheromoneMax,
                OnlyBestUpdates = true,
                StagnationLimit = 30
            };
            runner.AddSolver($"MaxMinAntColony_{seed}", maxMinAco);
        }

        foreach (var dataset in datasets)
        {
            runner.AddDataset(dataset.Item1, dataset.Item2, dataset.Item3);
        }

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

        runner.AddSolver("Greedy", new GreedySolver());
        foreach (var seed in seeds)
        {
            var standardAco = new AntColonySolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony_{seed}", standardAco);
        }

        foreach (var dataset in datasets)
        {
            runner.AddDataset(dataset.Item1, dataset.Item2, dataset.Item3);
        }

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

        foreach (var seed in seeds)
        {
            var standardAco = new AntColonySolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony_{seed}", standardAco);

            var aco2opt = new AntColony2OptSolver(standardAcoParams, seed, new ProgressWriter(resultsDir));
            runner.AddSolver($"AntColony2Opt_{seed}", aco2opt);
        }

        foreach (var dataset in datasets)
        {
            runner.AddDataset(dataset.Item1, dataset.Item2, dataset.Item3);
        }

        return runner;
    }
}