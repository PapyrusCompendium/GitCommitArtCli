using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GitCommitArtCli.Services {
    [Flags]
    public enum LogLevel {
        Debugging = 1,
        Errors = 2,
        Warnings = 4,
        Info = 8
    }

    public static class Log {
        public static LogLevel Level { get; set; } = LogLevel.Info | LogLevel.Errors | LogLevel.Warnings | LogLevel.Debugging;
        private static readonly Semaphore _semaphore = new(1, 1);

        private static string TimeStamp {
            get {
                return DateTime.Now.ToLongTimeString();
            }
        }

        public static void Info(string info,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Info)) {
                LogOutput(ConsoleColor.Green, string.Join("\n", info), classFile, lineNumber, callerName);
            }
        }

        public static void Error(string error,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Errors)) {
                LogOutput(ConsoleColor.Red, string.Join("\n", error), classFile, lineNumber, callerName);
            }
        }

        public static void Warning(string warning,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Warnings)) {
                LogOutput(ConsoleColor.Yellow, string.Join("\n", warning), classFile, lineNumber, callerName);
            }
        }

        public static void Debug(string debug,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Debugging)) {
                LogOutput(ConsoleColor.Magenta, string.Join("\n", debug), classFile, lineNumber, callerName);
            }
        }

        private static void LogOutput(ConsoleColor color, string log, string classFile, int lineNumber, string callerName) {
            _semaphore.WaitOne();
            var className = Path.GetFileNameWithoutExtension(classFile);
            var logPreamble = $"[{TimeStamp}][{className}::{callerName};{lineNumber}]:\n";

            Console.ForegroundColor = color;
            Console.Write(logPreamble);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(log);
            File.WriteAllText("GitCommitArt.log", log);
            _semaphore.Release();
        }
    }
}
