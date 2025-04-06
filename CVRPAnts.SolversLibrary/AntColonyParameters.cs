namespace CVRPAnts.SolversLibrary;

public class AntColonyParameters
{
    public int AntCount { get; set; } = 200;

    public int MaxIterations { get; set; } = 200;

    public double Alpha { get; set; } = 1.0;

    public double Beta { get; set; } = 5.0;

    public double EvaporationRate { get; set; } = 0.1;

    public double Q { get; set; } = 100.0;

    public double InitialPheromone { get; set; } = 0.1;

    public AntColonyParameters Clone()
    {
        return new AntColonyParameters
        {
            AntCount = this.AntCount,
            MaxIterations = this.MaxIterations,
            Alpha = this.Alpha,
            Beta = this.Beta,
            EvaporationRate = this.EvaporationRate,
            Q = this.Q,
            InitialPheromone = this.InitialPheromone
        };
    }
}