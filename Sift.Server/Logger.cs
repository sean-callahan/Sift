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
        }

        private static readonly IReadOnlyDictionary<Level, string> levelPrefix = new Dictionary<Level, string>()
        {
            { Level.Info, "Info" },
            { Level.Warning, "Warning" },
            { Level.Error, "Error" },
            { Level.Fatal, "Fatal" },
        };

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
