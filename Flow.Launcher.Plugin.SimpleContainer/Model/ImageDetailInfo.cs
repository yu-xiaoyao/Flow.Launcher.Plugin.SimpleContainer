using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.SimpleContainer.Model;

public class ImageDetailInfo
{
    public string Id { get; set; }
    public List<string> RepoTags { get; set; }
    public List<string> RepoDigests { get; set; }
    public string Parent { get; set; }
    public string Comment { get; set; }
    public string Created { get; set; }
    public string Version { get; set; }
    public string Author { get; set; }
    public string Architecture { get; set; }
    public string Os { get; set; }
    public long Size { get; set; }
}

