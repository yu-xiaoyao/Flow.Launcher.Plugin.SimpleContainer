namespace Flow.Launcher.Plugin.SimpleContainer.Docker;

public class DockerImageInfo
{
    public string ID { set; get; }
    public string Repository { set; get; }
    public string Digest { set; get; }
    public string Containers { set; get; }
    public string CreatedAt { set; get; }
    public string CreatedSince { set; get; }
    public string SharedSize { set; get; }
    public string Size { set; get; }
    public string Tag { set; get; }
    public string UniqueSize { set; get; }
    public string VirtualSize { set; get; }
}