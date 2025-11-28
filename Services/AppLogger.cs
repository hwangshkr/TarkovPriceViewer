using System;
using System.Diagnostics;

namespace TarkovPriceViewer.Services
{
    public static class AppLogger
    {
        private static string Format(string level, string source, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return $"[{timestamp}][{level}][{source}] {message}";
        }

        public static void Info(string source, string message)
        {
            Debug.WriteLine(Format("INFO", source, message));
        }

        public static void Warn(string source, string message)
        {
            Debug.WriteLine(Format("WARN", source, message));
        }

        public static void Error(string source, string message)
        {
            Debug.WriteLine(Format("ERROR", source, message));
        }

        public static void Error(string source, string message, Exception ex)
        {
            Debug.WriteLine(Format("ERROR", source, $"{message} Exception: {ex?.Message}"));
        }
    }
}
