using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Flow.Launcher.Plugin.SimpleContainer.Command;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Model;
using Flow.Launcher.Plugin.SimpleContainer.Util;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.SimpleContainer.Docker;

/// <summary>
/// Docker 容器 
/// </summary>
public class DockerCmdContainerManage : IContainerManage
{
    private const string CommandRootJsonFormat = " --format \"{{json .}}\"";

    private readonly string _executorPath;
    private int Timeout = 10 * 1000;


    public DockerCmdContainerManage() : this("docker")
    {
    }

    public DockerCmdContainerManage(string dockerPath)
    {
        _executorPath = string.IsNullOrEmpty(dockerPath) ? "docker" : dockerPath;
    }

    public ContainerType GetContainerType()
    {
        return ContainerType.Docker;
    }

    public string GetContainerId()
    {
        return "docker";
    }


    private ResultResponse<string> _getVersion()
    {
        return CommandUtil.ExecuteWithArgs(_executorPath, "version --format {{.Server.Version}}", Timeout);
    }

    public bool IsRunning()
    {
        var result = _getVersion();
        return result.Success;
    }


    public Tuple<bool, string> GetVersion()
    {
        var result = _getVersion();
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


        // if (!string.IsNullOrEmpty(queryName))
        //     arg += $" -f \"NAME={queryName}\"";
        // if (!string.IsNullOrEmpty(queryId))
        //     arg += $" -f \"ID={queryId}\"";

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
        var infos = new List<DockerImageInfo>(lines.Length);
        foreach (var line in lines)
        {
            // Console.WriteLine(line);
            try
            {
                var info = JsonSerializer.Deserialize<DockerImageInfo>(line);
                infos.Add(info);
            }
            catch (JsonException ex)
            {
                // skip
                // Console.WriteLine($"JSON 反序列化失败: {ex.Message}");
            }
        }

        return new ResultResponse<List<ImageInfo>>
        {
            Success = true,
            Result = infos.Select(_convertToBaseImageInfo).ToList(),
            Message = result.Message
        };
    }

    private static ImageInfo _convertToBaseImageInfo(DockerImageInfo imageInfo)
    {
        return new ImageInfo
        {
            ContainerType = ContainerType.Docker,
            RawInfo = imageInfo,
            Id = imageInfo.ID,
            Repository = imageInfo.Repository,
            Tag = imageInfo.Tag,
            Size = imageInfo.Size,
            Digest = imageInfo.Digest,
            Containers = imageInfo.Containers,
            CreatedAt = FormatUtil.FormatDateTime(imageInfo.CreatedAt)
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
            var detail = JsonSerializer.Deserialize<DockerImageDetailInfo>(result.Result);
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
            // skip
            // Console.WriteLine($"JSON 反序列化失败: {ex.Message}");
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
            arg = $"--platform={_convertPlatform((ImageArch)arch)} ";
        }

        //TODO copy command or open cmd
        return CommandUtil.ExecuteWithArgs(_executorPath, "pull " + arg + repo, Timeout);
    }

