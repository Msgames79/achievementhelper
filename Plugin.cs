using BepInEx;
using BepInEx.Logging;

namespace MyFirstPlugin;

[BepInPlugin("com.Msgames79.AchievementHelper", "AchievementHelper", "0.0.0")]
[BepInProcess("Human.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo("Plugin com.Msgames79.AchievementHelper is loaded!");
    }
}
