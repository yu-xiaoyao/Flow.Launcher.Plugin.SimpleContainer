using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.SimpleContainer.Model;

public class CommonInfo
{
}

public class GraphDriver
{
    [JsonPropertyName("Data")] public Dictionary<string, string> Data { get; set; }

    [JsonPropertyName("Name")] public string Name { get; set; }
}

public class RootFS
{
    [JsonPropertyName("Type")] public string Type { get; set; }

    [JsonPropertyName("Layers")] public List<string> Layers { get; set; }
}