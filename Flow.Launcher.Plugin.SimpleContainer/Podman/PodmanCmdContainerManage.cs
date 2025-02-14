using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Flow.Launcher.Plugin.SimpleContainer.Command;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Model;
using Flow.Launcher.Plugin.SimpleContainer.Util;

namespace Flow.Launcher.Plugin.SimpleContainer.Podman;

/// <summary>
/// Podman 容器
/// </summary>
public class PodmanCmdContainerManage : IContainerManage
{
    private const string CommandRootJsonFormat = " --format \"{{json .}}\"";

    private bool _isInit = false;
    private readonly string _executorPath;
    private int Timeout = 10 * 1000;


    public PodmanCmdContainerManage() : this("podman")
    {
    }

    public PodmanCmdContainerManage(string podmanPath)
    {
        _executorPath = string.IsNullOrEmpty(podmanPath) ? "podman" : podmanPath;
    }


    public ContainerType GetContainerType()
    {
        return ContainerType.Podman;
    }

    public string GetContainerId()
    {
        return "podman";
    }

    public bool IsRunning()
    {
        var result = CommandUtil.ExecuteWithArgs(_executorPath, "version --format {{.Server.Version}}", Timeout);
        return result.Success;
    }


    public Tuple<bool, string> GetVersion()
    {
        var result = CommandUtil.ExecuteWithArgs(_executorPath, "version --format {{.Server.Version}}", Timeout);
        return result.Success
            ? new Tuple<bool, string>(true, result.Result)
            : new Tuple<bool, string>(true, result.Message);
    }


    public ResultResponse<List<ImageInfo>> ListImages(
        bool all = false,
        bool dangling = false,
        string queryName = null,
        string queryId = null)
    {
        var arg = "";
        if (all)
        {
            arg += " -a ";
        }


        if (!dangling)
            arg += " -f \"dangling=false\"";

        if (!string.IsNullOrEmpty(queryName))
            arg += $" -f reference=*{queryName}*";

        if (!string.IsNullOrEmpty(queryId))
        {
            // podman id only support id start prefix query
            arg += $" -f id={queryId}";
        }

        var result =
            CommandUtil.ExecuteWithArgs(_executorPath, "images --no-trunc " + arg + CommandRootJsonFormat, Timeout);

        if (!result.Success)
        {
            return new ResultResponse<List<ImageInfo>>
            {
                Success = false,
                Message = result.Message
            };
        }

        var lines = result.Result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        var infos = new List<PodmanImageInfo>(lines.Length);
        foreach (var line in lines)
        {
            // Console.WriteLine(line);
            try
            {
                var info = JsonSerializer.Deserialize<PodmanImageInfo>(line);
                infos.Add(info);
            }
            catch (JsonException ex)
            {
                InnerLogger.Logger.Warn($"GetImages. JSON Format Error: {ex.Message}");
            }
        }

        // convert to infos to list
        var list = infos.Select(_convertToBaseImageInfo).ToList();

        return new ResultResponse<List<ImageInfo>>
        {
            Success = true,
            Result = list,
            Message = result.Message
        };
    }

    private static ImageInfo _convertToBaseImageInfo(PodmanImageInfo imageInfo)
    {
        return new ImageInfo
        {
            ContainerType = ContainerType.Podman,
            RawInfo = imageInfo,
            Id = imageInfo.Id,
            Repository = imageInfo.Repository,
            Tag = imageInfo.Tag,
            Size = FormatUtil.FormatFileSize(imageInfo.Size),
            Digest = imageInfo.Digest,
            Containers = $"{imageInfo.Containers}",
            CreatedAt = FormatUtil.FormatTimeStamp(imageInfo.Created * 1000)
        };
    }


