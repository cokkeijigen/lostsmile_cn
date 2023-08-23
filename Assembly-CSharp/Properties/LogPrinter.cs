using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text; 

// iTsukezigen, 输出日志用
public class LogPrinter
{
    static string CurrentDir = Directory.GetCurrentDirectory();

    static string Debug = Path.Combine(CurrentDir, "DEBUG");

    public static void Puts(string content, string description = "All")
    {
        if (!Directory.Exists(Debug)) return;
        string filePath = Path.Combine(CurrentDir, "DEBUG", $"LogPrinter_{description}.txt");
        if (File.Exists(filePath) && ((new FileInfo(filePath).Length/1024) > 100))
        {
            File.WriteAllText(filePath, string.Empty);
        }
        File.AppendAllText(filePath, content + "\n");
    }

    public static void OutStackTrace(string description = "OutStackTrace")
    {
        StackTrace stackTrace = new StackTrace();
        StringBuilder sb = new StringBuilder();
        sb.Append(description + ": ");
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            StackFrame frame = stackTrace.GetFrame(i); 
            sb.Append(
                "\n[" + i + "] " + frame.GetMethod().DeclaringType.FullName + ": " + frame.GetMethod().Name
            ); 
        }
        LogPrinter.Puts(sb.ToString()); 
    }


    public static void Dump(object obj, string name)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Path.Combine(Debug, name), FileMode.Create);
            formatter.Serialize(stream, obj);
            stream.Close();
        }
        catch (Exception e)
        {
            LogPrinter.Puts($"[BootInitScenariodData] {e.Message}", "err");
        }
    }
}
