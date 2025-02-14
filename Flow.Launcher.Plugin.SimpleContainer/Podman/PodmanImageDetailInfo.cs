using System.Collections.Generic;
using System.Text.Json.Serialization;
using Flow.Launcher.Plugin.SimpleContainer.Model;

namespace Flow.Launcher.Plugin.SimpleContainer.Podman;

public class PodmanImageDetailInfo
{
    public string Id { get; set; }
    public string Digest { get; set; }
    public List<string> RepoTags { get; set; }
    public List<string> RepoDigests { get; set; }
    public string Parent { get; set; }
    public string Comment { get; set; }
    public string Created { get; set; }
    public ImageConfig Config { get; set; }
    public string Version { get; set; }
    public string Author { get; set; }
    public string Architecture { get; set; }
    public string Os { get; set; }
    public long Size { get; set; }
    public long VirtualSize { get; set; }
    public GraphDriver GraphDriver { get; set; }
    public RootFS RootFS { get; set; }
    public Dictionary<string, string> Labels { get; set; }
    public Dictionary<string, string> Annotations { get; set; }
    public string ManifestType { get; set; }
    public string User { get; set; }
    public List<History> History { get; set; }
    public List<string> NamesHistory { get; set; }
}

public class ImageConfig
{
    public List<string> Env { get; set; }
    public List<string> Cmd { get; set; }
    public string WorkingDir { get; set; }
    public Dictionary<string, string> Labels { get; set; }
    public bool ArgsEscaped { get; set; }
}

public class History
{
    [JsonPropertyName("created")] public string Created { get; set; }
    [JsonPropertyName("created_by")] public string CreatedBy { get; set; }
    [JsonPropertyName("empty_layer")] public bool EmptyLayer { get; set; }
    [JsonPropertyName("comment")] public string Comment { get; set; }
}