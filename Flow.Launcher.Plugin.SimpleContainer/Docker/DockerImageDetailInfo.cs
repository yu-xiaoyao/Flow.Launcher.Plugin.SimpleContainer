using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Flow.Launcher.Plugin.SimpleContainer.Model;

namespace Flow.Launcher.Plugin.SimpleContainer.Docker;

public class DockerImageDetailInfo
{
    [JsonPropertyName("Id")] public string Id { get; set; }

    [JsonPropertyName("RepoTags")] public List<string> RepoTags { get; set; }

    [JsonPropertyName("RepoDigests")] public List<string> RepoDigests { get; set; }

    [JsonPropertyName("Parent")] public string Parent { get; set; }

    [JsonPropertyName("Comment")] public string Comment { get; set; }

    [JsonPropertyName("Created")] public DateTime Created { get; set; }

    [JsonPropertyName("DockerVersion")] public string Version { get; set; }

    [JsonPropertyName("Author")] public string Author { get; set; }

    [JsonPropertyName("Config")] public ImageConfig Config { get; set; }

    [JsonPropertyName("Architecture")] public string Architecture { get; set; }

    [JsonPropertyName("Os")] public string Os { get; set; }

    [JsonPropertyName("Size")] public long Size { get; set; }

    [JsonPropertyName("GraphDriver")] public GraphDriver GraphDriver { get; set; }

    [JsonPropertyName("RootFS")] public RootFS RootFS { get; set; }

    [JsonPropertyName("Metadata")] public Dictionary<string, string> Metadata { get; set; }
}

public class ImageConfig
{
    [JsonPropertyName("Hostname")] public string Hostname { get; set; }

    [JsonPropertyName("Domainname")] public string Domainname { get; set; }

    [JsonPropertyName("User")] public string User { get; set; }

    [JsonPropertyName("AttachStdin")] public bool AttachStdin { get; set; }

    [JsonPropertyName("AttachStdout")] public bool AttachStdout { get; set; }

    [JsonPropertyName("AttachStderr")] public bool AttachStderr { get; set; }

    // TODO 未知
    [JsonPropertyName("ExposedPorts")] public Dictionary<string, object> ExposedPorts { get; set; }

    [JsonPropertyName("Tty")] public bool Tty { get; set; }

    [JsonPropertyName("OpenStdin")] public bool OpenStdin { get; set; }

    [JsonPropertyName("StdinOnce")] public bool StdinOnce { get; set; }

    [JsonPropertyName("Env")] public List<string> Env { get; set; }

    //TODO 未知
    [JsonPropertyName("Cmd")] public List<string> Cmd { get; set; }

    [JsonPropertyName("Healthcheck")] public Healthcheck Healthcheck { get; set; }

    [JsonPropertyName("Image")] public string Image { get; set; }

    //TODO 未知
    [JsonPropertyName("Volumes")] public Dictionary<string, object> Volumes { get; set; }

    [JsonPropertyName("WorkingDir")] public string WorkingDir { get; set; }

    [JsonPropertyName("Entrypoint")] public List<string> Entrypoint { get; set; }

    // [JsonPropertyName("OnBuild")] public List<string> OnBuild { get; set; }

    [JsonPropertyName("Labels")] public Dictionary<string, string> Labels { get; set; }
}

public class Healthcheck
{
    [JsonPropertyName("Test")] public List<string> Test { get; set; }

    [JsonPropertyName("Interval")] public long Interval { get; set; }

    [JsonPropertyName("Timeout")] public long Timeout { get; set; }
}