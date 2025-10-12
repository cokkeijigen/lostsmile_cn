using System;
using System.Runtime.InteropServices;
using UnityEngine;

// iTsukezigen++
namespace CHSPatch
{
    public class Settings
    {

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String className, String windowName);

        public static void Init() 
        {
            try
            {
                IntPtr windowPtr = FindWindow(null, Application.productName);
                if (windowPtr != System.IntPtr.Zero)
                {
                    SetWindowText(windowPtr, "【星美岛绿茶品鉴中心】 LOSTSMILE 简体中文版 Beta.1.0");
                }
            }
            catch (Exception ex) 
            {
                Logger.OutMessage($"{ex.Message}");
            }
        }

    }
}
