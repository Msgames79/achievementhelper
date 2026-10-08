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
using System.Security;
using UniverseLib.UI;
using UniverseLib.UI.Panels;
using UnityEngine.UI;
using Multiplayer;
using System.Configuration;
using System.Collections;
using Mono.Security.X509.Extensions;
using System.Xml.Linq;
namespace AchievementHelper;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Main : BaseUnityPlugin
{
    public static Main instance;
    public bool guiOpened;
    public Rect mainWindowRect;
    void ShellFunction(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            Debug.Log("Invalid argument");
            return;
        }
        string[] args = arg.Split(' ');
        if (args.Length == 0)
        {
            Debug.Log("Invalid argument");
            return;
        }
        switch (args[0].ToLower())
        {
            case "unlock":
            case "u":
                {
                    if (args.Length == 1)
                    {
                        AHUnlockAchievements("all");
                        return;
                    }
                    AHUnlockAchievements(args[1]);
                    return;
                }
            case "reset":
            case "r":
                {
                    AHResetAchievements();
                    return;
                }
            case "showstatus":
            case "ss":
                {
                    showStatus = !showStatus;
                    return;
                }
            default:
                {
                    Debug.Log("Invalid argument");
                    return;
                }
        }
    }
    void AHResetAchievements()
    {
        Shell.RawInvoke("steamreset");
    }
    void AHUnlockAchievements(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            Debug.Log("Invalid argument");
            return;
        }
        switch (arg)
        {
            case "all":
                {
                    AHUnlockAchievements("nsb");
                    AHUnlockAchievements("sb");
                    return;
                }
            case "nsb":
                {
                    foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
                    {
                        if (achievement <= Achievement.ACH_SINGLE_RUN || achievement >= Achievement.ACH_INTRO_STATUE_HEAD || achievement == Achievement.ACH_FALL_1)
                        {
                            if (!StatsAndAchievements.unlocked.Contains(achievement))
                            {
                                StatsAndAchievements.UnlockAchievement(achievement, false, -1);
                            }
                        }
                    }
                    return;
                }
            case "sb":
                {
                    StatsAndAchievements.travelM = StatsAndAchievements.travelM < 25000f ? 25000f : StatsAndAchievements.travelM;
                    StatsAndAchievements.climbM = StatsAndAchievements.climbM < 100f ? 100f : StatsAndAchievements.climbM;
                    StatsAndAchievements.carryM = StatsAndAchievements.carryM < 1000f ? 1000f : StatsAndAchievements.carryM;
                    StatsAndAchievements.shipM = StatsAndAchievements.shipM < 1000f ? 1000f : StatsAndAchievements.shipM;
                    StatsAndAchievements.driveM = StatsAndAchievements.driveM < 1000f ? 1000f : StatsAndAchievements.driveM;
                    StatsAndAchievements.dumpsterM = StatsAndAchievements.dumpsterM < 50f ? 50f : StatsAndAchievements.dumpsterM;
                    StatsAndAchievements.fall = StatsAndAchievements.fall < 100 ? 100 : StatsAndAchievements.fall;
                    StatsAndAchievements.jump = StatsAndAchievements.jump < 1000 ? 1000 : StatsAndAchievements.jump;
                    StatsAndAchievements.drown = StatsAndAchievements.drown < 10 ? 10 : StatsAndAchievements.drown;
                    StatsAndAchievements.Save();
                    return;
                }
            default:
                {
                    int idNum;
                    if (Int32.TryParse(arg, out idNum))
                    {
                        Array numbers = Enum.GetValues(typeof(Achievement));
                        if ((idNum >= 1 && idNum <= 11) || idNum == 15 || (idNum >= 24 && idNum <= numbers.Length))
                        {
                            if (StatsAndAchievements.unlocked.Contains((Achievement)idNum - 1))
                            {
                                Debug.Log("Already unlocked");
                                return;
                            }
                            StatsAndAchievements.UnlockAchievement((Achievement)idNum - 1, false, -1);
                            return;
                        }
                        else if (idNum >= 12 && idNum <= 23)
                        {
                            switch ((Achievement)idNum - 1)
                            {
                                case Achievement.ACH_TRAVEL_1KM:
                                    {
                                        StatsAndAchievements.travelM = StatsAndAchievements.travelM < 1000f ? 1000f : StatsAndAchievements.travelM;
                                        break;
                                    }
                                case Achievement.ACH_TRAVEL_10KM:
                                    {
                                        StatsAndAchievements.travelM = StatsAndAchievements.travelM < 10000f ? 10000f : StatsAndAchievements.travelM;
                                        break;
                                    }
                                case Achievement.ACH_TRAVEL_100KM:
                                    {
                                        StatsAndAchievements.travelM = StatsAndAchievements.travelM < 25000f ? 25000f : StatsAndAchievements.travelM;
                                        break;
                                    }
                                case Achievement.ACH_FALL_1000:
                                    {
                                        StatsAndAchievements.fall = StatsAndAchievements.fall < 100 ? 100 : StatsAndAchievements.fall;
                                        break;
                                    }
                                case Achievement.ACH_JUMP_1000:
                                    {
                                        StatsAndAchievements.jump = StatsAndAchievements.jump < 1000 ? 1000 : StatsAndAchievements.jump;
                                        break;
                                    }
                                case Achievement.ACH_CLIMB_100M:
                                    {
                                        StatsAndAchievements.climbM = StatsAndAchievements.climbM < 100f ? 100f : StatsAndAchievements.climbM;
                                        break;
                                    }
                                case Achievement.ACH_CARRY_1000M:
                                    {
                                        StatsAndAchievements.carryM = StatsAndAchievements.carryM < 1000f ? 1000f : StatsAndAchievements.carryM;
                                        break;
                                    }
                                case Achievement.ACH_DROWN_10:
                                    {
                                        StatsAndAchievements.drown = StatsAndAchievements.drown < 10 ? 10 : StatsAndAchievements.drown;
                                        break;
                                    }
                                case Achievement.ACH_SHIP_1000M:
                                    {
                                        StatsAndAchievements.shipM = StatsAndAchievements.shipM < 1000f ? 1000f : StatsAndAchievements.shipM;
                                        break;
                                    }
                                case Achievement.ACH_DRIVE_1000M:
                                    {
                                        StatsAndAchievements.driveM = StatsAndAchievements.driveM < 1000f ? 1000f : StatsAndAchievements.driveM;
                                        break;
                                    }
                                case Achievement.ACH_DUMPSTER_50M:
                                    {
                                        StatsAndAchievements.dumpsterM = StatsAndAchievements.dumpsterM < 50f ? 50f : StatsAndAchievements.dumpsterM;
                                        break;
                                    }
                            }
                            StatsAndAchievements.Save();
                        }
                        else
                        {
                            Debug.Log("Out of range");
                        }
                    }
                    else if (String.IsNullOrWhiteSpace(arg))
                    {
                        AHUnlockAchievements("all");
                    }
                    else
                    {
                        Debug.Log("Invalid argument");
                    }
                    return;
                }
        }
    }
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
    private static float showSaved;
    private static GUIStyle ahStatusStyle;
    private static int ahStatusSize = 20;
    private static Color ahStatusColor;
    private static int ahStatusColorInt = 0xFF00FF;
    private static bool doUpdteAhStatusStyle;
    private static bool firstUpdate = true;
    void Start()
    {
        Shell.RegisterCommand("achievementhelper", new Action<string>(ShellFunction), null);
        Shell.RegisterCommand("ah", new Action<string>(ShellFunction), null);
        mainWindowRect = new Rect(720f, 60f, 0f, 0f);
        instance = this;
        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
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
    private readonly int achievementCount = Enum.GetValues(typeof(Achievement)).Length;
    void Update()
    {
        if (Game.instance != null)
        {
            if (firstUpdate)
            {
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
                travelDelta = climbDelta = carryDelta = shipDelta = driveDelta = dumpsterDelta = 0;
                fallDelta = 0;
                firstUpdate = false;
            }
            if (Input.GetKeyDown(KeyCode.Home))
            {
                guiOpened = !guiOpened;
            }
            if (showSaved > 0f)
            {
                showSaved -= Time.unscaledDeltaTime;
                if (showSaved <= 0f)
                {
                }
            }
            if (doUpdteAhStatusStyle)
            {
                if (ColorUtility.TryParseHtmlString("#" + ahStatusColorInt.ToString("X6"), out ahStatusColor))
                {
                    ahStatusStyle = new GUIStyle()
                    {
                        fontSize = ahStatusSize,
                        normal =
                        {
                            textColor = ahStatusColor
                        }
                    };
                    doUpdteAhStatusStyle = false;
                }
            }
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
            if (showStatus)
            {
                ahStatus.Clear();
                for (int i = 0; i < statusStrings.Length; i++)
                {
                    if (statusShowing[i])
                    {
                        switch (statusStrings[i])
                        {
                            case "TravelM":
                                {
                                    ahStatus.AppendLine(string.Format("TravelM {0} (+{1}) ({2}/3)", StatsAndAchievements.travelM, travelDelta, StatsAndAchievements.travelM < 1000f ? 0 : StatsAndAchievements.travelM < 10000f ? 1 : StatsAndAchievements.travelM < 25000f ? 2 : 3));
                                    break;
                                }
                            case "Fall":
                                {
                                    ahStatus.AppendLine(string.Format("Fall {0} (+{1})", StatsAndAchievements.fall, fallDelta));
                                    break;
                                }
                            case "Jump":
                                {
                                    ahStatus.AppendLine(string.Format("Jump {0} ({1})", StatsAndAchievements.jump, Human.localPlayer.state != HumanState.Jump ? "True" : "False"));
                                    break;
                                }
                            case "ClimbM":
                                {
                                    ahStatus.AppendLine(string.Format("ClimbM {0} (+{1})", StatsAndAchievements.climbM, climbDelta));
                                    break;
                                }
                            case "CarryM":
                                {
                                    ahStatus.AppendLine(string.Format("CarryM {0} (+{1})", StatsAndAchievements.carryM, carryDelta));
                                    break;
                                }
                            case "Drown":
                                {
                                    ahStatus.AppendLine(string.Format("Drown {0}", StatsAndAchievements.drown));
                                    break;
                                }
                            case "ShipM":
                                {
                                    ahStatus.AppendLine(string.Format("ShipM {0} (+{1})", StatsAndAchievements.shipM, shipDelta));
                                    break;
                                }
                            case "DriveM":
                                {
                                    ahStatus.AppendLine(string.Format("DriveM {0} (+{1})", StatsAndAchievements.driveM, driveDelta));
                                    break;
                                }
                            case "DumpsterM":
                                {
                                    ahStatus.AppendLine(string.Format("DumpsterM {0} (+{1})", StatsAndAchievements.dumpsterM, dumpsterDelta));
                                    break;
                                }
                            case "UnlockedCount":
                                {
                                    ahStatus.AppendLine(string.Format("Unlocked: {0}/{1} ({2})", unlocked, achievementCount, achievementCount == unlocked ? "Completed" : (achievementCount - unlocked).ToString() + " remaining"));
                                    break;
                                }
                            case "UnlockedCountNSB":
                                {
                                    ahStatus.AppendLine(string.Format("Unlocked(NSB): {0}/{1} ({2})", unlockedNsb, achievementCount - 11, achievementCount - 11 == unlockedNsb ? "Completed" : (achievementCount - 11 - unlockedNsb).ToString() + " remaining"));
                                    break;
                                }
                            case "UnlockedCountSB":
                                {
                                    ahStatus.AppendLine(string.Format("Unlocked(SB): {0}/{1} ({2})", unlockedSb, 11, unlocked - unlockedNsb == 11 ? "Completed" : (11 - unlocked + unlockedNsb).ToString() + " remaining"));
                                    break;
                                }
                            case "LastUnlocked":
                                {
                                    if (unlocked > unlockedSinceStartup)
                                    {
                                        ahStatus.AppendLine(string.Format("Last unlocked: {0}", lastUnlocked == null ? "None" : lastUnlocked));
                                    }
                                    break;
                                }
                            case "LastUnlockedNSB":
                                {
                                    if (unlocked > unlockedSinceStartup)
                                    {
                                        ahStatus.AppendLine(string.Format("Last unlocked(NSB): {0}", lastUnlockedNsb == null ? "None" : lastUnlockedNsb));
                                    }
                                    break;
                                }
                            case "LastUnlockedSB":
                                {
                                    if (unlocked > unlockedSinceStartup)
                                    {
                                        ahStatus.AppendLine(string.Format("Last unlocked(SB): {0}", lastUnlockedSb == null ? "None" : lastUnlockedSb));
                                    }
                                    break;
                                }
                            case "IsSingleRun":
                                {
                                    if (!StatsAndAchievements.unlocked.Contains(Achievement.ACH_SINGLE_RUN))
                                    {
                                        ahStatus.AppendLine(string.Format("IsSingleRun: {0}", Game.instance.singleRun));
                                    }
                                    break;
                                }
                            case "SavedTiming":
                                {
                                    if (showSaved > 0f)
                                    {
                                        ahStatus.AppendLine("Saved!");
                                    }
                                    break;
                                }
                        }
                    }
                }
                if (ahStatus.Length > 0)
                {
                    ahStatus.Length--;
                }
            }
        }
    }
    private static int achIndex = 1;
    private static bool showStatus = false;
    private static bool oldShowStatus = false;
    private static GUILayoutOption[] expandOptions;
    private static GUIStyle noWrapLabelStyle;
    private static GUIStyle noWrapTextFieldStyle;
    private static GUIStyle noWrapButtonStyle;
    private static GUIStyle noWrapToggleStyle;
    private static string[] statusStrings = { "TravelM", "Fall", "Jump", "ClimbM", "CarryM", "Drown", "ShipM", "DriveM", "DumpsterM", "UnlockedCount", "UnlockedCountNSB", "UnlockedCountSB", "LastUnlocked", "LastUnlockedNSB", "LastUnlockedSB", "IsSingleRun", "SavedTiming" };
    private static BitArray statusShowing = new BitArray(statusStrings.Length, true);
    private static float ahStatusX, ahStatusY;
    void WindowFunction(int windowId)
    {
        switch (windowId)
        {
            case 1399186700:
                {
                    using (var horizontalScope = new GUILayout.HorizontalScope())
                    {
                        using (var verticalScope = new GUILayout.VerticalScope())
                        {
                            if (GUILayout.Button("Unlock NSB achievements", noWrapButtonStyle, expandOptions))
                            {
                                AHUnlockAchievements("nsb");
                            }
                            if (GUILayout.Button("Unlock SB achievements", noWrapButtonStyle, expandOptions))
                            {
                                AHUnlockAchievements("sb");
                            }
                        }
                        using (var verticalScope = new GUILayout.VerticalScope())
                        {
                            if (GUILayout.Button("Unlock all achievements", noWrapButtonStyle, expandOptions))
                            {
                                AHUnlockAchievements("all");
                            }
                            if (GUILayout.Button("Reset achievements", noWrapButtonStyle, expandOptions))
                            {
                                AHResetAchievements();
                            }
                        }
                    }
                    GUILayout.Label(((Achievement)achIndex - 1).ToString(), noWrapLabelStyle, expandOptions);
                    using (var horizontalScope = new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("<<", noWrapButtonStyle))
                        {
                            achIndex = 1;
                            mainWindowRect.width = mainWindowRect.height = 0;
                        }
                        if (GUILayout.Button("-10", noWrapButtonStyle))
                        {
                            if (achIndex >= 11)
                            {
                                achIndex -= 10;
                            }
                            else
                            {
                                achIndex = 1;
                            }
                            mainWindowRect.width = mainWindowRect.height = 0;
                        }
                        if (GUILayout.Button("-", noWrapButtonStyle))
                        {
                            if (achIndex >= 2)
                            {
                                achIndex -= 1;
                                mainWindowRect.width = mainWindowRect.height = 0;
                            }
                        }
                        GUILayout.Label(achIndex.ToString(), noWrapTextFieldStyle, expandOptions);
                        if (GUILayout.Button("+", noWrapButtonStyle))
                        {
                            if (achIndex < achievementCount)
                            {
                                achIndex += 1;
                                mainWindowRect.width = mainWindowRect.height = 0;
                            }
                        }
                        if (GUILayout.Button("+10", noWrapButtonStyle))
                        {
                            if (achIndex < achievementCount - 9)
                            {
                                achIndex += 10;
                            }
                            else
                            {
                                achIndex = achievementCount;
                            }
                            mainWindowRect.width = mainWindowRect.height = 0;
                        }
                        if (GUILayout.Button(">>", noWrapButtonStyle))
                        {
                            achIndex = achievementCount;
                            mainWindowRect.width = mainWindowRect.height = 0;
                        }
                        if (GUILayout.Button("Unlock this Achievement", noWrapButtonStyle, expandOptions))
                        {
                            AHUnlockAchievements(achIndex.ToString());
                        }
                    }
                    showStatus = GUILayout.Toggle(showStatus, "Show status", noWrapToggleStyle, expandOptions);
                    if (oldShowStatus != showStatus)
                    {
                        mainWindowRect.width = mainWindowRect.height = 0;
                        oldShowStatus = showStatus;
                    }
                    if (showStatus)
                    {
                        using (var horizontalScope = new GUILayout.HorizontalScope())
                        {
                            using (var verticalScope = new GUILayout.VerticalScope())
                                for (int i = 0; i < statusStrings.Length; i++)
                                {
                                    statusShowing[i] = GUILayout.Toggle(statusShowing[i], statusStrings[i], noWrapToggleStyle, expandOptions);
                                }
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                {
                                    bool tempFlag;
                                    string tempStr;
                                    for (int i = 0; i < statusStrings.Length; i++)
                                    {
                                        using (var horizontalScope1 = new GUILayout.HorizontalScope())
                                        {
                                            if (i < statusStrings.Length - 1)
                                            {
                                                if (GUILayout.Button("↓", expandOptions))
                                                {
                                                    tempFlag = statusShowing[i];
                                                    tempStr = statusStrings[i];
                                                    statusShowing[i] = statusShowing[i + 1];
                                                    statusStrings[i] = statusStrings[i + 1];
                                                    statusShowing[i + 1] = tempFlag;
                                                    statusStrings[i + 1] = tempStr;
                                                }
                                            }
                                            if (i > 0)
                                            {
                                                if (GUILayout.Button("↑", expandOptions))
                                                {
                                                    tempFlag = statusShowing[i];
                                                    tempStr = statusStrings[i];
                                                    statusShowing[i] = statusShowing[i - 1];
                                                    statusStrings[i] = statusStrings[i - 1];
                                                    statusShowing[i - 1] = tempFlag;
                                                    statusStrings[i - 1] = tempStr;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        using (var horizontalScope = new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("<<", noWrapButtonStyle))
                            {
                                ahStatusSize = 1;
                                mainWindowRect.width = mainWindowRect.height = 0;
                                doUpdteAhStatusStyle = true;
                            }
                            if (GUILayout.Button("-10", noWrapButtonStyle))
                            {
                                if (ahStatusSize > 11)
                                {
                                    ahStatusSize -= 10;
                                }
                                else
                                {
                                    ahStatusSize = 1;
                                }
                                mainWindowRect.width = mainWindowRect.height = 0;
                                doUpdteAhStatusStyle = true;
                            }
                            if (GUILayout.Button("-", noWrapButtonStyle))
                            {
                                if (ahStatusSize > 1)
                                {
                                    ahStatusSize -= 1;
                                    mainWindowRect.width = mainWindowRect.height = 0;
                                }
                                doUpdteAhStatusStyle = true;
                            }
                            GUILayout.Label(ahStatusSize.ToString(), noWrapTextFieldStyle, expandOptions);
                            if (GUILayout.Button("+", noWrapButtonStyle))
                            {
                                ahStatusSize += 1;
                                mainWindowRect.width = mainWindowRect.height = 0;
                                doUpdteAhStatusStyle = true;
                            }
                            if (GUILayout.Button("+10", noWrapButtonStyle))
                            {
                                ahStatusSize += 10;
                                mainWindowRect.width = mainWindowRect.height = 0;
                                doUpdteAhStatusStyle = true;
                            }
                        }
                        using (var horizontalScope = new GUILayout.HorizontalScope())
                        {
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                if (GUILayout.Button("R-10", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0xFF0000) >= 0x0A0000)
                                    {
                                        ahStatusColorInt -= 0x0A0000;
                                    }
                                    else
                                    {
                                        ahStatusColorInt &= 0x00FFFF;
                                    }
                                    mainWindowRect.width = mainWindowRect.height = 0;
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("G-10", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0x00FF00) >= 0x000A00)
                                    {
                                        ahStatusColorInt -= 0x000A00;
                                    }
                                    else
                                    {
                                        ahStatusColorInt &= 0xFF00FF;
                                    }
                                    mainWindowRect.width = mainWindowRect.height = 0;
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("B-10", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0x0000FF) >= 0x00000A)
                                    {
                                        ahStatusColorInt -= 0x00000A;
                                    }
                                    else
                                    {
                                        ahStatusColorInt &= 0xFFFF00;
                                    }
                                    mainWindowRect.width = mainWindowRect.height = 0;
                                    doUpdteAhStatusStyle = true;
                                }
                            }
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                if (GUILayout.Button("R-", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0xFF0000) >= 0x010000)
                                    {
                                        ahStatusColorInt -= 0x010000;
                                        mainWindowRect.width = mainWindowRect.height = 0;
                                    }
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("G-", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0x00FF00) >= 0x000100)
                                    {
                                        ahStatusColorInt -= 0x000100;
                                        mainWindowRect.width = mainWindowRect.height = 0;
                                    }
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("B-", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0x0000FF) >= 0x000001)
                                    {
                                        ahStatusColorInt -= 0x000001;
                                        mainWindowRect.width = mainWindowRect.height = 0;
                                    }
                                    doUpdteAhStatusStyle = true;
                                }
                            }
                            GUILayout.Label(ahStatusColorInt.ToString("X6"), noWrapTextFieldStyle, expandOptions);
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                if (GUILayout.Button("R+", noWrapButtonStyle))
                                {
                                    if (ahStatusColorInt <= 0xFE0000)
                                    {
                                        ahStatusColorInt += 0x010000;
                                        mainWindowRect.width = mainWindowRect.height = 0;
                                    }
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("G+", noWrapButtonStyle))
                                {
                                    if (ahStatusColorInt <= 0x00FE00)
                                    {
                                        ahStatusColorInt += 0x000100;
                                        mainWindowRect.width = mainWindowRect.height = 0;
                                    }
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("B+", noWrapButtonStyle))
                                {
                                    if (ahStatusColorInt <= 0x0000FE)
                                    {
                                        ahStatusColorInt += 0x000001;
                                        mainWindowRect.width = mainWindowRect.height = 0;
                                    }
                                    doUpdteAhStatusStyle = true;
                                }
                            }
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                if (GUILayout.Button("R+10", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0xFF0000) <= 0xF50000)
                                    {
                                        ahStatusColorInt += 0x0A0000;
                                    }
                                    else
                                    {
                                        ahStatusColorInt |= 0xFF0000;
                                    }
                                    mainWindowRect.width = mainWindowRect.height = 0;
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("G+10", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0x00FF00) <= 0x00F500)
                                    {
                                        ahStatusColorInt += 0x000A00;
                                    }
                                    else
                                    {
                                        ahStatusColorInt |= 0x00FF00;
                                    }
                                    mainWindowRect.width = mainWindowRect.height = 0;
                                    doUpdteAhStatusStyle = true;
                                }
                                if (GUILayout.Button("B+10", noWrapButtonStyle))
                                {
                                    if ((ahStatusColorInt & 0x0000FF) <= 0x0000F5)
                                    {
                                        ahStatusColorInt += 0x00000A;
                                    }
                                    else
                                    {
                                        ahStatusColorInt |= 0x0000FF;
                                    }
                                    mainWindowRect.width = mainWindowRect.height = 0;
                                    doUpdteAhStatusStyle = true;
                                }
                            }
                        }

                        using (var horizontalScope = new GUILayout.HorizontalScope())
                        {
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                GUILayout.Label("X", noWrapLabelStyle, expandOptions);
                                GUILayout.Label("Y", noWrapLabelStyle, expandOptions);
                            }
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                ahStatusX = GUILayout.HorizontalSlider(ahStatusX, 0, Screen.width, expandOptions);
                                ahStatusY = GUILayout.HorizontalSlider(ahStatusY, 0, Screen.height, expandOptions);
                            }
                            using (var verticalScope = new GUILayout.VerticalScope())
                            {
                                GUILayout.Label(ahStatusX.ToString(), noWrapLabelStyle, expandOptions);
                                GUILayout.Label(ahStatusY.ToString(), noWrapLabelStyle, expandOptions);
                            }
                        }
                        using (var horizontalScope = new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(string.Format("Anchor: {0}", statusAlign[statusAlignIndex]), noWrapButtonStyle, expandOptions))
                            {
                                statusAlignIndex = (statusAlignIndex + 1) % 4;
                                doUpdteAhStatusStyle = true;
                            }
                        }
                    }
                    break;
                }
            default:
                {
                    return;
                }
        }
        GUI.DragWindow();
    }
    private static StringBuilder ahStatus = new StringBuilder();
    private static int statusAlignIndex;
    private static string[] statusAlign = { "TopLeft", "TopRight", "BottomLeft", "BottomRight" };
    void OnGUI()
    {
        if (expandOptions == null)
        {
            expandOptions =
            [
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
                GUILayout.MaxWidth(Screen.width)
            ];
            noWrapLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
            noWrapTextFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
            noWrapButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
            noWrapToggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
            ahStatusStyle = new GUIStyle()
            {
                fontSize = ahStatusSize,
                normal =
                {
                    textColor = ahStatusColor
                }
            };
            doUpdteAhStatusStyle = true;
        }
        if (guiOpened)
        {
            mainWindowRect = GUILayout.Window(1399186700, mainWindowRect, WindowFunction, "Achievement Helper", expandOptions);
        }
        if (showStatus)
        {
            switch (statusAlignIndex)
            {
                case 0:
                    {
                        GUI.Label(new Rect(ahStatusX, ahStatusY, Screen.width, Screen.height), ahStatus.ToString(), ahStatusStyle);
                        break;
                    }
                case 1:
                    {
                        GUI.Label(new Rect(Screen.width - ahStatusStyle.CalcSize(new GUIContent(ahStatus.ToString())).x - ahStatusX, ahStatusY, Screen.width, Screen.height), ahStatus.ToString(), ahStatusStyle);
                        break;
                    }
                case 2:
                    {
                        GUI.Label(new Rect(ahStatusX, Screen.height - ahStatusStyle.CalcSize(new GUIContent(ahStatus.ToString())).y - ahStatusY, Screen.width, Screen.height), ahStatus.ToString(), ahStatusStyle);
                        break;
                    }
                case 3:
                    {
                        GUI.Label(new Rect(Screen.width - ahStatusStyle.CalcSize(new GUIContent(ahStatus.ToString())).x - ahStatusX, Screen.height - ahStatusStyle.CalcSize(new GUIContent(ahStatus.ToString())).y - ahStatusY, Screen.width, Screen.height), ahStatus.ToString(), ahStatusStyle);
                        break;
                    }
            }
        }
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
                    unlockedNsb = GetNSB(StatsAndAchievements.unlocked);
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
            travelDelta = climbDelta = carryDelta = shipDelta = driveDelta = dumpsterDelta = 0;
            fallDelta = 0;
        }
    }
}
