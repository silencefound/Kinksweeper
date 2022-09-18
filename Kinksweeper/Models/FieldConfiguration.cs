using System;

namespace Kinksweeper.Models;

public class FieldConfiguration
{
    public int Dimension;
    public int MinesCount;
    public int PicsPerPunishment;

    public FieldConfiguration()
    {
        Dimension = Constants.DefaultFieldDimension;
        MinesCount = (int)(Constants.DefaultMineDensity * Math.Pow(Constants.DefaultFieldDimension, 2));
        PicsPerPunishment = Constants.DefaultPicsPerPunishment;
    }
}
