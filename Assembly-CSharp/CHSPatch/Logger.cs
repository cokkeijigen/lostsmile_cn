using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// iTsukezigen++
namespace CHSPatch
{
    public class Logger
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void LoggerDebugMessageFromCPP(string message);

        private static readonly LoggerDebugMessageFromCPP LoggerDebugMessage = null;

        static Logger()
        {
            IntPtr hDll = GetModuleHandle("LOSTSMILE_CN.dll");
            if (hDll != IntPtr.Zero)
            {
                LoggerDebugMessage = (LoggerDebugMessageFromCPP)Marshal.GetDelegateForFunctionPointer(
                    GetProcAddress(hDll, "DebugMessage"), typeof(LoggerDebugMessageFromCPP)
                );
            }
        }

        public static void OutMessage(string message, bool breakline = true)
        {
            if (message != null)
            {
                if(breakline) message += '\n';
                Logger.LoggerDebugMessage?.Invoke(message);
            }
        }

        public static void OutStackTrace()
        {
            StackTrace stackTrace = new StackTrace();
            StringBuilder sb = new StringBuilder("OutStackTrace: ");
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                sb.Append(
                    $"\n[{i}] {frame.GetMethod().DeclaringType.FullName}: {frame.GetMethod().Name}"
                );
            }
            Logger.OutMessage(sb.ToString());
        }
    }
}
