namespace Flow.Launcher.Plugin.SimpleContainer.Docker;

public class DockerContainerInfo
{
    public string Command { set; get; }
    public string CreatedAt { set; get; }
    public string ID { set; get; }
    public string Image { set; get; }
    public string Labels { set; get; }
    public string LocalVolumes { set; get; }
    public string Mounts { set; get; }
    public string Names { set; get; }
    public string Networks { set; get; }
    public string Ports { set; get; }
    public string RunningFor { set; get; }
    public string Size { set; get; }
    public string State { set; get; }
    public string Status { set; get; }
}