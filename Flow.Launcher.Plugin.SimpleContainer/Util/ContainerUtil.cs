using System;

namespace Flow.Launcher.Plugin.SimpleContainer.Util;

public class ContainerUtil
{
    public static bool IsRunning(string state)
    {
        return "running".Equals(state, StringComparison.OrdinalIgnoreCase);
    }
}