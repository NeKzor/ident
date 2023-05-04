/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

using HarmonyLib;
using Ident.Minigame;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Ident.TAS;

// Hook into the defrag mini game so we can press the skip button in the pause menu :^)
[HarmonyPatch(typeof(miniGame), "Update")]
public class miniGame_Update
{
    public static bool MovedCursor = false;
    public static bool Once = false;

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

            Once = false;
            InputSystem.GetDevice<Keyboard>()
                .QueueState(new KeyboardState().PressKey(Key.Escape))
                .QueueState(new KeyboardState().ReleaseKey(Key.Escape));

            InputSystem.QueueStateEvent(InputSystem.GetDevice<Mouse>(), new MouseState() {}.WithButton(MouseButton.Left));
        }

        // Once paused, find the skip button and select it.
        if (__instance.menuObjectLink.paused)
        {
            if (!Once) {
                Once = true;
                InputSystem.QueueStateEvent(InputSystem.GetDevice<Mouse>(), new MouseState() {}.WithButton(MouseButton.Left, false));
            }

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
                        .QueueState(new KeyboardState().ReleaseKey(Key.Space));
                }
                else
                {
                    //var cursor = GameManager.Instance.Environment.VirtualCursor;
                    var buttonPosition = button.transform.position;
                    var camera = GameManager.Instance.Cameras.UiCamera;

                    var screenPoint = camera.WorldToScreenPoint(buttonPosition);
                    screenPoint.x -= MovedCursor ? 30 : 20;
                    MovedCursor = !MovedCursor;

                    Plugin.Log.LogInfo($"Want to move mouse to = {screenPoint} | Button = {buttonPosition} | Mouse = {Mouse.current.position}");

                    InputSystem.GetDevice<Mouse>().WarpCursorPosition(screenPoint);
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
