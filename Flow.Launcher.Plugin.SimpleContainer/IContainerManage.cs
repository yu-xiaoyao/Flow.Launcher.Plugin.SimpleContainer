using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Model;

namespace Flow.Launcher.Plugin.SimpleContainer;

/// <summary>
/// 容器管理接口
/// </summary>
public interface IContainerManage
{
    public ContainerType GetContainerType();
    public string GetContainerId();

    /// <summary>
    /// 是否运行中
    /// </summary>
    /// <returns></returns>
    public bool IsRunning();

    /// <summary>
    /// 版本号
    /// </summary>
    /// <returns></returns>
    public Tuple<bool, string> GetVersion();

    /// <summary>
    /// list images
    /// </summary>
    /// <param name="all"></param>
    /// <param name="dangling"></param>
    /// <param name="queryName"></param>
    /// <param name="queryId"></param>
    /// <returns></returns>
    public ResultResponse<List<ImageInfo>> ListImages(
        bool all = false,
        bool dangling = false,
        string queryName = null,
        string queryId = null);

    /// <summary>
    /// docker images inspect 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ResultResponse<ImageDetailInfo> GetImage(string id);

    public ResultResponse<string> SaveImage(string id, string savePath);

    public ResultResponse<string> RemoveImage(string id, bool force = false);

    public ResultResponse<string> PullImage(string repo, ImageArch? arch = null);

    public string GetPullImageCommand(string repo, ImageArch? arch = null);

    public string TagImage(string id, string newTag);


    public ResultResponse<List<ContainerInfo>> ListContainers(
        bool all = false,
        string queryName = null,
        string queryId = null);

    /// <summary>
    /// docker inspect 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ResultResponse<ContainerDetailInfo> GetContainer(string id);

    public ResultResponse<string> StartContainer(string id);
    public ResultResponse<string> RestartContainer(string id);
    public ResultResponse<string> StopContainer(string id, int timeSeconds = -1);
    public ResultResponse<string> RemoveContainer(string id, bool force = false);
    public ResultResponse<string> KillContainer(string id, string signal = null);

    public void ExecContainer(string id, string execShellPath, string containerCommand);

    public void LogContainer(string id, string execShellPath, int lastLine = 200);
}