using EnhancedItemInfo.Core;
using System;
using System.Text;

namespace EnhancedItemInfo.Utils;

public static class Logger {
    public enum LogLevel {
        None,
        DEBUG,
        INFO,
        WARN,
        ERROR,
    }

#if DEBUG
    public static LogLevel CurrentLevel = LogLevel.DEBUG;
#else
    public static LogLevel CurrentLevel = LogLevel.INFO;
#endif

    public static string Format(LogLevel level, string message, Exception? ex = null) {
        StringBuilder stringBuilder = new();
        stringBuilder.Append("[");
        stringBuilder.Append(VersionInfo.Name);
        stringBuilder.Append("][");
        stringBuilder.Append(level);
        stringBuilder.Append(']');
        stringBuilder.Append(message);
        if (ex != null) {
            stringBuilder.Append("\n");
            stringBuilder.Append(ex);
        }
        return stringBuilder.ToString();
    }
    public static void Log(LogLevel level, string message, Exception? ex = null) {
        if (level < CurrentLevel) {
            return;
        }
        message = Format(level, message, ex);
        switch (level) {
            case LogLevel.DEBUG:
            case LogLevel.INFO:
                UnityEngine.Debug.Log(message);
                break;
            case LogLevel.WARN:
                UnityEngine.Debug.LogWarning(message);
                break;
            case LogLevel.ERROR:
                UnityEngine.Debug.LogError(message);
                break;
        }
    }
    public static void Debug(string message) => Log(LogLevel.DEBUG, message);
    public static void Info(string message) => Log(LogLevel.INFO, message);
    public static void Warn(string message) => Log(LogLevel.WARN, message);
    public static void Error(string message, Exception? ex = null) => Log(LogLevel.ERROR, message, ex);
}
