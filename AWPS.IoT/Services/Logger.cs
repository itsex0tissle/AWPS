using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace AWPS.IoT.Services
{
    public static class Logger
    {
        private static ArrayList Prefixes { get; } = new();

        [Conditional("DEBUG")]
        public static void AddPrefix(string prefix)
        {
            Prefixes.Add(prefix);
        }

        [Conditional("DEBUG")]
        public static void RemovePrefix(string prefix)
        {
            Prefixes.Remove(prefix);
        }

        [Conditional("DEBUG")]
        public static void RemoveLastPrefix()
        {
            if(Prefixes.Count is 0)
            {
                return;
            }
            Prefixes.RemoveAt(Prefixes.Count - 1);
        }

        [Conditional("DEBUG")]
        public static void LogMessage(string message_type, string message)
        {
            StringBuilder builder = new();
            builder.Append('[');
            builder.Append(DateTime.UtcNow.ToString());
            builder.Append("][");
            builder.Append(message_type);
            builder.Append(']');
            foreach(string prefix in Prefixes)
            {
                builder.Append('[');
                builder.Append(prefix);
                builder.Append(']');
            }
            builder.Append(": ");
            builder.AppendLine(message);
            Debug.WriteLine(builder.ToString());
        }

        [Conditional("DEBUG")]
        public static void LogInfo(string message)
        {
            LogMessage("Info", message);
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string message)
        {
            LogMessage("Warning", message);
        }

        [Conditional("DEBUG")]
        public static void LogError(string message)
        {
            LogMessage("Error", message);
        }

        [Conditional("DEBUG")]
        public static void LogException(Exception exception)
        {
            LogMessage("Exception", exception.ToString());
        }
    }
}