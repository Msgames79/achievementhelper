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
    private static Achievement? lastUnlocked = null;
    private static Achievement? lastUnlockedNsb = null;
    private static Achievement? lastUnlockedSb = null;
    private static float showSaved = 0f;
    private static bool doStatsUpdate = false;
    private static object[] values;
    private static object[] values1;
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
        doStatsUpdate = true;
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
        doStatsUpdate = true;
    }
    public static void ShowStats()
    {
        instance.showStats = !instance.showStats;
        if (instance.showStats)
        {
            doStatsUpdate = true;
        }
    }
    public void Awake()
    {
        mainWindowRect = new Rect(330, 750, 381, 200);
        guiLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(false),
            GUILayout.MaxWidth(Screen.width)
        };
        guiOpened = false;
        instance = this;
    }

    public void Update()
    {
        if (Game.instance != null)
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                guiOpened = !guiOpened;
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
        if (!string.IsNullOrEmpty(color))
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
        if (!string.IsNullOrEmpty(size))
        {
            if (int.TryParse(size, out sizeint))
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
            if (guiOpened)
            {
                mainWindowRect = GUILayout.Window(1399186700, mainWindowRect, WindowFunction, "Achievement Helper", guiLayoutOptions);
            }
            if (showStats)
            {
                if (StatsAndAchievements.travelM > oldTravelM)
                {
                    travelDelta = StatsAndAchievements.travelM - oldTravelM;
                    oldTravelM = StatsAndAchievements.travelM;
                    doStatsUpdate = true;
                }
                if (StatsAndAchievements.fall > oldFall)
                {
                    fallDelta = StatsAndAchievements.fall - oldFall;
                    oldFall = StatsAndAchievements.fall;
                    doStatsUpdate = true;
                }
                if (StatsAndAchievements.climbM > oldClimbM)
                {
                    climbDelta = StatsAndAchievements.climbM - oldClimbM;
                    oldClimbM = StatsAndAchievements.climbM;
                    doStatsUpdate = true;
                }
                if (StatsAndAchievements.carryM > oldCarryM)
                {
                    carryDelta = StatsAndAchievements.carryM - oldCarryM;
                    oldCarryM = StatsAndAchievements.carryM;
                    doStatsUpdate = true;
                }
                if (StatsAndAchievements.shipM > oldShipM)
                {
                    shipDelta = StatsAndAchievements.shipM - oldShipM;
                    oldShipM = StatsAndAchievements.shipM;
                    doStatsUpdate = true;
                }
                if (StatsAndAchievements.driveM > oldDriveM)
                {
                    driveDelta = StatsAndAchievements.driveM - oldDriveM;
                    oldDriveM = StatsAndAchievements.driveM;
                    doStatsUpdate = true;
                }
                if (StatsAndAchievements.dumpsterM > oldDumpsterM)
                {
                    dumpsterDelta = StatsAndAchievements.dumpsterM - oldDumpsterM;
                    oldDumpsterM = StatsAndAchievements.dumpsterM;
                    doStatsUpdate = true;
                }
                if (doStatsUpdate)
                {
                    values = [StatsAndAchievements.travelM, travelDelta, StatsAndAchievements.travelM < 1000f ? 0 : StatsAndAchievements.travelM < 10000f ? 1 : StatsAndAchievements.travelM < 25000f ? 2 : 3, StatsAndAchievements.fall, fallDelta, StatsAndAchievements.jump, Human.localPlayer.state != HumanState.Jump ? "True" : "False", StatsAndAchievements.climbM, climbDelta, StatsAndAchievements.carryM, carryDelta, StatsAndAchievements.drown, StatsAndAchievements.shipM, shipDelta, StatsAndAchievements.driveM, driveDelta, StatsAndAchievements.dumpsterM, dumpsterDelta, unlocked, achievementCount, achievementCount == unlocked ? "Completed" : (achievementCount - unlocked).ToString() + " remaining", unlockedNsb, achievementCount - 11, achievementCount - 11 == unlockedNsb ? "Completed" : (achievementCount - 11 - unlockedNsb).ToString() + " remaining", (unlocked - unlockedNsb), unlocked - unlockedNsb == 11 ? "Completed" : (11 - unlocked + unlockedNsb).ToString() + " remaining"];
                    values1 = [lastUnlocked, lastUnlockedNsb == null ? "None" : lastUnlockedNsb, lastUnlockedSb == null ? "None" : lastUnlockedSb];
                }
                GUILayout.Label(string.Format("TravelM {0} (+{1}) ({2}/3)\nFall {3} (+{4})\nJump {5} ({6})\nClimbM {7} (+{8})\nCarryM {9} (+{10})\nDrown {11}\nShipM {12} (+{13})\nDriveM {14} (+{15})\nDumpsterM {16} (+{17})\nUnlocked: {18}/{19} ({20})\nNSB: {21}/{22} ({23})\nSB: {24}/11 ({25})", values), gUIStyle);
                if (unlocked > unlockedSinceStartup)
                {
                    GUILayout.Label(string.Format("Last unlocked: {0}\nNSB: {1}\nSB: {2}", values1), gUIStyle);
                }
                if (!StatsAndAchievements.unlocked.Contains(Achievement.ACH_SINGLE_RUN))
                {
                    GUILayout.Label($"Single run: {Game.instance.singleRun}", gUIStyle);
                    }
                if (showSaved > 0f)
                {
                    GUILayout.Label("Saved!", gUIStyle3);
                }
            }
            if (!string.IsNullOrEmpty(achievementId))
            {
                if (achievementId.Length > Math.Ceiling(Math.Log10(achievementCount)))
                {
                    achievementId = achievementId.Substring(0, (int)Math.Ceiling(Math.Log10(achievementCount)));
                }
                if (int.TryParse(achievementId, out idNum) && idNum > 0 && idNum <= achievementCount)
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
        if (GUILayout.Button("Show stats", guiLayoutOptions))
        {
            ShowStats();
        }
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
                    unlockedNsb = GetNSB();
                    unlockedSb = unlocked - unlockedNsb;
                    lastUnlocked = SteamAchievementMap.achievementMap[pCallback.m_rgchAchievementName];
                    if (lastUnlocked <= Achievement.ACH_SINGLE_RUN || lastUnlocked >= Achievement.ACH_INTRO_STATUE_HEAD || lastUnlocked == Achievement.ACH_FALL_1)
                    {
                        lastUnlockedNsb = lastUnlocked;
                    }
                    else
                    {
                        lastUnlockedSb = lastUnlocked;
                    }
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
            lastUnlocked = lastUnlockedNsb = lastUnlockedSb = null;
        }
    }
}