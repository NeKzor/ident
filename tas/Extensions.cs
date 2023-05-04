/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Ident.TAS;

// Extensions which provide a slightly better API for queueing keyboard/mouse states to the input system
// and the ability to access private fields/properties of objects.
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
    public static MouseState PressButton(this MouseState state, MouseButton button)
    {
        return state.WithButton(button, true);
    }
    public static MouseState ReleaseButton(this MouseState state, MouseButton button)
    {
        return state.WithButton(button, false);
    }
    public static Mouse QueueState(this Mouse mouse, MouseState state)
    {
        InputSystem.QueueStateEvent(mouse, state);
        return mouse;
    }
    public static Mouse SetPosition(this Mouse mouse, Vector3 screenPoint)
    {
        mouse.WarpCursorPosition(screenPoint);
        return mouse;
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
