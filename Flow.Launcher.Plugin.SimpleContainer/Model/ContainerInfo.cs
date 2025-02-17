using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Docker;
using Flow.Launcher.Plugin.SimpleContainer.Podman;

namespace Flow.Launcher.Plugin.SimpleContainer.Model;

public class ContainerInfo
{
    public ContainerType ContainerType { get; set; }

    public string Command { set; get; }
    public DateTime CreatedAt { set; get; }
    public string Id { set; get; }
    public string Image { set; get; }
    public Dictionary<string, string> Labels { get; set; }
    public List<string> Mounts { get; set; }
    public List<string> Names { get; set; }
    public List<string> Networks { get; set; }

    public string State { get; set; }

    public string Status { get; set; }

    public List<ContainerPortMapping> Ports { get; set; }

    public object RawInfo { get; set; }

    public string GetShortId()
    {
        return Id.Length > 12 ? Id[..12] : Id;
    }

    public string GetName()
    {
        return Names[0];
    }

    public string GetNetwork()
    {
        return Networks[0];
    }
}

public class ContainerPortMapping
{
    public string HostIp { get; set; }
    public int HostPort { get; set; }
    public int ContainerPort { get; set; }
    public string Protocol { get; set; }
    
}