    public string GetPullImageCommand(string repo, ImageArch? arch = null)
    {
        var arg = "";
        if (arch != null)
        {
            arg = $"--arch={_convertPlatform((ImageArch)arch)} ";
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
            arg += $" -f \"NAME={queryName}\" ";
        if (!string.IsNullOrEmpty(queryId))
            arg += $" -f \"ID={queryId}\" ";

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
        var infos = new List<DockerContainerInfo>(lines.Length);
        foreach (var line in lines)
        {
            // Console.WriteLine(line);
            try
            {
                var info = JsonSerializer.Deserialize<DockerContainerInfo>(line);
                infos.Add(info);
            }
            catch (JsonException ex)
            {
                // skip
                // Console.WriteLine($"JSON 反序列化失败: {ex.Message}");
            }
        }

        return new ResultResponse<List<ContainerInfo>>
        {
            Success = true,
            Result = infos.Select(_convertToBaseContainerInfo).ToList(),
            Message = result.Message
        };
    }

    private static ContainerInfo _convertToBaseContainerInfo(DockerContainerInfo containerInfo)
    {
        var createTime = FormatUtil.FormatDateTime(containerInfo.CreatedAt);

        InnerLogger.Logger.Info(containerInfo.Ports);

        List<ContainerPortMapping> ports = _resolveDockerPortMapping(containerInfo.Ports);
        // if (containerInfo.Ports != null)
        // {
        //     ports = containerInfo.Ports.Select(p =>
        //             new ContainerPortMapping
        //             {
        //                 HostIp = p.HostIp,
        //                 HostPort = p.HostPort,
        //                 ContainerPort = p.ContainerPort,
        //                 Range = p.Range,
        //                 Protocol = p.Protocol
        //             })
        //         .ToList();
        // }
        //
        // return new ContainerInfo
        // {
        //     ContainerType = ContainerType.Docker,
        //     RawInfo = containerInfo,
        //     Command = string.Join(" ", containerInfo.Command),
        //     CreatedAt = createTime,
        //     Id = containerInfo.ID,
        //     Image = containerInfo.Image,
        //     Labels = containerInfo.Labels,
        //     Mounts = containerInfo.Mounts,
        //     Names = containerInfo.Names,
        //     Networks = containerInfo.Networks,
        //     State = containerInfo.State,
        //     Status = containerInfo.Status,
        //     Ports = ports
        // };
        return new ContainerInfo();
    }

    public static List<ContainerPortMapping> _resolveDockerPortMapping(string portsStr)
    {
        var ports = new List<ContainerPortMapping>();
        if (string.IsNullOrEmpty(portsStr)) return ports;


        var portList = portsStr.Split(", ");

        InnerLogger.Logger.Debug("portList = " + portList.Length);

        foreach (var str in portList)
        {
            var split = str.Split("->");
            if (split.Length != 2) continue;

            var first = split[0].Trim();
            var second = split[1].Trim();


            try
            {
                var index = first.LastIndexOf(":", StringComparison.Ordinal);
                var hostIp = "";
                string hostPortStr;
                if (index > 0)
                {
                    hostIp = first[..index];
                    hostPortStr = first[(index + 1)..];
                }
                else
                {
                    hostPortStr = first;
                }

                var hostPortTryParse = int.TryParse(hostPortStr, out var hostPort);
                if (!hostPortTryParse)
                {
                    InnerLogger.Logger.Error(
                        $"Host Port Convert Error. hostIp = [{portsStr}]. hostPortStr = [{hostPortStr}]");
                    continue;
                }

                var protocol = "tcp";

                string containerPortStr;

                var si = second.IndexOf("/", StringComparison.Ordinal);
                if (si != -1)
                {
                    containerPortStr = second[..si];
                    protocol = second[(si + 1)..];
                }
                else
                {
                    containerPortStr = second;
                }

                var containerPortTryParse = int.TryParse(containerPortStr, out var containerPort);
                if (!containerPortTryParse)
                {
                    InnerLogger.Logger.Error(
                        $"Container Port Convert Error. hostIp = [{portsStr}]. hostPortStr = [{hostPortStr}]");
                    continue;
                }

                ports.Add(new ContainerPortMapping
                {
                    HostIp = hostIp,
                    HostPort = hostPort,
                    Protocol = protocol,
                    ContainerPort = containerPort
                });
            }
            catch (Exception e)
            {
                InnerLogger.Logger.Error($"portsStr = [{portsStr}]. cause: {e.Message}");
            }
        }

        return ports;
    }

    public ResultResponse<ContainerDetailInfo> GetContainer(string id)
    {
        var result = CommandUtil.ExecuteWithArgs(_executorPath, "container inspect " + id + " --format \"{{json .}}\"",
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


    private string _convertPlatform(ImageArch arch)
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