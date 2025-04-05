namespace CVRPAnts.SolversLibrary;

public interface ICVRPSolver
{
    CVRPSolution Solve(CVRPInstance parameters);
}