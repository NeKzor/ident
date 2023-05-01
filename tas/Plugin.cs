/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Ident.Minigame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Ident.TAS
{
    // ID of each map hotspot location.
    public enum MapHotspotLocationId : int
    {
        Lobby = 0,
        AdminOffice = 1,
        Library = 2,
        LandingPad = 3,
        Vault = 4,
    }

    // Game scenes.
    public static class Scene
    {
        public const string Intro = "titlesequence";
        public const string Map = "locMap";
        public const string Credits = "locCredits";
    }

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

        // Plugin initialization.
        private void Awake()
        {
            Instance = this;
            Log = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
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
                IsTimerRunning = true;
            else if (IsTimerRunning && gameManager.sceneHandlerLink.currentScene == Scene.Credits)
                IsTimerRunning = false;

            if (IsTimerRunning)
                TotalTime += Time.unscaledDeltaTime;
        }

        // Draw the speedrun timer.
        private void OnGUI()
        {
            // NOTE: Position depends on screen resolution which might not be ideal.

            GUI.Label(
                new(10, Screen.height - 90, 64, 10),
                System.TimeSpan.FromSeconds(TotalTime).ToString("mm':'ss'.'fff"),
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

    // Extensions which provide a slightly better API for queueing key states to the input system
    // and the ability to access private properties of objects.
    public static class UnityExtensions
    {
        public static KeyboardState PressKey(this KeyboardState state, Key key)
        {
            state.Set(key, true);
            return state;
        }
        public static KeyboardState ReleaseKey(this KeyboardState state, Key key)
        {
            state.Set(key, false);
            return state;
        }
        public static Keyboard QueueState(this Keyboard keyboard, KeyboardState state)
        {
            InputSystem.QueueStateEvent(keyboard, state);
            return keyboard;
        }
        public static T GetField<T>(this object @object, string name, System.Type type)
        {
            var property = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)(property?.GetValue(@object) ?? default);
        }
        public static T GetProperty<T>(this object @object, string name, System.Type type)
        {
            var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)(property?.GetValue(@object) ?? default);
        }
    }

    // Hook into the dialogue manager and handle each dialogue state if it is playing,
    // otherwise we want to handle the intro or the location map.
    [HarmonyPatch(typeof(DialogueManager), "Update")]
    public class DialogueManager_Update
    {
        private static void Prefix(
            DialogueManager __instance,
            Ink.Runtime.Story ___m_story,
            DialogueUIPanel ___m_DialogueUI
        )
        {
            if (!Plugin.Instance.IsTimerRunning)
                return;

            if (__instance.isPlaying)
            {
                switch (__instance.state)
                {
                    case DialogueManager.State.Dialogue:
                        {
                            Plugin.Log.LogInfo("Continue");

                            InputSystem.GetDevice<Keyboard>()
                                .QueueState(new KeyboardState().PressKey(Key.P))
                                .QueueState(new KeyboardState().ReleaseKey(Key.P));

                            break;
                        }
                    case DialogueManager.State.Choice:
                        {
                            Plugin.Log.LogInfo("Choose");

                            InputSystem.GetDevice<Keyboard>()
                                .QueueState(new KeyboardState().PressKey(Key.RightArrow))
                                .QueueState(new KeyboardState().ReleaseKey(Key.RightArrow));

                            // Get all route options based on the current conversation.

                            var conversation = __instance.conversation.info;
                            if (!Routes.English.TryGetValue(conversation, out var options))
                            {
                                Plugin.Log.LogWarning($"Conversation {conversation} not found");
                                break;
                            }

                            // Get the button index for the choice we have to make  based on the current text.

                            var text = ___m_story.currentText.TrimEnd();
                            if (!options.TryGetValue(text, out var buttonStrategy))
                            {
                                Plugin.Log.LogWarning($"Story text {text} not found");
                                break;
                            }

                            var (buttonIndex, requiredButtonText) = buttonStrategy;
                            var m_choiceButtons = ___m_DialogueUI
                                .GetField<List<ChoiceButton>>("m_choiceButtons", typeof(DialogueUIPanel));

                            // Simply select the first button if we don't have to move down.
                            if (buttonIndex == 0)
                            {
                                var choiceButtonText = m_choiceButtons.First()
                                    .GetField<string>("m_title", typeof(ChoiceButton));
                                if (choiceButtonText != requiredButtonText)
                                {
                                    Plugin.Log.LogWarning("choiceButtonText != requiredButtonText");
                                    Plugin.Log.LogWarning($"{choiceButtonText} != {requiredButtonText}");
                                    break;
                                }

                                InputSystem.GetDevice<Keyboard>()
                                    .QueueState(new KeyboardState().PressKey(Key.Space))
                                    .QueueState(new KeyboardState().ReleaseKey(Key.Space));

                                break;
                            }

                            // Try to find the right choice by starting to select the next button.

                            InputSystem.GetDevice<Keyboard>()
                                .QueueState(new KeyboardState().PressKey(Key.DownArrow));

                            var index = -1;

                            foreach (var choiceButton in m_choiceButtons)
                            {
                                index += 1;
                                Plugin.Log.LogInfo($"Button index = {index} | Want index = {buttonIndex}");

                                if (index != buttonIndex)
                                    continue;

                                if (choiceButton.button is not UnityEngine.UI.Button button)
                                    continue;

                                var hasSelection = button
                                    .GetProperty<bool>("hasSelection", typeof(UnityEngine.UI.Selectable));

                                Plugin.Log.LogInfo($"hasSelection = {hasSelection}");

                                if (hasSelection is not true)
                                    continue;

                                var choiceButtonText = choiceButton.GetField<string>("m_title", typeof(ChoiceButton));
                                if (choiceButtonText != requiredButtonText)
                                {
                                    Plugin.Log.LogWarning("choiceButtonText != requiredButtonText");
                                    Plugin.Log.LogWarning($"{choiceButtonText} != {requiredButtonText}");
                                    break;
                                }

                                // We selected the correct button, so let's make the choice.

                                InputSystem.GetDevice<Keyboard>()
                                    .QueueState(new KeyboardState().ReleaseKey(Key.DownArrow))
                                    .QueueState(new KeyboardState().PressKey(Key.Space))
                                    .QueueState(new KeyboardState().ReleaseKey(Key.Space));

                                break;
                            }

                            break;
                        }
                    case DialogueManager.State.NonChoiceButton:
                        {
                            Plugin.Log.LogInfo("Defrag/Stitch/End");

                            // TODO: Is right arrow input even needed here?

                            InputSystem.GetDevice<Keyboard>()
                                .QueueState(new KeyboardState().PressKey(Key.RightArrow))
                                .QueueState(new KeyboardState().ReleaseKey(Key.RightArrow))
                                .QueueState(new KeyboardState().PressKey(Key.Space))
                                .QueueState(new KeyboardState().ReleaseKey(Key.Space));

                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                switch (__instance.menuConductorLink.sceneHandlerLink.currentScene)
                {
                    case Scene.Intro:
                        {
                            Plugin.Log.LogInfo("Skip");

                            InputSystem.GetDevice<Keyboard>()
                                .QueueState(new KeyboardState().PressKey(Key.Escape))
                                .QueueState(new KeyboardState().ReleaseKey(Key.Escape));

                            break;
                        }
                    case Scene.Map:
                        {
                            // Find the next location ID based on the last conversation.
                            // NOTE: This is not correct if we load from a save.

                            var conversation = __instance.conversation?.info ?? "a1_s3_adminOfficeFirstMeeting";
                            if (!Plugin.NextLocations.TryGetValue(conversation, out var nextLocation))
                            {
                                Plugin.Log.LogWarning($"Next location for {conversation} not found");
                                break;
                            }

                            var env = GameManager.Instance.Environment;
                            var m_hotspots = env
                                .GetField<HashSet<EnvironmentHotspot>>("m_hotspots", typeof(EnvironmentManager));

                            // Find the next hotspot that we want to move to based on the location ID.

                            var nextHotspot = m_hotspots.FirstOrDefault((hotspot) =>
                            {
                                return hotspot
                                    .GetField<int?>("_LocationID", hotspot.GetType()) == (int)nextLocation;
                            });

                            if (nextHotspot is null)
                            {
                                Plugin.Log.LogWarning($"Hotspot location id {nextLocation} not found");
                                break;
                            }

                            // Check if the cursor is highlighting the hotspot, then load it.

                            var cursor = GameManager.Instance.Environment.VirtualCursor;
                            var m_highlighted = cursor
                                .GetField<EnvironmentHotspot>("m_highlighted", typeof(VirtualCursor));

                            if (m_highlighted == nextHotspot)
                            {
                                InputSystem.GetDevice<Keyboard>()
                                    .QueueState(new KeyboardState()
                                        .PressKey(Key.Space)
                                        .ReleaseKey(Key.RightArrow)
                                        .ReleaseKey(Key.LeftArrow)
                                        .ReleaseKey(Key.UpArrow)
                                        .ReleaseKey(Key.DownArrow)
                                    )
                                    .QueueState(new KeyboardState().ReleaseKey(Key.Space));

                                break;
                            }

                            // Otherwise, move until the cursor highlights the hotspot location.

                            var state = new KeyboardState();
                            var hotspotPosition = nextHotspot.GetViewportPosition();
                            var cursorPosition = cursor.position;

                            Plugin.Log.LogInfo($"Target = {hotspotPosition} | Cursor = {cursorPosition}");

                            if (hotspotPosition.x > cursorPosition.x)
                                state = state.PressKey(Key.RightArrow).ReleaseKey(Key.LeftArrow);
                            else if (hotspotPosition.x < cursorPosition.x)
                                state = state.PressKey(Key.LeftArrow).ReleaseKey(Key.RightArrow);

                            if (hotspotPosition.y > cursorPosition.y)
                                state = state.PressKey(Key.UpArrow).ReleaseKey(Key.DownArrow);
                            else if (hotspotPosition.y < cursorPosition.y)
                                state = state.PressKey(Key.DownArrow).ReleaseKey(Key.UpArrow);

                            InputSystem.GetDevice<Keyboard>().QueueState(state);

                            break;
                        }
                }
            }
        }
    }

    // Hook into the defrag mini game so we can press the skip button in the pause menu :^)
    [HarmonyPatch(typeof(miniGame), "Update")]
    public class miniGame_Update
    {
        private static void Prefix(miniGame __instance, bool ___m_playable)
        {
            if (!Plugin.Instance.IsTimerRunning)
                return;

            // NOTE: The game will buffer our pause input until the animator has finished.
            // TODO: Buffering does not matter, right?
            if (!__instance.menuObjectLink.paused
                && ___m_playable
                && __instance.menuObjectLink.isFullyVisible
                && !__instance.animator.IsRunning())
            {
                Plugin.Log.LogInfo("Pause");

                InputSystem.GetDevice<Keyboard>()
                    .QueueState(new KeyboardState().PressKey(Key.Escape))
                    .QueueState(new KeyboardState().ReleaseKey(Key.Escape))
                    .QueueState(new KeyboardState().PressKey(Key.DownArrow));
            }

            // Once paused, find the skip button and select it.
            if (__instance.menuObjectLink.paused)
            {
                var button = __instance.pauseLink.storyDefragButtons.Find((button) => button.name == "SkipMinigame");
                if (button)
                {
                    var hasSelection = button
                        .GetComponent<PlatformLayer.PLUI_Button>()
                        .GetProperty<bool>("hasSelection", typeof(UnityEngine.UI.Selectable));

                    if (hasSelection is true)
                    {
                        Plugin.Log.LogInfo("Skip");

                        InputSystem.GetDevice<Keyboard>()
                            .QueueState(new KeyboardState().PressKey(Key.Space))
                            .QueueState(new KeyboardState().ReleaseKey(Key.Space))
                            .QueueState(new KeyboardState().ReleaseKey(Key.DownArrow));
                    }
                }
            }

            // Continue when the end result screen shows up.
            if (__instance.resultsScreen.menuObjectLink.isVisible)
            {
                Plugin.Log.LogInfo("Continue");

                InputSystem.GetDevice<Keyboard>()
                    .QueueState(new KeyboardState().PressKey(Key.Space))
                    .QueueState(new KeyboardState().ReleaseKey(Key.Space));
            }
        }
    }
}
