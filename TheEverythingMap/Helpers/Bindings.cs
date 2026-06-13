using TheEverythingMap.Settings;

namespace TheEverythingMap.Helpers;

internal class Bindings
{
    internal static void Update()
    {
        if (StateUtils.IsInLevel() && !StateUtils.IsChatActive())
        {
            if (Input.GetKeyDown(ConfigValues.ZoomInKey.Value))
            {
                ConfigValues.Zoom.Value = Mathf.Max(ConfigValues.Zoom.Value - 0.5f, 1.5f);
            }
            if (Input.GetKeyDown(ConfigValues.ZoomOutKey.Value))
            {
                ConfigValues.Zoom.Value = Mathf.Min(ConfigValues.Zoom.Value + 0.5f, 10f);
            }
        }
    }
}

