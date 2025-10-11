using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// iTsukezigen++
namespace CHSPatch
{
    public class Logger
    {
#if DEBUG
        [DllImport("LOSTSMILE_CN.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void DebugMessage(string message);

        public static void OutMessage(string message, bool breakline = true)
        {
            try
            {
                if (breakline) message += '\n';
                DebugMessage(message);
            }
            catch (Exception) {
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
#else
    public static void OutMessage(string message, bool breakline = true)
    {
    }

    public static void OutStackTrace()
    {
    }
#endif
    }
}
