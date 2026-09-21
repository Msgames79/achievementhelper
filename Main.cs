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
    private bool showStats;
    private Rect mainWindowRect;
    public static Main instance;
    private static GUILayoutOption[] guiLayoutOptions;
    private GUIStyle gUIStyle;
    private GUIStyle dontWrap;
    private GUIStyle idStyle;
    private bool isFirst = true;
    private string achievementId;
    private int idNum;
    public static int achievementCount;
    private string achievementName;
    private void Start()
    {
        Shell.RegisterCommand("sr", new Action(SteamResetAlias), null);
        Shell.RegisterCommand("steamunlock", new Action<string>(SteamUnlock), null);
        Shell.RegisterCommand("su", new Action<string>(SteamUnlock), null);
        Shell.RegisterCommand("showstats", new Action(ShowStats), null);
        Shell.RegisterCommand("ss", new Action(ShowStats), null);
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
                    return;
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
                    return;
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
                    return;
                }
            default:
                {
                    if (Int32.TryParse(option, out instance.idNum) && instance.idNum > 0 && instance.idNum <= achievementCount)
                    {
                        StatsAndAchievements.UnlockAchievement((Achievement)instance.idNum - 1, false, -1);
                    }
                    else if (String.IsNullOrWhiteSpace(option))
                    {
                        foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
                        {
                            StatsAndAchievements.UnlockAchievement(achievement, false, -1);
                        }
                    }
                    return;
                }
        }
    }
    public static void SteamResetAlias()
    {
        Shell.RawInvoke("steamreset");
    }
    public static void ShowStats()
    {
        Main.instance.showStats = !Main.instance.showStats;
    }
    public void Awake()
    {
        mainWindowRect = new Rect(330, 750, 381, 200);
        Main.guiLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(false),
            GUILayout.MaxWidth(Screen.width)
        };
        Main.guiOpened = false;
        Main.instance = this;
        Main.achievementCount = Enum.GetValues(typeof(Achievement)).Length;
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

    public void OnGUI()
    {
        if (Game.instance != null)
        {
            if (Main.guiOpened)
            {
                mainWindowRect = GUILayout.Window(1399186700, mainWindowRect, WindowFunction, "Achievement Helper", guiLayoutOptions);
            }
            if (isFirst)
            {
                gUIStyle = new GUIStyle()
                {
                    fontSize = 30
                };
                dontWrap = new GUIStyle()
                {
                    wordWrap = false,
                    margin = new RectOffset(0, 0, 8, 0),
                    alignment = TextAnchor.MiddleCenter,
                    normal =
                    {
                        textColor = Color.white
                    }
                };
                idStyle = new GUIStyle(GUI.skin.textField)
                {
                    wordWrap = false,
                    alignment = TextAnchor.LowerLeft,
                    fixedWidth = 40
                    
                };
                isFirst = false;
            }
            if (showStats)
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
                GUILayout.Label(status.ToString(), gUIStyle);                                                         
            }
            if (!String.IsNullOrEmpty(achievementId))
            {
                if (achievementId.Length > Math.Ceiling(Math.Log10((double)achievementCount)))
                {
                    achievementId = achievementId.Substring(0, (int)Math.Ceiling(Math.Log10((double)achievementCount)));
                }
                if (Int32.TryParse(achievementId, out idNum) && idNum > 0 && idNum <= achievementCount)
                {
                    achievementName = Enum.GetName(typeof(Achievement), idNum - 1);
                }
                else
                {
                    achievementName = "Invalid";
                }
            }
            else
            {
                achievementName = "Invalid";
            }
        }
    }

    void WindowFunction(int windowID)
    {
        if (GUILayout.Button("Unlock all achievements", guiLayoutOptions))
        {
            SteamUnlock("all");
        }
        if (GUILayout.Button("Unlock NSB achievements", guiLayoutOptions))
        {
            SteamUnlock("nsb");
        }
        if (GUILayout.Button("Unlock SB achievements", guiLayoutOptions))
        {
            SteamUnlock("sb");
        }
        GUILayout.BeginHorizontal();
        achievementId = GUILayout.TextField(achievementId, idStyle, guiLayoutOptions);
        GUILayout.Label(achievementName, dontWrap, guiLayoutOptions);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Unlock this achievement", guiLayoutOptions) && idNum > 0 && idNum <= achievementCount)
        {
            StatsAndAchievements.UnlockAchievement((Achievement)idNum - 1, false, -1);
        }
        if (GUILayout.Button("Reset achievements", guiLayoutOptions))
        {
            SteamResetAlias();
        }
        showStats = GUILayout.Toggle(showStats, "Show stats", guiLayoutOptions);
        GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
    }
}