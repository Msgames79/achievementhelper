using System;
using System.ComponentModel;
using System.Text;
using BepInEx;
using HumanAPI;
using UnityEngine;

namespace AchievementHelper;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Main : BaseUnityPlugin
{
    private static bool guiOpened;
    private bool showStatus;
    private Rect mainWindowRect;
    public static Main instance;
    private static GUILayoutOption[] guiLayoutOptions;
    private static GUILayoutOption[] guiLayoutOptions2;
    private GUIStyle gUIStyle;
    private void Start()
    {
        Shell.RegisterCommand("steamunlock", new Action<string>(SteamUnlock), null);
        Shell.RegisterCommand("sr", new Action(SteamResetAlias), null);
        Shell.RegisterCommand("su", new Action<string>(SteamUnlock), null);
        Shell.RegisterCommand("ss", new Action(ShowStatus), null);
    }

    public static void SteamUnlock(string option)
    {
        switch (option)
        {
            case "all":
                {
                    foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
                    {
                        StatsAndAchievements.UnlockAchievement(achievement, false, -1);
                    }
                    break;
                }
            case "nsb":
                {
                    foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
                    {
                        if (achievement <= Achievement.ACH_SINGLE_RUN || achievement >= Achievement.ACH_INTRO_STATUE_HEAD || achievement == Achievement.ACH_FALL_1)
                        {
                            StatsAndAchievements.UnlockAchievement(achievement, false, -1);
                        }
                    }
                    break;
                }
            case "sb":
                {
                    foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
                    {
                        if (achievement >= Achievement.ACH_TRAVEL_1KM && achievement <= Achievement.ACH_DUMPSTER_50M && achievement != Achievement.ACH_FALL_1)
                        {
                            StatsAndAchievements.UnlockAchievement(achievement, false, -1);
                        }
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    public static void SteamResetAlias()
    {
        Shell.RawInvoke("steamreset");
    }
    public static void ShowStatus()
    {
        Main.instance.showStatus = !Main.instance.showStatus;
    }
    public unsafe void Awake()
    {
        this.mainWindowRect = new Rect(330, 750, 160, 200);
        Main.guiLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(false)
        };
        Main.guiLayoutOptions2 = new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true)
        };
        this.gUIStyle = new GUIStyle() {fontSize = 30};
        Main.guiOpened = false;
        Main.instance = this;
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
        if (Game.instance != null)
        {
            if (Main.guiOpened)
            {
                mainWindowRect = GUILayout.Window(1399186700, mainWindowRect, WindowFunction, "Achievement Helper");
            }
        }
        if (showStatus)
        {
            StringBuilder status = new StringBuilder();
            status.Append("TravelM ");
            status.AppendLine(StatsAndAchievements.travelM.ToString());
            status.Append("Fall ");
            status.AppendLine(StatsAndAchievements.fall.ToString());
            status.Append("Jump ");
            status.AppendLine(StatsAndAchievements.jump.ToString());
            status.Append("ClimbM ");
            status.AppendLine(StatsAndAchievements.climbM.ToString());
            status.Append("CarryM ");
            status.AppendLine(StatsAndAchievements.carryM.ToString());
            status.Append("Drown ");
            status.AppendLine(StatsAndAchievements.drown.ToString());
            status.Append("ShipM ");
            status.AppendLine(StatsAndAchievements.shipM.ToString());
            status.Append("DriveM ");
            status.AppendLine(StatsAndAchievements.driveM.ToString());
            status.Append("DumpstarM ");
            status.AppendLine(StatsAndAchievements.dumpsterM.ToString());
            GUILayout.Label(status.ToString());                                                         
        }
    }

    void WindowFunction(int windowID)
    {
        if (GUILayout.Button("Unlock all achievements"))
        {
            SteamUnlock("all");
        }
        if (GUILayout.Button("Unlock NSB achievements"))
        {
            SteamUnlock("nsb");
        }
        if (GUILayout.Button("Unlock SB achievements"))
        {
            SteamUnlock("sb");
        }
        GUILayout.BeginHorizontal();
        GUILayout.Button("AH, EO, EO, EO, EO, OOOOO!");
        GUILayout.Label("AH, EO, EO, EO, EO, OOOOO!", guiLayoutOptions2);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Reset achievements"))
        {
            SteamResetAlias();
        }
        showStatus = GUILayout.Toggle(showStatus, "Show status");
        GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
    }
}