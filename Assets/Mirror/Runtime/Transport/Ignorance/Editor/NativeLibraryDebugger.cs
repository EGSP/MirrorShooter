#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Mirror
{
    public class NativeLibraryDebugger
    {
        [MenuItem("Ignorance Debugging/Enet Library Name")]
        public static void RevealEnetLibraryName()
        {
            EditorUtility.DisplayDialog("Enet Library Name", $"Use this for debugging.\nYour platform expects the native Enet library to be called: {ENet.Native.nativeLibraryName}", "Got it");
        }
    }
}
#endif
