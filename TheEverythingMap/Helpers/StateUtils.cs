namespace TheEverythingMap.Helpers;

internal static class StateUtils
{
    internal static bool HasLocalMapToolActive()
    {
        PlayerAvatar instance = PlayerAvatar.instance;
        return (object)instance != null && instance.playerAvatarVisuals.playerAvatarRightArm.mapToolController != null && instance.playerAvatarVisuals.playerAvatarRightArm.mapToolController.Active;
    }

    internal static bool IsInLevel()
    {
        return SemiFunc.RunIsLevel() && (object)GameDirector.instance != null && (int)GameDirector.instance.currentState == 2;
    }

    internal static bool IsChatActive()
    {
        return (object)ChatManager.instance != null && ChatManager.instance.chatActive;
    }
}