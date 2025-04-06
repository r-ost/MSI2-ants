using CVRPAnts.BenchmarkApp.Models;
using CVRPAnts.BenchmarkApp.Services;
using CVRPAnts.ParserLibrary;
using CVRPAnts.SolversLibrary;

namespace CVRPAnts.BenchmarkApp;

internal class Program
{
    private const string TEST_DATA_DIR = @"D:\Projects\MSI2-ants\TestData";
    private const string RESULTS_DIR = @"D:\Projects\MSI2-ants\Results";

    static void Main(string[] args)
    {
        Console.WriteLine("CVRP Algorithms Benchmarks");
        Console.WriteLine("=========================");

        Directory.CreateDirectory(Path.GetFullPath(TEST_DATA_DIR));
        Directory.CreateDirectory(Path.GetFullPath(RESULTS_DIR));

        Console.WriteLine("\nAvailable tests:");
        Console.WriteLine("1. Hypothesis 1: Ant Colony 2-opt vs Classic Ant Colony");
        Console.WriteLine("2. Hypothesis 2: Max-Min Ant Colony vs Classic Ant Colony");
        Console.WriteLine("3. Hypothesis 3: Greedy vs Ant Colony with increasing client count");
        Console.WriteLine("4. Hypothesis 4: Ant Colony 2-opt vs Classic Ant Colony with client dispersion");
        Console.WriteLine("5. Run all tests");
        Console.WriteLine("0. Exit");

        while (true)
        {
            Console.Write("\nSelect a test to run (0-5): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 5)
            {
                Console.WriteLine("Invalid choice. Please enter a number between 0 and 5.");
                continue;
            }

            if (choice == 0)
            {
                break;
            }

            switch (choice)
            {
                case 1:
                    RunHypothesis1Test();
                    break;
                case 2:
                    RunHypothesis2Test();
                    break;
                case 3:
                    RunHypothesis3Test();
                    break;
                case 4:
                    RunHypothesis4Test();
                    break;
                case 5:
                    RunAllTests();
                    break;
            }
        }

        Console.WriteLine("Exiting the program");
    }

    static void RunHypothesis1Test()
    {
        Console.WriteLine("\n=== Running Hypothesis 1 Test ===");
        Console.WriteLine("Algorytm mrówkowy z heurystyką 2-opt znajduje trasy o co najmniej 10% mniejszym koszcie w porównaniu do klasycznego algorytmu mrówkowego.");

        var runner = TestScenarios.CreateHypothesis1Test(
            Path.GetFullPath(TEST_DATA_DIR),
            Path.GetFullPath(RESULTS_DIR));

        var results = runner.RunBenchmarks();

        ResultsLogger.SaveResultsWithTimestamp(results, Path.GetFullPath(RESULTS_DIR), runner.TestName);
    }

    static void RunHypothesis2Test()
    {
        Console.WriteLine("\n=== Running Hypothesis 2 Test ===");
        Console.WriteLine("Algorytm mrówkowy z modyfikacją Max-Min znajduje rozwiązanie o koszcie nie większym niż 5% od kosztu rozwiązania znalezionego przez klasyczny algorytm mrówkowy, ale potrzebuje do tego o 10% mniejszej liczby pojazdów.");

        var runner = TestScenarios.CreateHypothesis2Test(
            Path.GetFullPath(TEST_DATA_DIR),
            Path.GetFullPath(RESULTS_DIR));

        var results = runner.RunBenchmarks();

        ResultsLogger.SaveResultsWithTimestamp(results, Path.GetFullPath(RESULTS_DIR), runner.TestName);
    }

    static void RunHypothesis3Test()
    {
        Console.WriteLine("\n=== Running Hypothesis 3 Test ===");
        Console.WriteLine(" Procentowa różnica kosztów wyznaczonych tras między algorytmem zachłannym i klasycznym algorytmem mrówkowym rośnie wraz ze wzrostem liczby klientów na niekorzyść algorytmu zachłannego.");

        var runner = TestScenarios.CreateHypothesis3Test(
            Path.GetFullPath(TEST_DATA_DIR),
            Path.GetFullPath(RESULTS_DIR));

        var results = runner.RunBenchmarks();

        ResultsLogger.SaveResultsWithTimestamp(results, Path.GetFullPath(RESULTS_DIR), runner.TestName);
    }

    static void RunHypothesis4Test()
    {
        Console.WriteLine("\n=== Running Hypothesis 4 Test ===");
        Console.WriteLine("Procentowa różnica kosztów wyznaczonych tras między algorytmem mrówkowym z heurystyką 2-opt i klasycznym algorytmem mrówkowym rośnie wraz ze wzrostem rozproszenia klientów (mierzonego średnią odległością klientów od magazynu) na niekorzyść klasycznego algorytmu.");

        var runner = TestScenarios.CreateHypothesis4Test(
            Path.GetFullPath(TEST_DATA_DIR),
            Path.GetFullPath(RESULTS_DIR));

        var results = runner.RunBenchmarks();

        ResultsLogger.SaveResultsWithTimestamp(results, Path.GetFullPath(RESULTS_DIR), runner.TestName);
    }

    static void RunAllTests()
    {
        Console.WriteLine("\n=== Running All Tests ===");

        RunHypothesis1Test();
        RunHypothesis2Test();
        RunHypothesis3Test();
        RunHypothesis4Test();

        Console.WriteLine("\nAll tests completed!");
    }
}
