using Flow.Launcher.Plugin.SimpleContainer.Common;

namespace Flow.Launcher.Plugin.SimpleContainer;

public class Settings : BaseModel
{
    public string ExportDirectory { get; set; }

    public ContainerType ContainerType { get; set; }

    public string ContainerExecutePath { get; set; }

    public string ShellPath { get; set; }

    public Settings()
    {
        ContainerType = ContainerType.Docker;
    }
}