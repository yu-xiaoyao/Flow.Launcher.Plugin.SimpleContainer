using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Model;

namespace Flow.Launcher.Plugin.SimpleContainer.Model;

public class ContainerDetailInfo
{
    [JsonPropertyName("Id")] public string Id { get; set; }

    [JsonPropertyName("Created")] public DateTime Created { get; set; }

    [JsonPropertyName("Path")] public string Path { get; set; }

    [JsonPropertyName("Args")] public List<string> Args { get; set; }

    [JsonPropertyName("State")] public State State { get; set; }

    [JsonPropertyName("Image")] public string Image { get; set; }

    [JsonPropertyName("ResolvConfPath")] public string ResolvConfPath { get; set; }

    [JsonPropertyName("HostnamePath")] public string HostnamePath { get; set; }

    [JsonPropertyName("HostsPath")] public string HostsPath { get; set; }

    [JsonPropertyName("LogPath")] public string LogPath { get; set; }

    [JsonPropertyName("Name")] public string Name { get; set; }

    [JsonPropertyName("RestartCount")] public int RestartCount { get; set; }

    [JsonPropertyName("Driver")] public string Driver { get; set; }

    [JsonPropertyName("Platform")] public string Platform { get; set; }

    [JsonPropertyName("MountLabel")] public string MountLabel { get; set; }

    [JsonPropertyName("ProcessLabel")] public string ProcessLabel { get; set; }

    [JsonPropertyName("AppArmorProfile")] public string AppArmorProfile { get; set; }

    [JsonPropertyName("ExecIDs")] public List<string> ExecIDs { get; set; }

    [JsonPropertyName("HostConfig")] public HostConfig HostConfig { get; set; }

    [JsonPropertyName("GraphDriver")] public GraphDriver GraphDriver { get; set; }

    [JsonPropertyName("Mounts")] public List<Mount> Mounts { get; set; }

    [JsonPropertyName("Config")] public Config Config { get; set; }

    [JsonPropertyName("NetworkSettings")] public NetworkSettings NetworkSettings { get; set; }
}

public class State
{
    [JsonPropertyName("Status")] public string Status { get; set; }

    [JsonPropertyName("Running")] public bool Running { get; set; }

    [JsonPropertyName("Paused")] public bool Paused { get; set; }

    [JsonPropertyName("Restarting")] public bool Restarting { get; set; }

    [JsonPropertyName("OOMKilled")] public bool OOMKilled { get; set; }

    [JsonPropertyName("Dead")] public bool Dead { get; set; }

    [JsonPropertyName("Pid")] public int Pid { get; set; }

    [JsonPropertyName("ExitCode")] public int ExitCode { get; set; }

    [JsonPropertyName("Error")] public string Error { get; set; }

    [JsonPropertyName("StartedAt")] public DateTime StartedAt { get; set; }

    [JsonPropertyName("FinishedAt")] public DateTime FinishedAt { get; set; }
}

public class LogConfig
{
    [JsonPropertyName("Type")] public string Type { get; set; }

    [JsonPropertyName("Config")] public Dictionary<string, string> Config { get; set; }
}

public class PortBinding
{
    [JsonPropertyName("HostIp")] public string HostIp { get; set; }

    [JsonPropertyName("HostPort")] public string HostPort { get; set; }
}

public class RestartPolicy
{
    [JsonPropertyName("Name")] public string Name { get; set; }

    [JsonPropertyName("MaximumRetryCount")]
    public int MaximumRetryCount { get; set; }
}

public class HostConfig
{
    [JsonPropertyName("Binds")] public List<string> Binds { get; set; }

    [JsonPropertyName("ContainerIDFile")] public string ContainerIDFile { get; set; }

    [JsonPropertyName("LogConfig")] public LogConfig LogConfig { get; set; }

    [JsonPropertyName("NetworkMode")] public string NetworkMode { get; set; }

    [JsonPropertyName("PortBindings")] public Dictionary<string, List<PortBinding>> PortBindings { get; set; }

    [JsonPropertyName("RestartPolicy")] public RestartPolicy RestartPolicy { get; set; }

    [JsonPropertyName("AutoRemove")] public bool AutoRemove { get; set; }

    [JsonPropertyName("VolumeDriver")] public string VolumeDriver { get; set; }

    [JsonPropertyName("VolumesFrom")] public List<string> VolumesFrom { get; set; }

    [JsonPropertyName("ConsoleSize")] public List<int> ConsoleSize { get; set; }

    [JsonPropertyName("CapAdd")] public List<string> CapAdd { get; set; }

    [JsonPropertyName("CapDrop")] public List<string> CapDrop { get; set; }

    [JsonPropertyName("CgroupnsMode")] public string CgroupnsMode { get; set; }

    [JsonPropertyName("Dns")] public List<string> Dns { get; set; }

