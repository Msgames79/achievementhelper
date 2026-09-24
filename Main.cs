using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using HumanAPI;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Net;
using Steamworks;

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
    private GUIStyle gUIStyle2;
    private GUIStyle gUIStyle3;
    private GUIStyle dontWrap;
    private GUIStyle idStyle;
    private GUIStyle statsStyle;
    private bool isFirst = true;
    private string achievementId;
    private int idNum;
    public readonly static int achievementCount = Enum.GetValues(typeof(Achievement)).Length;
    private string achievementName;
    private Color statsColor = new Color(1f, 0f, 0f);
    private string statsSizeString = "30";
    private string statsColorString = "F00";
    public StatMonitorHuman statMonitorHuman = FindObjectOfType<StatMonitorHuman>();
    public static float oldTravelM;
    public static int oldFall;
    public static float oldClimbM;
    public static float oldCarryM;
    public static float oldShipM;
    public static float oldDriveM;
    public static float oldDumpsterM;

    public static float travelDelta;
    public static int fallDelta;
    public static float climbDelta;
    public static float carryDelta;
    public static float shipDelta;
    public static float driveDelta;
    public static float dumpsterDelta;
    private static int unlocked;
    private static int unlockedSinceStartup;
    private static int unlockedNsb;
    private static int unlockedSb;
    private static string lastUnlocked;
    private static float showSaved = 0f;
    private void Start()
    {
        Shell.RegisterCommand("sr", new Action(SteamResetAlias), null);
        Shell.RegisterCommand("steamunlock", new Action<string>(SteamUnlock), null);
        Shell.RegisterCommand("su", new Action<string>(SteamUnlock), null);
        Shell.RegisterCommand("showstats", new Action(ShowStats), null);
        Shell.RegisterCommand("ss", new Action(ShowStats), null);
        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
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
                    if (Int32.TryParse(option, out instance.idNum))
                    {
                        if (instance.idNum > 0 && instance.idNum <= achievementCount)
                        {
                            StatsAndAchievements.UnlockAchievement((Achievement)instance.idNum - 1, false, -1);
                        }
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
    }

    public void Update()
    {
        if (Game.instance != null)
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Main.guiOpened = !Main.guiOpened;
            }
            if (showSaved > 0f)
            {
                showSaved -= Time.unscaledDeltaTime;
            }
        }
    }

    private void ChangeStatsStyle(string size, string color)
    {
        int sizeint;
        if (!String.IsNullOrEmpty(color))
        {
            if (!ColorUtility.TryParseHtmlString(statsColorString.Substring(0, 1) == "#" ? color : "#" + color, out statsColor))
            {
                return;
            }
        }
        else
        {
            return;
        }
        if (!String.IsNullOrEmpty(size))
        {
            if (Int32.TryParse(size, out sizeint))
            {
                if (sizeint <= 0)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
        gUIStyle = new GUIStyle()
        {
            fontSize = sizeint,
            normal =
            {
                textColor = statsColor
            }
        };
    }

    public static int GetNSB()
    {
        int i = 0;
        foreach (Achievement achievement in StatsAndAchievements.unlocked)
        {
            if (achievement <= Achievement.ACH_SINGLE_RUN || achievement >= Achievement.ACH_INTRO_STATUE_HEAD || achievement == Achievement.ACH_FALL_1)
            {
                i++;
            }
        }
        return i;
    }

    public static int GetNSB(List<Achievement> achievements)
    {
        int i = 0;
        foreach (Achievement achievement in achievements)
        {
            if (achievement <= Achievement.ACH_SINGLE_RUN || achievement >= Achievement.ACH_INTRO_STATUE_HEAD || achievement == Achievement.ACH_FALL_1)
            {
                i++;
            }
        }
        return i;
    }

    public void OnGUI()
    {
        if (Game.instance != null)
        {
            if (isFirst)
            {
                gUIStyle = new GUIStyle()
                {
                    fontSize = 30,
                    normal =
                    {
                        textColor = statsColor
                    }
                };
                gUIStyle2 = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.LowerLeft,
                    normal =
                    {
                        textColor = statsColor
                    }
                };
                gUIStyle3 = new GUIStyle()
                {
                    fontSize = 30,
                    normal =
                    {
                        textColor = Color.red
                    }
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
                statsStyle = new GUIStyle(GUI.skin.textField)
                {
                    wordWrap = false,
                    alignment = TextAnchor.LowerLeft,
                    fixedWidth = 70

                };
                oldTravelM = StatsAndAchievements.travelM;
                oldFall = StatsAndAchievements.fall;
                oldClimbM = StatsAndAchievements.climbM;
                oldCarryM = StatsAndAchievements.carryM;
                oldShipM = StatsAndAchievements.shipM;
                oldDriveM = StatsAndAchievements.driveM;
                oldDumpsterM = StatsAndAchievements.dumpsterM;
                unlocked = unlockedSinceStartup = StatsAndAchievements.unlocked.Count;
                unlockedNsb = GetNSB(StatsAndAchievements.unlocked);
                unlockedSb = unlocked - unlockedNsb;
                isFirst = false;
                return;
            }
            if (Main.guiOpened)
            {
                mainWindowRect = GUILayout.Window(1399186700, mainWindowRect, WindowFunction, "Achievement Helper", guiLayoutOptions);
            }
            if (showStats)
            {
                if (StatsAndAchievements.travelM > oldTravelM)
                {
                    travelDelta = StatsAndAchievements.travelM - oldTravelM;
                    oldTravelM = StatsAndAchievements.travelM;
                }
                if (StatsAndAchievements.fall > oldFall)
                {
                    fallDelta = StatsAndAchievements.fall - oldFall;
                    oldFall = StatsAndAchievements.fall;
                }
                if (StatsAndAchievements.climbM > oldClimbM)
                {
                    climbDelta = StatsAndAchievements.climbM - oldClimbM;
                    oldClimbM = StatsAndAchievements.climbM;
                }
                if (StatsAndAchievements.carryM > oldCarryM)
                {
                    carryDelta = StatsAndAchievements.carryM - oldCarryM;
                    oldCarryM = StatsAndAchievements.carryM;
                }
                if (StatsAndAchievements.shipM > oldShipM)
                {
                    shipDelta = StatsAndAchievements.shipM - oldShipM;
                    oldShipM = StatsAndAchievements.shipM;
                }
                if (StatsAndAchievements.driveM > oldDriveM)
                {
                    driveDelta = StatsAndAchievements.driveM - oldDriveM;
                    oldDriveM = StatsAndAchievements.driveM;
                }
                if (StatsAndAchievements.dumpsterM > oldDumpsterM)
                {
                    dumpsterDelta = StatsAndAchievements.dumpsterM - oldDumpsterM;
                    oldDumpsterM = StatsAndAchievements.dumpsterM;
                }
                string[] values = { StatsAndAchievements.travelM.ToString(), travelDelta.ToString(), StatsAndAchievements.fall.ToString(), fallDelta.ToString(), StatsAndAchievements.jump.ToString(), Human.localPlayer.state != HumanState.Jump ? "True" : "False", StatsAndAchievements.climbM.ToString(), climbDelta.ToString(), StatsAndAchievements.carryM.ToString(), carryDelta.ToString(), StatsAndAchievements.drown.ToString(), StatsAndAchievements.shipM.ToString(), shipDelta.ToString(), StatsAndAchievements.driveM.ToString(), driveDelta.ToString(), StatsAndAchievements.dumpsterM.ToString(), dumpsterDelta.ToString() };
                GUILayout.Label(string.Format("TravelM {0} (+{1})\nFall {2} (+{3})\nJump {4} ({5})\nClimbM {6} (+{7})\nCarryM {8} (+{9})\nDrown {10}\nShipM {11} (+{12})\nDriveM {13} (+{14})\nDumpsterM {15} (+{16})", values), gUIStyle);
                if (unlocked > 0)
                {
                    string[] values1 = { unlocked.ToString(), achievementCount.ToString(), achievementCount == unlocked ? "Completed" : (achievementCount - unlocked).ToString() + " remaining", unlockedNsb.ToString(), (achievementCount - 11).ToString(), achievementCount - 11 == unlockedNsb ? "Completed" : (achievementCount - 11 - unlockedNsb).ToString() + " remaining", (unlocked - unlockedNsb).ToString(), "11", unlocked - unlockedNsb == 11 ? "Completed" : (11 - unlocked + unlockedNsb).ToString() + " remaining" };
                    GUILayout.Label(string.Format("Unlocked: {0}/{1} ({2})\nNSB: {3}/{4} ({5})\nSB: {6}/{7} ({8})", values1), gUIStyle);
                }
                if (unlocked > unlockedSinceStartup)
                {
                    GUILayout.Label(string.Format("Last unlocked: {0}", lastUnlocked), gUIStyle);
                }
                if (showSaved > 0f)
                {
                    GUILayout.Label("Saved!", gUIStyle3);
                }
            }
            if (!String.IsNullOrEmpty(achievementId))
            {
                if (achievementId.Length > Math.Ceiling(Math.Log10(achievementCount)))
                {
                    Debug.Log(achievementCount);
                    achievementId = achievementId.Substring(0, (int)Math.Ceiling(Math.Log10(achievementCount)));
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
            if (!String.IsNullOrEmpty(statsColorString))
            {
                if (ColorUtility.TryParseHtmlString(statsColorString.Substring(0, 1) == "#" ? statsColorString : "#" + statsColorString, out statsColor))
                {
                    gUIStyle2 = new GUIStyle(GUI.skin.label)
                    {
                        normal =
                        {
                            textColor = statsColor
                        }
                    };
                }
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
        GUILayout.BeginHorizontal();
        showStats = GUILayout.Toggle(showStats, "Show stats", guiLayoutOptions);
        statsSizeString = GUILayout.TextField(statsSizeString, statsStyle, guiLayoutOptions);
        statsColorString = GUILayout.TextField(statsColorString, statsStyle, guiLayoutOptions);
        GUILayout.Label("█", gUIStyle2);
        if (GUILayout.Button("Apply change", guiLayoutOptions))
        {
            ChangeStatsStyle(statsSizeString, statsColorString);
        }
        GUILayout.EndHorizontal();
        GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
    }

    [HarmonyPatch(typeof(StatsAndAchievements), "Save")]
    static class SaveDetector
    {
        [HarmonyPostfix]
        public static void DetectSave()
        {
            showSaved = 1f;
        }
    }

    [HarmonyPatch(typeof(StatsAndAchievements), "OnAchievementStored")]
    static class UnlockDetector
    {
        [HarmonyPostfix]
        public static void DetectSave(ref CGameID ___m_GameID, ref UserAchievementStored_t pCallback)
        {
            if ((ulong)___m_GameID == pCallback.m_nGameID)
            {
                if (pCallback.m_nMaxProgress == 0)
                {
                    unlocked++;
                    unlockedNsb = Main.GetNSB();
                    unlockedSb = unlocked - unlockedNsb;
                    lastUnlocked = Enum.GetName(typeof(Achievement), SteamAchievementMap.achievementMap[pCallback.m_rgchAchievementName]);
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CheatCodes), "SteamReset")]
    static class ResetDetector
    {
        [HarmonyPostfix]
        public static void OnReset()
        {
            unlocked = unlockedNsb = unlockedSinceStartup = 0;
        }
    }
}