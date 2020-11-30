using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using InputStructs;

public static class InputManager
{
    private static Dictionary<string, KeyCode> keyBinds;
    public static Dictionary<string, KeyCode> KeyBinds { get; private set; }
    private static Dictionary<string, InputAxis> axes;
    public static Dictionary<string, InputAxis> Axes { get; private set; }

    static readonly string[] bindNames = new string[]
    {
        "Confirm",
        "Shoot",
        "Focus",
        "Pause"
    };

    static readonly KeyCode[] defaultKeys =
    {
        KeyCode.Space,
        KeyCode.X,
        KeyCode.LeftShift,
        KeyCode.Escape
    };

    static readonly string[] axisNames = new string[]
        {
            "MoveUp",
            "MoveRight"
        };

    static readonly InputAxis[] standartAxes = new InputAxis[]
    {
        new InputAxis(KeyCode.UpArrow, KeyCode.DownArrow),
        new InputAxis(KeyCode.RightArrow, KeyCode.LeftArrow)
    };

    static InputManager() { SetupInputs(); }

    /// <summary>
    /// Setting up the InputDictionary
    /// </summary>
    private static void SetupInputs()
    {
        keyBinds = new Dictionary<string, KeyCode>();
        for (int i = 0; i < bindNames.Length; i++)
        {
            keyBinds.Add(bindNames[i], defaultKeys[i]);
        }

        axes = new Dictionary<string, InputAxis>();
        for (int i = 0; i < axisNames.Length; i++)
        {
            axes.Add(axisNames[i], standartAxes[i]);
        }
    }

    /// <summary>
    /// Resets all Keybinds.
    /// </summary>
    public static void ResetAllBinds()
    {
        for (int i = 0; i < bindNames.Length; i++)
        {
            keyBinds[bindNames[i]] = defaultKeys[i];
        }
    }

    /// <summary>
    /// Used for setting a Keybind.
    /// </summary>
    /// <param name="name">The name of the Keybind that is meant to be set.</param>
    /// <param name="key">The Key it should be set to.</param>
    /// <returns>Whether the setting was successful.</returns>
    public static bool SetKeyBind(string name, KeyCode key)
    {
        if (!keyBinds.ContainsKey(name)) return false;
        var values = keyBinds.Values.ToArray();
        foreach (var usedKey in values)
        {
            if (key == usedKey) return false;
        }
        keyBinds[name] = key;
        return true;
    }

    /// <summary>
    /// Used for getting what key is assigned to a Keybind.
    /// </summary>
    /// <param name="name">The name of the Keybind.</param>
    /// <returns>The assigned Key.</returns>
    public static KeyCode GetKeyBindKey(string name)
    {
        keyBinds.TryGetValue(name, out KeyCode output);
        return output;
    }

    /// <summary>
    /// True once when a key bind is pressed.
    /// </summary>
    /// <param name="bindName">The Key bind</param>
    public static bool GetKeyDown(string bindName)
    {
        return Input.GetKeyDown(keyBinds[bindName]);
    }

    /// <summary>
    /// True while a key bind is pressed.
    /// </summary>
    /// <param name="bindName">The Key bind</param>
    public static bool GetKey(string bindName)
    {
        return Input.GetKey(keyBinds[bindName]);
    }

    /// <summary>
    /// True when a key bind is let go.
    /// </summary>
    /// <param name="bindName">The Key bind</param>
    public static bool GetKeyUp(string bindName)
    {
        return Input.GetKeyUp(keyBinds[bindName]);
    }

    /// <summary>
    /// Returns the value of a given axis <b>without smoothing</b>.
    /// </summary>
    /// <param name="axisName">The given axis.</param>
    public static float GetAxisRaw(string axisName)
    {
        float val = 0;
        if (Input.GetKey(axes[axisName].Positive)) val += 1;
        if (Input.GetKey(axes[axisName].Negative)) val -= 1;
        return val;
    }

    /// <summary>
    /// Gets a list of all KeyBinds as KeyBind struct. Used for saving KeyBinds.
    /// </summary>
    /// <returns>The list of keybinds.</returns>
    public static List<KeyBind> GetKeyBindList()
    {
        var keys = keyBinds.Keys.ToArray();
        var values = keyBinds.Values.ToArray();
        var output = new List<KeyBind>();
        for (int i = 0; i < keys.Length; i++)
        {
            output.Add(new KeyBind(keys[i], values[i]));
        }
        return output;
    }
}

namespace InputStructs
{
    [Serializable]
    public struct KeyBind
    {
        public KeyBind(string name, KeyCode key)
        {
            Name = name;
            Key = key;
        }
        public string Name;
        public KeyCode Key;
    }

    [Serializable]
    public struct AxisBind
    {
        public AxisBind(string name, InputAxis keys)
        {
            Name = name;
            Keys = keys;
        }
        public string Name;
        public InputAxis Keys;
    }

    [Serializable]
    public struct InputAxis
    {
        public InputAxis(KeyCode positive, KeyCode negative)
        {
            Positive = positive;
            Negative = negative;
        }
        public KeyCode Positive;
        public KeyCode Negative;
    }
}