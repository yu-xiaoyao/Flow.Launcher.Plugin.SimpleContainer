using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.SimpleContainer.Podman;

public class PodmanContainerInfo
{
    [JsonPropertyName("AutoRemove")] public bool AutoRemove { get; set; }

    [JsonPropertyName("Command")] public List<string> Command { get; set; }

    [JsonPropertyName("Created")] public string Created { get; set; }

    [JsonPropertyName("CreatedAt")] public string CreatedAt { get; set; }

    [JsonPropertyName("CIDFile")] public string cidFile { get; set; }

    [JsonPropertyName("Exited")] public bool Exited { get; set; }

    [JsonPropertyName("ExitedAt")] public long ExitedAt { get; set; }

    [JsonPropertyName("ExitCode")] public int ExitCode { get; set; }

    [JsonPropertyName("ExposedPorts")] public Dictionary<string, List<string>> ExposedPorts { get; set; }

    [JsonPropertyName("Id")] public string Id { get; set; }

    [JsonPropertyName("Image")] public string Image { get; set; }

    [JsonPropertyName("ImageID")] public string ImageId { get; set; }

    [JsonPropertyName("IsInfra")] public bool IsInfra { get; set; }

    [JsonPropertyName("Labels")] public Dictionary<string, string> Labels { get; set; }

    [JsonPropertyName("Mounts")] public List<string> Mounts { get; set; }

    [JsonPropertyName("Names")] public List<string> Names { get; set; }

    // TODO
    // [JsonPropertyName("Namespaces")] public Dictionary<string, string> Namespaces { get; set; }

    [JsonPropertyName("Networks")] public List<string> Networks { get; set; }

    [JsonPropertyName("Pid")] public int Pid { get; set; }

    [JsonPropertyName("Pod")] public string Pod { get; set; }

    [JsonPropertyName("PodName")] public string PodName { get; set; }

    [JsonPropertyName("Ports")] public List<PortMapping> Ports { get; set; }

    [JsonPropertyName("Restarts")] public int Restarts { get; set; }

    [JsonPropertyName("Size")] public long? Size { get; set; }

    [JsonPropertyName("StartedAt")] public long StartedAt { get; set; }

    [JsonPropertyName("State")] public string State { get; set; }

    [JsonPropertyName("Status")] public string Status { get; set; }
}

public class PortMapping
{
    [JsonPropertyName("host_ip")] public string HostIp { get; set; }

    [JsonPropertyName("container_port")] public int ContainerPort { get; set; }

    [JsonPropertyName("host_port")] public int HostPort { get; set; }

    [JsonPropertyName("range")] public int Range { get; set; }

    [JsonPropertyName("protocol")] public string Protocol { get; set; }
}