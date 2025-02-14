using System;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Docker;
using Flow.Launcher.Plugin.SimpleContainer.Podman;

namespace Flow.Launcher.Plugin.SimpleContainer.Model;

public class ImageInfo
{
    public ContainerType ContainerType { get; set; }

    /// <summary>
    /// Image Id
    /// </summary>
    public string Id { set; get; }

    public string Repository { set; get; }
    public string Tag { set; get; }

    /// <summary>
    /// 转换成字符串
    /// </summary>
    public string Size { set; get; }

    public string Digest { set; get; }

    public string Containers { set; get; }

    public DateTime CreatedAt { set; get; }

    public object RawInfo { get; set; }

    public string GetShortId()
    {
        return Id.Length > 12 ? Id[..12] : Id;
    }
}