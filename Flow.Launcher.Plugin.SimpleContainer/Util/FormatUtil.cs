using System;
using System.Globalization;

namespace Flow.Launcher.Plugin.SimpleContainer.Util;

public class FormatUtil
{
    private static readonly string[] units = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string FormatFileSize(long? bytes)
    {
        if (bytes == null) return "N/A";

        var unitIndex = 0;
        var value = bytes.Value;
        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        return $"{value:0.##} {units[unitIndex]}";
    }

    public static DateTime FormatTimeStamp(long milliseconds)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
    }


    /// <summary>
    /// format 
    /// </summary>
    /// <param name="dateTime">2024-08-14 00:55:20 +0800 CST</param>
    /// <returns></returns>
    public static DateTime FormatDateTime(string dateTime)
    {
        // 2023-12-31 21:32:23 +0800 CST
        return DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss zzzz 'CST'", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// format
    /// </summary>
    /// <param name="dateTime">2025-01-14T12:37:45.274282993+08:00</param>
    /// <returns></returns>
    public static DateTime FormatDateTimeISO(string dateTime)
    {
        return DateTime.Parse(dateTime, null, DateTimeStyles.RoundtripKind);
    }
}