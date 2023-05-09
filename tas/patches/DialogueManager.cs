/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Ident.TAS;

// Hook into the dialogue manager and handle each dialogue state if it is playing,
// otherwise we want to handle the intro or the location map.
[HarmonyPatch(typeof(DialogueManager), "Update")]
public class DialogueManager_Update
{
    public static bool MovedCursor = false;
    public static string LastScene = "";

    private static void Postfix(
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
                        InputSystem.GetDevice<Keyboard>()
                            .QueueState(new KeyboardState().PressKey(Key.P))
                            .QueueState(new KeyboardState().ReleaseKey(Key.P));

                        break;
                    }
                case DialogueManager.State.Choice:
                    {
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

                        // Move the mouse to the button position and select it.

                        var (buttonIndex, requiredButtonText) = buttonStrategy;
                        var m_choiceButtons = ___m_DialogueUI
                            .GetField<List<ChoiceButton>>("m_choiceButtons", typeof(DialogueUIPanel));

                        var choiceButton = m_choiceButtons[buttonIndex];
                        var buttonPosition = choiceButton.transform.position;
                        var camera = GameManager.Instance.Cameras.UiCamera;

                        var screenPoint = camera.WorldToScreenPoint(buttonPosition);
                        screenPoint.x -= 20;

                        InputSystem.GetDevice<Mouse>()
                            .SetPosition(screenPoint);

                        if (choiceButton.button is not UnityEngine.UI.Button button)
                            break;

                        var hasSelection = button
                            .GetProperty<bool>("hasSelection", typeof(UnityEngine.UI.Selectable));

                        if (hasSelection is not true)
                            break;

                        InputSystem.GetDevice<Keyboard>()
                            .QueueState(new KeyboardState().PressKey(Key.Space))
                            .QueueState(new KeyboardState().ReleaseKey(Key.Space));

                        break;
                    }
                case DialogueManager.State.NonChoiceButton:
                    {
                        var m_StitchButton = ___m_DialogueUI
                            .GetField<UnityEngine.GameObject>("m_StitchButton", typeof(DialogueUIPanel));

                        var buttonPosition = m_StitchButton.transform.position;
                        var camera = GameManager.Instance.Cameras.UiCamera;

                        var screenPoint = camera.WorldToScreenPoint(buttonPosition);
                        screenPoint.x -= 20;

                        InputSystem.GetDevice<Mouse>()
                            .SetPosition(screenPoint)
                            .QueueState(new MouseState().PressButton(MouseButton.Left));

                        InputSystem.GetDevice<Keyboard>()
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
                                .QueueState(new KeyboardState().PressKey(Key.Space))
                                .QueueState(new KeyboardState().ReleaseKey(Key.Space));

                            break;
                        }

                        // Otherwise, move the mouse to the hotspot location.

                        var hotspotPosition = nextHotspot.GetViewportPosition();
                        var camera = GameManager.Instance.Cameras.UiCamera;

                        var screenPoint = camera.ViewportToScreenPoint(hotspotPosition);
                        screenPoint.x += MovedCursor ? 1 : 0;
                        MovedCursor = !MovedCursor;

                        InputSystem.GetDevice<Mouse>()
                            .SetPosition(screenPoint)
                            .QueueState(new MouseState().PressButton(MouseButton.Left));

                        break;
                    }
                case Scene.NoScene:
                    {
                        if (LastScene != __instance.menuConductorLink.sceneHandlerLink.currentScene)
                        {
                            var time = Plugin.Instance.TotalTimeFormatted;
                            var duration = Plugin.Instance.LastDurationFormatted;

                            Plugin.Log.LogInfo($"{time} | {duration} | {LastScene}");

                            Plugin.Instance.Split();
                        }
                        break;
                    }
            }

            LastScene = __instance.menuConductorLink.sceneHandlerLink.currentScene;
        }
    }
}
