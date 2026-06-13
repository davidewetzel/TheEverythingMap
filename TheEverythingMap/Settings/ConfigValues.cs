using BepInEx;
using BepInEx.Configuration;

namespace TheEverythingMap.Settings;

public static class ConfigValues
{
    public const string DEFAULT_TEAMMATE_COLOR = "white";
    public const string DEFAULT_ENEMY_COLOR = "red";
    public const string DEFAULT_ITEM_COLOR = "yellow";
    public const string DEFAULT_DEAD_TEAMMATE_COLOR = "black";

    internal static ConfigEntry<float> Zoom = null!;

    internal static ConfigEntry<float> Opacity = null!;

    internal static ConfigEntry<MinimapPosition> Preset = null!;

    internal static ConfigEntry<int> WidthHeight = null!;

    internal static ConfigEntry<int> Buffer = null!;

    internal static ConfigEntry<KeyCode> SettingsKey = null!;

    internal static ConfigEntry<KeyCode> ZoomInKey = null!;

    internal static ConfigEntry<KeyCode> ZoomOutKey = null!;

    internal static ConfigEntry<string> TeammateColor = null!;

    internal static ConfigEntry<bool> ShowEnemies = null!;

    internal static ConfigEntry<bool> ShowItems = null!;

    internal static ConfigEntry<string> EnemyColor = null!;

    internal static ConfigEntry<string> ItemColor = null!;

    internal static ConfigEntry<string> DeadTeammateColor = null!;

    internal static ConfigEntry<bool> ExploreMap = null!;

    internal static void Configure(BaseUnityPlugin plugin)
    {
        TeammateColor = plugin.Config.Bind("General", "Teammate Color", DEFAULT_TEAMMATE_COLOR, new ConfigDescription("The color of teammates on the minimap. (Recommended value = white)", null, []));
        DeadTeammateColor = plugin.Config.Bind("General", "Dead Teammate Color", DEFAULT_DEAD_TEAMMATE_COLOR, new ConfigDescription("The color of dead teammates on the minimap. (Recommended value = black)", null, []));
        EnemyColor = plugin.Config.Bind("General", "Enemy Color", DEFAULT_ENEMY_COLOR, new ConfigDescription("The color of enemies on the minimap. (Recommended value = red)", null, []));
        ItemColor = plugin.Config.Bind("General", "Item Color", DEFAULT_ITEM_COLOR, new ConfigDescription("The color of items on the minimap. (Recommended value = yellow)", null, []));
        ShowEnemies = plugin.Config.Bind("General", "Show Enemies", false, new ConfigDescription("Whether to show enemies on the minimap. (Recommended value = false)", null, []));
        ShowItems = plugin.Config.Bind("General", "Show Items", false, new ConfigDescription("Whether to show items on the minimap. (Recommended value = false)", null, []));
        ExploreMap = plugin.Config.Bind("General", "Explore Map", false, new ConfigDescription("Whether to explore the map automatically. (Recommended value = false)", null, []));
        Zoom = plugin.Config.Bind("General", "Zoom", 2.25f, new ConfigDescription("The minimap zoom level. (Recommended value = 2.25)", null, []));
        Opacity = plugin.Config.Bind("General", "Opacity", 0.85f, new ConfigDescription("The minimap opacity. (Recommended value = 0.85)", null, []));
        Preset = plugin.Config.Bind("Position", "Preset", MinimapPosition.BottomLeft, new ConfigDescription("The minimap position preset. (Recommended value = BottomLeft)", null, []));
        WidthHeight = plugin.Config.Bind("Dimensions", "WidthHeight", 500, new ConfigDescription("The width of the minimap. (Recommended value = 375)", null, []));
        Buffer = plugin.Config.Bind("Dimensions", "Buffer", 12, new ConfigDescription("The buffer between edge of screen added to presets. (Recommended value = 12)", null, []));
        SettingsKey = plugin.Config.Bind("Key Bindings", "Settings", KeyCode.M, new ConfigDescription("The key to open the minimap settings. (Recommended value = M)", null, []));
        ZoomInKey = plugin.Config.Bind("Key Bindings", "Zoom In", KeyCode.Equals, new ConfigDescription("The key to zoom in on the minimap. (Recommended value = Equals)", null, []));
        ZoomOutKey = plugin.Config.Bind("Key Bindings", "Zoom Out", KeyCode.Minus, new ConfigDescription("The key to zoom out on the minimap. (Recommended value = Minus)", null, []));
    }

    internal static void ResetDefaultValues()
    {
        Zoom.Value = (float)Zoom.DefaultValue;
        Opacity.Value = (float)Opacity.DefaultValue;
        Preset.Value = (MinimapPosition)Preset.DefaultValue;
        Buffer.Value = (int)Buffer.DefaultValue;
        WidthHeight.Value = (int)WidthHeight.DefaultValue;
        SettingsKey.Value = (KeyCode)SettingsKey.DefaultValue;
        ZoomInKey.Value = (KeyCode)ZoomInKey.DefaultValue;
        ZoomOutKey.Value = (KeyCode)ZoomOutKey.DefaultValue;
        TeammateColor.Value = (string)TeammateColor.DefaultValue;
        EnemyColor.Value = (string)EnemyColor.DefaultValue;
        ShowEnemies.Value = (bool)ShowEnemies.DefaultValue;
        ExploreMap.Value = (bool)ExploreMap.DefaultValue;
        ItemColor.Value = (string)ItemColor.DefaultValue;
        DeadTeammateColor.Value = (string)DeadTeammateColor.DefaultValue;

    }
}