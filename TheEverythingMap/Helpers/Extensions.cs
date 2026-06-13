using System.Collections.Generic;

namespace TheEverythingMap.Helpers;

internal static class Extensions
{
    //idk why i'm putting it here but i'm too lazy to move it somewhere more appropriate
    public static Dictionary<string, Color> VALID_COLORS = new()
    {
        { "white", Color.white },
        { "red", Color.red },
        { "green", Color.green },
        { "blue", Color.blue },
        { "yellow", Color.yellow },
        { "cyan", Color.cyan },
        { "magenta", Color.magenta },
        { "black", Color.black },
        { "lilac", new Color((175f/255f), (143f/255f), (233f/255f)) },
        { "purple", new Color((164f/255f), (112f/255f), (227f/255f)) }
    };


public static Color ToColor(this string color)
    {
        Color defaultColor = Color.white;
        VALID_COLORS.TryGetValue(color.ToLower(), out defaultColor);
        return defaultColor;
    }

    public static float ToScale(this EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.VeryLight => 0.55f,
            EnemyType.Light => 0.7f,
            EnemyType.Medium => 0.85f,
            EnemyType.Heavy => 1f,
            EnemyType.VeryHeavy => 1.15f,
            _ => 1
        };
    }

}