    public ResultResponse<ImageDetailInfo> GetImage(string id)
    {
        var result =
            CommandUtil.ExecuteWithArgs(_executorPath, "images inspect " + id + CommandRootJsonFormat, Timeout);

        if (!result.Success)
        {
            return new ResultResponse<ImageDetailInfo>
            {
                Success = false,
                Message = result.Message
            };
        }

        try
        {
            var detail = JsonSerializer.Deserialize<PodmanImageDetailInfo>(result.Result);
            //TODO
            var info = new ImageDetailInfo
            {
            };

            return new ResultResponse<ImageDetailInfo>
            {
                Success = true,
                Result = info,
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            InnerLogger.Logger.Warn($"GetImage. id = {id}. JSON Format Error: {ex.Message}");
            return new ResultResponse<ImageDetailInfo>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public ResultResponse<string> SaveImage(string id, string savePath)
    {
        InnerLogger.Logger.Debug($"SaveImage. {id}. path: {savePath}");
        return CommandUtil.ExecuteWithArgs(_executorPath, "save -o \"" + savePath + "\" " + id, Timeout);
    }

    public ResultResponse<string> RemoveImage(string id, bool force = false)
    {
        InnerLogger.Logger.Debug($"RemoveImage. {id}. force: {force}");
        var arg = "";
        if (force)
            arg = " --force";
        return CommandUtil.ExecuteWithArgs(_executorPath, "image rm " + id + arg, Timeout);
    }

    public ResultResponse<string> PullImage(string repo, ImageArch? arch = null)
    {
        InnerLogger.Logger.Debug($"PullImage. {repo}. arch = {arch}");
        var arg = "";
        if (arch != null)
        {
            arg = $"--arch={_convertArch((ImageArch)arch)} ";
        }

        //TODO copy command or open cmd
        return CommandUtil.ExecuteWithArgs(_executorPath, "pull " + arg + repo, Timeout);
    }

    public string GetPullImageCommand(string repo, ImageArch? arch = null)
    {
        var arg = "";
        if (arch != null)
        {
            arg = $"--arch={_convertArch((ImageArch)arch)} ";
        }

        return $"{GetContainerId()} pull {arg} {repo}";
    }


    public string TagImage(string id, string newTag)
    {
        throw new NotImplementedException();
    }


    public ResultResponse<List<ContainerInfo>> ListContainers(
        bool all = false,
        string queryName = null,
        string queryId = null)
    {
        var arg = "";
        if (all)
            arg += " -a ";

        if (!string.IsNullOrEmpty(queryName))
            arg += $" -f name={queryName}";
        if (!string.IsNullOrEmpty(queryId))
            arg += $" -f id={queryId} ";

        var result =
            CommandUtil.ExecuteWithArgs(_executorPath, "container ls --no-trunc " + arg + CommandRootJsonFormat,
                Timeout);

        if (!result.Success)
        {
            return new ResultResponse<List<ContainerInfo>>
            {
                Success = false,
                Message = result.Message
            };
        }

        var lines = result.Result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        var infos = new List<PodmanContainerInfo>(lines.Length);
        foreach (var line in lines)
        {
            // Console.WriteLine(line);
            try
            {
                var info = JsonSerializer.Deserialize<PodmanContainerInfo>(line);
                infos.Add(info);
            }
            catch (JsonException ex)
            {
                InnerLogger.Logger.Warn($"GetContainers. JSON Format Error: {ex.Message}");
            }
        }

        return new ResultResponse<List<ContainerInfo>>
        {
            Success = true,
            Result = infos.Select(_convertToBaseContainerInfo).ToList(),
            Message = result.Message
        };
    }


    private static ContainerInfo _convertToBaseContainerInfo(PodmanContainerInfo containerInfo)
    {
        var createAtStr = containerInfo.CreatedAt;
        var createTime = string.IsNullOrEmpty(createAtStr)
            ? FormatUtil.FormatDateTimeISO(containerInfo.Created)
            : FormatUtil.FormatDateTime(createAtStr);

        List<ContainerPortMapping> ports = null;
        if (containerInfo.Ports != null)
        {
            ports = containerInfo.Ports.Select(p =>
                    new ContainerPortMapping
                    {
                        HostIp = p.HostIp,
                        HostPort = p.HostPort,
                        ContainerPort = p.ContainerPort,
                        Range = p.Range,
                        Protocol = p.Protocol
                    })
                .ToList();
        }

        return new ContainerInfo
        {
            ContainerType = ContainerType.Podman,
            RawInfo = containerInfo,
            Command = string.Join(" ", containerInfo.Command),
            CreatedAt = createTime,
            Id = containerInfo.Id,
            Image = containerInfo.Image,
            Labels = containerInfo.Labels,
            Mounts = containerInfo.Mounts,
            Names = containerInfo.Names,
            Networks = containerInfo.Networks,
            State = containerInfo.State,
            Status = containerInfo.Status,
            Ports = ports
        };
    }

    public ResultResponse<ContainerDetailInfo> GetContainer(string id)
    {
        var result = CommandUtil.ExecuteWithArgs(_executorPath, "container inspect " + id + CommandRootJsonFormat,
            Timeout);
        Console.WriteLine(result);
        return null;
    }

    public ResultResponse<string> StartContainer(string id)
    {
        return CommandUtil.ExecuteWithArgs(_executorPath, "container start " + id, Timeout);
    }

    public ResultResponse<string> RestartContainer(string id)
    {
        return CommandUtil.ExecuteWithArgs(_executorPath, "container restart " + id, Timeout);
    }

    public ResultResponse<string> StopContainer(string id, int timeSeconds = -1)
    {
        var arg = "";
        if (timeSeconds > 0)
            arg = $" --time {timeSeconds}";

        return CommandUtil.ExecuteWithArgs(_executorPath, "container stop " + id + arg, Timeout);
    }

    public ResultResponse<string> RemoveContainer(string id, bool force = false)
    {
        if (force)
        {
            StopContainer(id);
        }

        return CommandUtil.ExecuteWithArgs(_executorPath, "container rm " + id, Timeout);
    }

    public ResultResponse<string> KillContainer(string id, string signal = null)
    {
        return CommandUtil.ExecuteWithArgs(_executorPath, "container inspect " + id + CommandRootJsonFormat,
            Timeout);
    }


    public void ExecContainer(string id, string execShellPath, string containerCommand)
    {
        CommandUtil.ExecuteWithArgs(execShellPath, containerCommand, Timeout);
    }


    public void LogContainer(string id, string shellPath, int lastLine = 200)
    {
        throw new NotImplementedException();
    }

    private string _convertArch(ImageArch arch)
    {
        switch (arch)
        {
            case ImageArch.Arm64:
                return "arm64";
            default:
                return "amd64";
        }
    }
}