    [JsonPropertyName("DnsOptions")] public List<string> DnsOptions { get; set; }

    [JsonPropertyName("DnsSearch")] public List<string> DnsSearch { get; set; }

    [JsonPropertyName("ExtraHosts")] public List<string> ExtraHosts { get; set; }

    [JsonPropertyName("GroupAdd")] public List<string> GroupAdd { get; set; }

    [JsonPropertyName("IpcMode")] public string IpcMode { get; set; }

    [JsonPropertyName("Cgroup")] public string Cgroup { get; set; }

    [JsonPropertyName("Links")] public List<string> Links { get; set; }

    [JsonPropertyName("OomScoreAdj")] public int OomScoreAdj { get; set; }

    [JsonPropertyName("PidMode")] public string PidMode { get; set; }

    [JsonPropertyName("Privileged")] public bool Privileged { get; set; }

    [JsonPropertyName("PublishAllPorts")] public bool PublishAllPorts { get; set; }

    [JsonPropertyName("ReadonlyRootfs")] public bool ReadonlyRootfs { get; set; }

    [JsonPropertyName("SecurityOpt")] public List<string> SecurityOpt { get; set; }

    [JsonPropertyName("UTSMode")] public string UTSMode { get; set; }

    [JsonPropertyName("UsernsMode")] public string UsernsMode { get; set; }

    [JsonPropertyName("ShmSize")] public long ShmSize { get; set; }

    [JsonPropertyName("Runtime")] public string Runtime { get; set; }

    [JsonPropertyName("Isolation")] public string Isolation { get; set; }

    [JsonPropertyName("CpuShares")] public int CpuShares { get; set; }

    [JsonPropertyName("Memory")] public long Memory { get; set; }

    [JsonPropertyName("NanoCpus")] public long NanoCpus { get; set; }

    [JsonPropertyName("CgroupParent")] public string CgroupParent { get; set; }

    [JsonPropertyName("BlkioWeight")] public int BlkioWeight { get; set; }

    [JsonPropertyName("BlkioWeightDevice")]
    public List<object> BlkioWeightDevice { get; set; }

    [JsonPropertyName("BlkioDeviceReadBps")]
    public List<object> BlkioDeviceReadBps { get; set; }

    [JsonPropertyName("BlkioDeviceWriteBps")]
    public List<object> BlkioDeviceWriteBps { get; set; }

    [JsonPropertyName("BlkioDeviceReadIOps")]
    public List<object> BlkioDeviceReadIOps { get; set; }

    [JsonPropertyName("BlkioDeviceWriteIOps")]
    public List<object> BlkioDeviceWriteIOps { get; set; }

    [JsonPropertyName("CpuPeriod")] public int CpuPeriod { get; set; }

    [JsonPropertyName("CpuQuota")] public int CpuQuota { get; set; }

    [JsonPropertyName("CpuRealtimePeriod")]
    public int CpuRealtimePeriod { get; set; }

    [JsonPropertyName("CpuRealtimeRuntime")]
    public int CpuRealtimeRuntime { get; set; }

    [JsonPropertyName("CpusetCpus")] public string CpusetCpus { get; set; }

    [JsonPropertyName("CpusetMems")] public string CpusetMems { get; set; }

    [JsonPropertyName("Devices")] public List<object> Devices { get; set; }

    [JsonPropertyName("DeviceCgroupRules")]
    public List<string> DeviceCgroupRules { get; set; }

    [JsonPropertyName("DeviceRequests")] public List<object> DeviceRequests { get; set; }

    [JsonPropertyName("MemoryReservation")]
    public long MemoryReservation { get; set; }

    [JsonPropertyName("MemorySwap")] public long MemorySwap { get; set; }

    [JsonPropertyName("MemorySwappiness")] public long? MemorySwappiness { get; set; }

    [JsonPropertyName("OomKillDisable")] public bool OomKillDisable { get; set; }

    [JsonPropertyName("PidsLimit")] public long? PidsLimit { get; set; }

    [JsonPropertyName("Ulimits")] public List<object> Ulimits { get; set; }

    [JsonPropertyName("CpuCount")] public int CpuCount { get; set; }

    [JsonPropertyName("CpuPercent")] public int CpuPercent { get; set; }

    [JsonPropertyName("IOMaximumIOps")] public int IOMaximumIOps { get; set; }

    [JsonPropertyName("IOMaximumBandwidth")]
    public int IOMaximumBandwidth { get; set; }

    [JsonPropertyName("MaskedPaths")] public List<string> MaskedPaths { get; set; }

    [JsonPropertyName("ReadonlyPaths")] public List<string> ReadonlyPaths { get; set; }
}

public class Mount
{
    [JsonPropertyName("Type")] public string Type { get; set; }

    [JsonPropertyName("Source")] public string Source { get; set; }

    [JsonPropertyName("Destination")] public string Destination { get; set; }

