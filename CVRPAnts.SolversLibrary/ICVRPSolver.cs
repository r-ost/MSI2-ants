namespace CVRPAnts.SolversLibrary;

public interface ICVRPSolver
{
    CVRPSolution Solve(CVRPInstance instance);

    IProgressWriter? ProgressWriter { get; }

    int Seed { get; }
}