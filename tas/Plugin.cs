/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Ident.TAS;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; }
    public static ManualLogSource Log { get; private set; }

    // Here we map every conversation that ends with transitioning into the location map
    // to the next hotspot location that the cursor has to move to.
    public static Dictionary<string, MapHotspotLocationId> NextLocations = new()
        {
            { "a1_s3_adminOfficeFirstMeeting", MapHotspotLocationId.Library },
            // For some reason the "landingPad" is called "vault" and the "vault" is called "landingPad" :>
            { "a1_s4_firstVisitToLibrary", MapHotspotLocationId.LandingPad },
            { "a1_s5_b_firstCassEncounterPostGame", MapHotspotLocationId.AdminOffice },
            { "a1_s6_b_returnToAdminPostGame", MapHotspotLocationId.Vault },
            { "a2_s1_landingPadProxyArrives", MapHotspotLocationId.LandingPad },
            { "a2_s2_b_returnToCassPostGame", MapHotspotLocationId.Library },
            { "a2_s3_sierras question", MapHotspotLocationId.Lobby },
            { "a2_s4_b_lobbyGrishCheckInPostGame", MapHotspotLocationId.AdminOffice },
            { "a2_s5_adminReportBack", MapHotspotLocationId.Vault },
            { "a2_s6_landingPadProxyOutcome", MapHotspotLocationId.LandingPad},
            { "a2_s7_b_cassIsNotAGuardPostGame", MapHotspotLocationId.Library },
            { "a2_s8_LibraryClosure", MapHotspotLocationId.LandingPad },
            { "a2_s9_b_AnotherVaultExplosionPostGame", MapHotspotLocationId.AdminOffice },
            { "a3_s1_alt1_admin", MapHotspotLocationId.Vault },
        };

    public float TotalTime { get; private set; } = 0.0f;
    public bool IsTimerRunning { get; private set; } = false;
    public float LastSplit { get; private set; } = 0.0f;

    public string TotalTimeFormatted => System.TimeSpan.FromSeconds(TotalTime).ToString("mm':'ss'.'fff");
    public string LastDurationFormatted => System.TimeSpan.FromSeconds(TotalTime - LastSplit).ToString("mm':'ss'.'fff");

    public void Split()
    {
        LastSplit = TotalTime;
    }

    // Plugin initialization.
    private void Awake()
    {
        Instance = this;
        Log = Logger;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"Loaded {PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} by NeKz :^)");
    }

    // Start, stop or update the speedrun timer.
    private void Update()
    {
        var gameManager = Singleton<DialogueManager>.Instance;
        if (!gameManager)
            return;

        var m_isInSceneTransition = gameManager.sceneHandlerLink
            .GetField<bool>("m_isInSceneTransition", typeof(sceneHandler));

        if (m_isInSceneTransition)
            return;

        if (!IsTimerRunning && gameManager.sceneHandlerLink.currentScene == Scene.Intro)
        {
            TotalTime = 0.0f;
            LastSplit = 0.0f;
            IsTimerRunning = true;
        }
        else if (IsTimerRunning && gameManager.sceneHandlerLink.currentScene == Scene.Credits)
        {
            Plugin.Log.LogInfo(TotalTimeFormatted);
            IsTimerRunning = false;
        }

        if (IsTimerRunning)
            TotalTime += Time.unscaledDeltaTime;
    }

    // Draw the speedrun timer.
    private void OnGUI()
    {
        // NOTE: Position depends on screen resolution which might not be ideal.

        GUI.Label(
            new(10, Screen.height - 90, 64, 10),
            TotalTimeFormatted,
            new()
            {
                fontSize = 18,
                normal = new()
                {
                    textColor = new(255, 255, 255),
                    background = GUI.skin.label.normal.background,
                },
            }
        );
    }
}
