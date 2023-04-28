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
    }

    [HarmonyPatch(typeof(miniGame), "Update")]
    public class Patch_miniGame_Update
    {
        static bool once = false;
        private static void Prefix(miniGame __instance, bool ___m_playable)
        {
            if (!__instance.menuObjectLink.paused
                && ___m_playable
                && __instance.menuObjectLink.isFullyVisible
                && !__instance.animator.IsRunning()
                && !once)
            {
                once = true;
                var state = new KeyboardState();
                state.Press(Key.Escape);
                var keyboard = InputSystem.GetDevice<Keyboard>();
                InputSystem.QueueStateEvent(keyboard, state);

            }
        }
    }
}
