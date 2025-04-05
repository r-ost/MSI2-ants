namespace CVRPAnts.SolversLibrary;

using CVRPAnts.GraphLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

public class AntColonySolver : AntColonyBaseSolver
{
    public AntColonySolver(int? seed = null) : base(seed)
    {
    }

    // Standard ACO implementation doesn't override any base behaviors
}