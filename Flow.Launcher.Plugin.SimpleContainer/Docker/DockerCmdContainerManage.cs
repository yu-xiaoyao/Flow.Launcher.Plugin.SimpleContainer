using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Flow.Launcher.Plugin.SimpleContainer.Command;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Model;
using Flow.Launcher.Plugin.SimpleContainer.Util;

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
        throw new NotImplementedException();
    }

    public string GetPullImageCommand(string repo, ImageArch? arch = null)
    {
        throw new NotImplementedException();
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
        var labels = containerInfo.Labels.Split(",", StringSplitOptions.TrimEntries);

        return new ContainerInfo
        {
            ContainerType = ContainerType.Docker,
            //TODO more field
        };
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
        throw new NotImplementedException();
    }

    public ResultResponse<string> RestartContainer(string id)
    {
        throw new NotImplementedException();
    }

    public ResultResponse<string> StopContainer(string id, int timeSeconds = -1)
    {
        throw new NotImplementedException();
    }

    public ResultResponse<string> KillContainer(string id, string signal = null)
    {
        throw new NotImplementedException();
    }

    public ResultResponse<string> RemoveContainer(string id, bool force = false)
    {
        throw new NotImplementedException();
    }

    public void ExecContainer(string id, string execShellPath, string containerCommand)
    {
        throw new NotImplementedException();
    }


    public void LogContainer(string id, string shellPath, int lastLine = 200)
    {
        throw new NotImplementedException();
    }
}