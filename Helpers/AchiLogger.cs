using Godot;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AchiQuiz.Helpers
{
    public static class AchiLogger
    {
        public static bool PrintViaDebugger { get; set; } = false;

        private static void Log(string message, string file = "", int lineNumber = 0)
        {
            file = file.Replace(".cs", "");
            var timestamp = DateTime.Now.ToString("HH:mm:ss");

            message = message.TrimStart('"');
            message = message.TrimEnd('"');

            GD.Print($"[{timestamp} {file}:{lineNumber}] {message}");

            if (PrintViaDebugger)
            {
                Debugger.Log(0, $"[{file}:{lineNumber}", $"{timestamp}] {message}");
            }
        }

        public static void Log(object @object, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var @string = JsonConvert.SerializeObject(@object);
            var file = Path.GetFileName(filePath);

            Log(@string, file, lineNumber);
        }
    }
}
