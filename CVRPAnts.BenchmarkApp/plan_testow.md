I want to compare different methods to solve CVRP:

-   CVRPAnts.SolversLibrary.GreedySolver
-   CVRPAnts.SolversLibrary.AntColonySolver
-   CVRPAnts.SolversLibrary.AntColony2OptSolver
-   CVRPAnts.SolversLibrary.AntColonyMaxMinSolver

My hypothesis are:

1. Algorytm mrówkowy z heurystyką 2-opt znajduje trasy o co najmniej 10% mniejszym koszcie w
   porównaniu do klasycznego algorytmu mrówkowego.
2. Algorytm mrówkowy z modyfikacją Max-Min znajduje rozwiązanie o koszcie nie większym niż 5% od
   kosztu rozwiązania znalezionego przez klasyczny algorytm mrówkowy, ale potrzebuje do tego o 10%
   mniejszej liczby pojazdów.
3. Procentowa różnica kosztów wyznaczonych tras między algorytmem zachłannym i klasycznym
   algorytmem mrówkowym rośnie wraz ze wzrostem liczby klientów na niekorzyść algorytmu
   zachłannego.
4. Procentowa różnica kosztów wyznaczonych tras między algorytmem mrówkowym z heurystyką 2-opt i
   klasycznym algorytmem mrówkowym rośnie wraz ze wzrostem rozproszenia klientów (mierzonego
   średnią odległością klientów od magazynu) na niekorzyść klasycznego algorytmu.

TestData used for the tests (from folder TestData):

-   X-n190-k8.vrp (optimal solution: X-n190-k8.sol)
-   X-n420-k130.vrp (optimal solution: X-n420-k130.sol)
-   X-n228-k23.vrp (optimal solution: X-n228-k23.sol)
-   X-n367-k17.vrp (optimal solution: X-n367-k17.sol)

Result of each test should be saved in a csv in folder Results. The csv name should include the name of the test, the date of the test. The csv should contain the following columns:

-   TestName (name of the test)
-   Date (date of the test)
-   Solver (name of the solver)
-   TestData (name of the test data)
-   Cost (cost of the solution - routes length)
-   NumberOfVehicles (number of routes)
-   Time (time of the test in seconds)
-   OptimalCost (cost of the optimal solution)
-   CostDifference (difference between the cost of the solution and the optimal solution in percent)

In the separate file I want to save the length of the routes for each iteration of the algorithm. The file name should include the name of the test, the date of the test and the name of the solver. The file should contain the following columns:

-   Iteration (number of iteration)
-   Cost (cost of the solution - routes length)
-   NumberOfVehicles (number of routes)
-   Time (time of the test in seconds)
