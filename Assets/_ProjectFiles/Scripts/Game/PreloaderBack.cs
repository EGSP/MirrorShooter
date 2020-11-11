﻿using Mirror;
using UnityEngine;

namespace Game
{
    // Входная точка в виде статического класса, которая будет вызываться в Preloader
    public static class PreloaderBack
    {
        public static void Awake(Preloader preloader)
        {
            Application.targetFrameRate = 60;
        }

        public static void AfterAwake(Preloader preloader)
        {
            
        }

        public static void Start(Preloader preloader)
        {
            
        }

        public static void AfterStart(Preloader preloader)
        {
            
        }
    }
}