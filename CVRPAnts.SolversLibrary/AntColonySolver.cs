namespace CVRPAnts.SolversLibrary;
public class AntColonySolver(AntColonyParameters parameters, int seed, IProgressWriter? progressWriter)
    : AntColonyBaseSolver(parameters, seed, progressWriter)
{
}