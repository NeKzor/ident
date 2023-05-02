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
