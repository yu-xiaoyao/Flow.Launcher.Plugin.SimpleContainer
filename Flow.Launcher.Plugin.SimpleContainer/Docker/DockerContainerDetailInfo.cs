using System.Collections.Generic;

namespace Flow.Launcher.Plugin.SimpleContainer.Docker;

public class DockerContainerDetailInfo
{
    public string Id { get; set; }
    public string Created { get; set; }
    public string Path { get; set; }
    public List<string> Args { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    
}