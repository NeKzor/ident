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
        private static void Prefix(DialogueManager __instance)
        {
            if (__instance.isPlaying)
            {
                var keyboard = InputSystem.GetDevice<Keyboard>();

                switch (__instance.state)
                {
                    case DialogueManager.State.Dialogue:
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.P));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.P));
                        Plugin.Log.LogInfo("Continue");
                        break;
                    case DialogueManager.State.Choice:
                        // TODO: Insert menu down key presses based on gi.m_story.currentText
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Space));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Space));
                        Plugin.Log.LogInfo("Choose");
                        break;
                    case DialogueManager.State.NonChoiceButton:
                        InputSystem.QueueStateEvent(keyboard, Plugin.PressKey(Key.Space));
                        InputSystem.QueueStateEvent(keyboard, Plugin.ReleaseKey(Key.Space));
                        Plugin.Log.LogInfo("Defrag/Stitch/End");
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
