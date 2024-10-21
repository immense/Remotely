namespace Remotely.Shared.Utilities;

public static class MathHelper
{
    public static string GetFormattedPercent(double current, double max)
    {
        if (current < 1 || max < 1)
        {
            return "0%";
        }

        return $"{Math.Round(current / max * 100)} %";
    }

    public static string GetFormattedPercent(double percentage)
    {
        return $"{Math.Round(percentage * 100)} %";
    }
}
