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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Ident.TAS
{
    public enum MapHotspotLocationId : int
    {
        Lobby = 0,
        AdminOffice = 1,
        Library = 2,
        LandingPad = 3,
        Vault = 4,
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        public static Dictionary<string, MapHotspotLocationId> NextLocations = new()
        {
            { "a1_s3_adminOfficeFirstMeeting", MapHotspotLocationId.Library },
            // We need to go to the vault after this but for some reason it is called "landingPad".
            // It will be called "vault" once we unlock the real landing pad. I wonder why that is :>
            { "a1_s4_firstVisitToLibrary", MapHotspotLocationId.LandingPad },
            { "a1_s5_b_firstCassEncounterPostGame", MapHotspotLocationId.AdminOffice },
            { "a1_s6_b_returnToAdminPostGame", MapHotspotLocationId.Vault },
            { "a2_s1_landingPadProxyArrives", MapHotspotLocationId.LandingPad },
            { "a2_s2_b_returnToCassPostGame", MapHotspotLocationId.Library },
            { "a2_s3_sierras question", MapHotspotLocationId.Lobby },
            { "a2_s4_b_lobbyGrishCheckInPostGame", MapHotspotLocationId.AdminOffice },
            { "a2_s5_adminReportBack", MapHotspotLocationId.LandingPad },
            { "a2_s6_landingPadProxyOutcome", MapHotspotLocationId.Vault},
            { "a2_s7_b_cassIsNotAGuardPostGame", MapHotspotLocationId.Library },
            { "a2_s8_LibraryClosure", MapHotspotLocationId.Vault },
            { "a2_s9_b_AnotherVaultExplosionPostGame", MapHotspotLocationId.AdminOffice },
            { "a3_s1_alt1_admin", MapHotspotLocationId.LandingPad },
        };

        private void Awake()
        {
            Log = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static KeyboardState PressKey(Key key)
        {
            return new KeyboardState(key);
        }
        public static KeyboardState ReleaseKey(Key key)
        {
            var state = new KeyboardState();
            state.Release(key);
            return state;
        }
    }

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
        public static void QueueState(this Keyboard keyboard, KeyboardState state)
        {
            InputSystem.QueueStateEvent(keyboard, state);
        }
    }

    [HarmonyPatch(typeof(DialogueManager), "Update")]
    public class DialogueManager_Update
    {
        private static void Prefix(
            DialogueManager __instance,
            Ink.Runtime.Story ___m_story,
            DialogueUIPanel ___m_DialogueUI
        )
        {
            if (__instance.isPlaying)
            {
                var keyboard = InputSystem.GetDevice<Keyboard>();

                switch (__instance.state)
                {
                    case DialogueManager.State.Dialogue:
                        Plugin.Log.LogInfo("Continue");
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.P));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.P));
                        break;
                    case DialogueManager.State.Choice:
                        Plugin.Log.LogInfo("Choose");
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.RightArrow));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.RightArrow));

                        var conversation = __instance.conversation.info;
                        if (!Routes.English.TryGetValue(conversation, out var options))
                        {
                            Plugin.Log.LogWarning($"Conversation {conversation} not found");
                            break;
                        }

                        var text = ___m_story.currentText.TrimEnd();
                        if (!options.TryGetValue(text, out var buttonIndex))
                        {
                            Plugin.Log.LogWarning($"Story text {text} not found");
                            break;
                        }

                        if (buttonIndex == 0)
                        {
                            InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Space));
                            InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Space));
                            break;
                        }

                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.DownArrow));

                        var m_choiceButtons = (List<ChoiceButton>)(___m_DialogueUI
                            .GetType()
                            .GetField("m_choiceButtons", BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(___m_DialogueUI));

                        var index = 0;

                        foreach (var choiceButton in m_choiceButtons)
                        {
                            if (choiceButton.button is UnityEngine.UI.Button button)
                            {
                                var hasSelection = (bool?)(typeof(UnityEngine.UI.Selectable)
                                    .GetProperty("hasSelection", BindingFlags.NonPublic | BindingFlags.Instance)
                                    .GetValue(button));

                                if (hasSelection == true && index == buttonIndex)
                                {
                                    InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.DownArrow));
                                    InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Space));
                                    InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Space));
                                }
                            }

                            Plugin.Log.LogInfo($"Button index: {index}");
                            index += 1;
                        }
                        break;
                    case DialogueManager.State.NonChoiceButton:
                        Plugin.Log.LogInfo("Defrag/Stitch/End");
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.RightArrow));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.RightArrow));
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Space));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Space));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (__instance.menuConductorLink.sceneHandlerLink.currentScene)
                {
                    case "titlesequence":
                        {
                            Plugin.Log.LogInfo("Skip");
                            var keyboard = InputSystem.GetDevice<Keyboard>();
                            InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Escape));
                            InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Escape));
                            break;
                        }
                    case "locMap":
                        {
                            var env = GameManager.Instance.Environment;
                            var m_hotspots = (HashSet<EnvironmentHotspot>)(env
                                .GetType()
                                .GetField("m_hotspots", BindingFlags.NonPublic | BindingFlags.Instance)
                                .GetValue(env));

                            var conversation = __instance.conversation?.info ?? "a1_s3_adminOfficeFirstMeeting";
                            if (!Plugin.NextLocations.TryGetValue(conversation, out var nextLocation))
                            {
                                Plugin.Log.LogWarning($"Next location for {conversation} not found");
                                break;
                            }

                            var mouse = InputSystem.GetDevice<Mouse>();
                            var cursor = GameManager.Instance.Environment.VirtualCursor;
                            var camera = GameManager.Instance.Cameras.UiCamera;

                            var nextHotspot = m_hotspots.FirstOrDefault((hotspot) => {
                                var _LocationID = (int)(hotspot
                                    .GetType()
                                    .GetField("_LocationID", BindingFlags.NonPublic | BindingFlags.Instance)
                                    .GetValue(hotspot));

                                return _LocationID == (int)nextLocation;
                            });

                            if (nextHotspot is null)
                                break;

                            var m_highlighted = (EnvironmentHotspot)(typeof(VirtualCursor)
                                    .GetField("m_highlighted", BindingFlags.NonPublic | BindingFlags.Instance)
                                    .GetValue(cursor));

                            var keyboard = InputSystem.GetDevice<Keyboard>();
                            var state = new KeyboardState();

                            if (m_highlighted == nextHotspot)
                            {
                                keyboard.QueueState(state.PressKey(Key.Space)
                                    .ReleaseKey(Key.RightArrow)
                                    .ReleaseKey(Key.LeftArrow)
                                    .ReleaseKey(Key.UpArrow)
                                    .ReleaseKey(Key.DownArrow)
                                );
                                keyboard.QueueState(new KeyboardState().ReleaseKey(Key.Space));
                                break;
                            }

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

                            keyboard.QueueState(state);

                            // var screenPoint = camera.ViewportToScreenPoint(hotspotPosition);

                            // Plugin.Log.LogInfo($"Want to move mouse to = {screenPoint} | Viewport = {hotspotPosition}");

                            // InputState.Change(mouse, new MouseState() { position = screenPoint });
                            // //InputSystem.QueueStateEvent(mouse, new MouseState() { position = screenPoint });

                            // Plugin.Log.LogInfo($"{PlatformLayer.PLManager.Get().InputManager.MouseIsInActiveUse()} | {PlatformLayer.PLManager.Get().InputManager.UINavigationMode.GetCurrentMode()}");

                            break;
                        }
                }
            }
        }
    }

    [HarmonyPatch(typeof(miniGame), "Update")]
    public class miniGame_Update
    {
        private static void Prefix(miniGame __instance, bool ___m_playable)
        {
            if (!__instance.menuObjectLink.paused
                && ___m_playable
                && __instance.menuObjectLink.isFullyVisible
                && !__instance.animator.IsRunning())
            {
                Plugin.Log.LogInfo("Pause");
                var keyboard = InputSystem.GetDevice<Keyboard>();
                InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Escape));
                InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Escape));
                InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.DownArrow));
            }

            if (__instance.menuObjectLink.paused)
            {
                var button = __instance.pauseLink.storyDefragButtons.Find((button) => button.name == "SkipMinigame");
                if (button)
                {
                    var hasSelection = (bool?)(typeof(UnityEngine.UI.Selectable)
                       .GetProperty("hasSelection", BindingFlags.NonPublic | BindingFlags.Instance)
                       .GetValue(button.GetComponent<PlatformLayer.PLUI_Button>()));

                    if (hasSelection == true)
                    {
                        Plugin.Log.LogInfo("Skip");
                        var keyboard = InputSystem.GetDevice<Keyboard>();
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Space));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Space));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.DownArrow));
                    }
                }
            }

            if (__instance.resultsScreen.menuObjectLink.isVisible)
            {
                Plugin.Log.LogInfo("Continue");
                var keyboard = InputSystem.GetDevice<Keyboard>();
                InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Space));
                InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Space));
            }
        }
    }
}
