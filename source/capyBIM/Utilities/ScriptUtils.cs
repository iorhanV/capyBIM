using System.Windows.Input;

namespace capyBIM.Utilities;

public class ScriptUtils
{
    #region Keyboard key checks

    /// <summary>
    /// Verifies if the user is holding down the shift key.
    /// </summary>
    /// <returns>A boolean.</returns>
    public static bool KeyHeldShift()
    {
        return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
    }

    /// <summary>
    /// Verifies if the user is holding down the control key.
    /// </summary>
    /// <returns>A boolean.</returns>
    public static bool KeyHeldControl()
    {
        return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    }

    /// <summary>
    /// Verifies if the user is holding down the alt key.
    /// </summary>
    /// <returns>A boolean.</returns>
    public static bool KeyHeldAlt()
    {
        return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
    }

    #endregion
}