using System;
using BepInEx;
using UnityEngine;

namespace AchievementHelper;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Main : BaseUnityPlugin
{
    private static bool guiOpened;
    private Rect mainWindowRect;
    private void Start()
    {
        Shell.RegisterCommand("steamunlock", new Action(this.SteamUnlock), null);
        Shell.RegisterCommand("su", new Action(this.SteamUnlock), null);
    }

    public unsafe void Awake()
    {
        this.mainWindowRect = new Rect(330, 750, 400, 200);
        Main.guiOpened = false;
    }

    public unsafe void Update()
    {
        if (Game.instance != null)
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Main.guiOpened = !Main.guiOpened;
            }
        }
    }

    public unsafe void OnGUI()
    {
        bool flag = Game.instance != null;
        if (flag)
        {
            if (Main.guiOpened)
            {
                mainWindowRect = GUI.Window(1399186700, mainWindowRect, WindowFunction, "Achievement Helper");
            }
        }
    }

    void WindowFunction(int windowID)
    {
        if (GUI.Button(new Rect(20, 30, 100, 20), "context here"))
        {
            Debug.Log("pressed");
        }
        GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
    }

    public void SteamUnlock()
    {
        foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
        {
            StatsAndAchievements.UnlockAchievement(achievement, false, -1);
        }
    }
}
