using UnityEngine;

namespace Game.Configuration
{
    public static class InputSettings
    {
        static InputSettings()
        {
            MoveForward = KeyCode.W;
            MoveBackward = KeyCode.S;
            MoveRightward = KeyCode.D;
            MoveLeftward = KeyCode.A;

            Run = KeyCode.LeftShift;
            Jump = KeyCode.Space;
        }
        
        public static KeyCode MoveForward { get; set; }
        public static KeyCode MoveBackward { get; set; }
        public static KeyCode MoveRightward { get; set; }
        public static KeyCode MoveLeftward { get; set; }
        
        public static KeyCode Run { get; set; }
        
        public static KeyCode Jump { get; set; }
    }
}