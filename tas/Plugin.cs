/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Ident.Minigame;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Ident.TAS
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

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

                        var m_choiceButtons = (System.Collections.Generic.List<ChoiceButton>)(___m_DialogueUI
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
        }
    }

    [HarmonyPatch(typeof(miniGame), "Update")]
    public class miniGame_Update
    {
        static bool paused = false;
        static bool skipped = false;
        private static void Prefix(miniGame __instance, bool ___m_playable)
        {
            if (!__instance.menuObjectLink.paused
                && ___m_playable
                && __instance.menuObjectLink.isFullyVisible
                && !__instance.animator.IsRunning()
                && !paused)
            {
                paused = true;
                var keyboard = InputSystem.GetDevice<Keyboard>();
                InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Escape));
                InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Escape));
            }

            // TODO: This should happen when the pause menu is open
            // if (paused && __instance.pauseLink.menuObjectLink.canvasGroup.interactable && !skipped) {
            //     skipped = true;
            //     var keyboard = InputSystem.GetDevice<Keyboard>();
            //     InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.DownArrow));
            //     InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.DownArrow));
            // }
        }
    }
}
