namespace Kinksweeper.Models;

public static class Constants
{
    public const int DefaultFieldDimension = 9;
    public const double DefaultMineDensity = 0.2;
    public const int DefaultPicsPerPunishment = 10;

    public static readonly FieldConfiguration EasyConfiguration = new()
    {
        Dimension = 6,
        MinesCount = 5,
        PicsPerPunishment = DefaultPicsPerPunishment,
    };
    
    public static readonly FieldConfiguration MediumConfiguration = new()
    {
        Dimension = 9,
        MinesCount = 28,
        PicsPerPunishment = DefaultPicsPerPunishment,
    };
    
    public static readonly FieldConfiguration HardConfiguration = new()
    {
        Dimension = 12,
        MinesCount = 60,
        PicsPerPunishment = DefaultPicsPerPunishment,
    };
}