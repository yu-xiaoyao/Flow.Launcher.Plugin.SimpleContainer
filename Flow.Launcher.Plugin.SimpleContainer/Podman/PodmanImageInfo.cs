using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.SimpleContainer.Podman;

public class PodmanImageInfo
{
    [JsonPropertyName("repository")] public string Repository { get; set; }

    [JsonPropertyName("tag")] public string Tag { get; set; }

    [JsonPropertyName("Id")] public string Id { get; set; }

    [JsonPropertyName("ParentId")] public string ParentId { get; set; }

    [JsonPropertyName("RepoTags")] public List<string> RepoTags { get; set; }

    [JsonPropertyName("RepoDigests")] public List<string> RepoDigests { get; set; }

    [JsonPropertyName("Created")] public long Created { get; set; }

    [JsonPropertyName("Size")] public long? Size { get; set; }

    [JsonPropertyName("SharedSize")] public long SharedSize { get; set; }

    [JsonPropertyName("VirtualSize")] public long VirtualSize { get; set; }

    [JsonPropertyName("Labels")] public Dictionary<string, string> Labels { get; set; }

    [JsonPropertyName("Containers")] public int Containers { get; set; }

    [JsonPropertyName("Arch")] public string Arch { get; set; }

    [JsonPropertyName("Digest")] public string Digest { get; set; }

    [JsonPropertyName("History")] public List<string> History { get; set; }

    [JsonPropertyName("IsManifestList")] public bool IsManifestList { get; set; }

    [JsonPropertyName("Names")] public List<string> Names { get; set; }

    [JsonPropertyName("Os")] public string Os { get; set; }
}