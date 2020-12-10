using System;

namespace Game.Entities.Controllers
{
    /// <summary>
    /// Отвечает за режимы ввода в игре. 
    /// </summary>
    public static class GlobalInput
    {
        public enum InputMode
        {
            Character,
            Interface
        }
        
        public static InputMode Mode { get; private set; }

        public static event Action<InputMode> OnModeChanged = delegate(InputMode mode) {  };
        
        public static void SetCharacterMode()
        {
            Mode = InputMode.Character;
            OnModeChanged(Mode);
        }

        public static void SetInterfaceMode()
        {
            Mode = InputMode.Interface;
            OnModeChanged(Mode);
        }
    }
}