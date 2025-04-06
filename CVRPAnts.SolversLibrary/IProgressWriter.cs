namespace CVRPAnts.SolversLibrary;

public interface IProgressWriter
{
    void WriteProgress(int iteration, double elapsedTime, double bestSolutionLength);
}