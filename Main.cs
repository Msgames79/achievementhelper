using System;
using BepInEx;
using UnityEngine;

namespace AchievementHelper;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Main : BaseUnityPlugin
{
    private void Start()
    {
        Shell.RegisterCommand("steamunlock", new Action(this.SteamUnlock), null);
        Shell.RegisterCommand("su", new Action(this.SteamUnlock), null);
    }

    public unsafe void Update()
    {
    }

    public void SteamUnlock()
    {
        foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
        {
            StatsAndAchievements.UnlockAchievement(achievement, false, -1);
        }
    }
}