    [JsonPropertyName("Mode")] public string Mode { get; set; }

    [JsonPropertyName("RW")] public bool RW { get; set; }

    [JsonPropertyName("Propagation")] public string Propagation { get; set; }
}

public class Config
{
    [JsonPropertyName("Hostname")] public string Hostname { get; set; }

    [JsonPropertyName("Domainname")] public string Domainname { get; set; }

    [JsonPropertyName("User")] public string User { get; set; }

    [JsonPropertyName("AttachStdin")] public bool AttachStdin { get; set; }

    [JsonPropertyName("AttachStdout")] public bool AttachStdout { get; set; }

    [JsonPropertyName("AttachStderr")] public bool AttachStderr { get; set; }

    [JsonPropertyName("ExposedPorts")] public Dictionary<string, object> ExposedPorts { get; set; }

    [JsonPropertyName("Tty")] public bool Tty { get; set; }

    [JsonPropertyName("OpenStdin")] public bool OpenStdin { get; set; }

    [JsonPropertyName("StdinOnce")] public bool StdinOnce { get; set; }

    [JsonPropertyName("Env")] public List<string> Env { get; set; }

    [JsonPropertyName("Cmd")] public List<string> Cmd { get; set; }

    [JsonPropertyName("Image")] public string Image { get; set; }

    [JsonPropertyName("Volumes")] public Dictionary<string, object> Volumes { get; set; }

    [JsonPropertyName("WorkingDir")] public string WorkingDir { get; set; }

    [JsonPropertyName("Entrypoint")] public List<string> Entrypoint { get; set; }

    [JsonPropertyName("OnBuild")] public List<string> OnBuild { get; set; }

    [JsonPropertyName("Labels")] public Dictionary<string, string> Labels { get; set; }
}

public class NetworkSettings
{
    [JsonPropertyName("Bridge")] public string Bridge { get; set; }

    [JsonPropertyName("SandboxID")] public string SandboxID { get; set; }

    [JsonPropertyName("SandboxKey")] public string SandboxKey { get; set; }

    [JsonPropertyName("Ports")] public Dictionary<string, List<PortBinding>> Ports { get; set; }

    [JsonPropertyName("HairpinMode")] public bool HairpinMode { get; set; }

    [JsonPropertyName("LinkLocalIPv6Address")]
    public string LinkLocalIPv6Address { get; set; }

    [JsonPropertyName("LinkLocalIPv6PrefixLen")]
    public int LinkLocalIPv6PrefixLen { get; set; }

    [JsonPropertyName("SecondaryIPAddresses")]
    public List<string> SecondaryIPAddresses { get; set; }

    [JsonPropertyName("SecondaryIPv6Addresses")]
    public List<string> SecondaryIPv6Addresses { get; set; }

    [JsonPropertyName("EndpointID")] public string EndpointID { get; set; }

    [JsonPropertyName("Gateway")] public string Gateway { get; set; }

    [JsonPropertyName("GlobalIPv6Address")]
    public string GlobalIPv6Address { get; set; }

    [JsonPropertyName("GlobalIPv6PrefixLen")]
    public int GlobalIPv6PrefixLen { get; set; }

    [JsonPropertyName("IPAddress")] public string IPAddress { get; set; }

    [JsonPropertyName("IPPrefixLen")] public int IPPrefixLen { get; set; }

    [JsonPropertyName("IPv6Gateway")] public string IPv6Gateway { get; set; }

    [JsonPropertyName("MacAddress")] public string MacAddress { get; set; }

    [JsonPropertyName("Networks")] public Dictionary<string, Network> Networks { get; set; }
}

public class Network
{
    [JsonPropertyName("IPAMConfig")] public object IPAMConfig { get; set; }

    [JsonPropertyName("Links")] public List<string> Links { get; set; }

    [JsonPropertyName("Aliases")] public List<string> Aliases { get; set; }

    [JsonPropertyName("MacAddress")] public string MacAddress { get; set; }

    [JsonPropertyName("DriverOpts")] public Dictionary<string, string> DriverOpts { get; set; }

    [JsonPropertyName("NetworkID")] public string NetworkID { get; set; }

    [JsonPropertyName("EndpointID")] public string EndpointID { get; set; }

    [JsonPropertyName("Gateway")] public string Gateway { get; set; }

    [JsonPropertyName("IPAddress")] public string IPAddress { get; set; }

    [JsonPropertyName("IPPrefixLen")] public int IPPrefixLen { get; set; }

    [JsonPropertyName("IPv6Gateway")] public string IPv6Gateway { get; set; }

    [JsonPropertyName("GlobalIPv6Address")]
    public string GlobalIPv6Address { get; set; }

    [JsonPropertyName("GlobalIPv6PrefixLen")]
    public int GlobalIPv6PrefixLen { get; set; }

    [JsonPropertyName("DNSNames")] public List<string> DNSNames { get; set; }
}