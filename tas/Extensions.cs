/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Ident.TAS;

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
