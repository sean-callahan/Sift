using System;
using System.Collections.Generic;
using Sift.Common;

namespace Sift.Server
{
    // TODO: save to file
    internal static class Logger
    {
        private const int MaxRecursiveExceptions = 10;

        public enum Level
        {
            Info,
            Warning,
            Error,
            Fatal,
            Debug,
        }

        private static readonly IReadOnlyDictionary<Level, string> levelPrefix = new Dictionary<Level, string>()
        {
            { Level.Info, "Info" },
            { Level.Warning, "Warning" },
            { Level.Error, "Error" },
            { Level.Fatal, "Fatal" },
            { Level.Debug, "Debug" },
        };

        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugLog(string msg)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] [" + levelPrefix[Level.Debug] + "] " + msg);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugLog(Exception ex, int i = 0)
        {
            Log($"---\n{ex.Message}\n{ex.StackTrace}", Level.Debug);
            if (ex.InnerException != null && i < MaxRecursiveExceptions)
                Log(ex, Level.Debug, i + 1);
        }

        public static void Log(string msg, Level level = Level.Info)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] [" + levelPrefix[level] + "] " + msg);
        }

        public static void Log(Caller c, string message, Level level = Level.Info) => Log($"[{c.Number}] " + message, level);

        public static void Log(Exception ex, Level level = Level.Error, int i = 0)
        {
            Log($"---\n{ex.Message}\n{ex.StackTrace}", level);
            if (ex.InnerException != null && i < MaxRecursiveExceptions)
                Log(ex, level, i + 1);
        }
    }
}
