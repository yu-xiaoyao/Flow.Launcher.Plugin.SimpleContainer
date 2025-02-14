using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Docker;
using Flow.Launcher.Plugin.SimpleContainer.Model;
using Flow.Launcher.Plugin.SimpleContainer.Podman;
using Flow.Launcher.Plugin.SimpleContainer.Util;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.SimpleContainer
{
    public class SimpleContainer : IPlugin, IContextMenu, ISettingProvider
    {
        public const string IconPath = "Images\\SimpleContainer.png";
        public const string IconPathGreen = "Images\\SimpleContainer_G.png";

        private PluginInitContext _context;

        private IContainerManage _cm;
        private Settings _settings;

        public void Init(PluginInitContext context)
        {
            // DEBUG
            InnerLogger.SetAsFlowLauncherLogger(context.API, LoggerLevel.DEBUG);

            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();

            if (_settings.ContainerType == ContainerType.Podman)
            {
                _cm = new PodmanCmdContainerManage(_settings.ContainerExecutePath);
            }
            else
            {
                _cm = new DockerCmdContainerManage(_settings.ContainerExecutePath);
            }

            InnerLogger.Logger.Debug($"Container Type = {_settings.ContainerType}");
        }


        public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            _context.API.LogInfo("CM", $"QueryAsync: {query.Search}. token: {token}");
            return Task.Run(() => Query(query), token);
        }

        public List<Result> Query(Query query)
        {
            var results = new List<Result>();
            if (buildTipResult(query, results))
            {
                return results;
            }

            #region command with start p, ps psa pull

            var firstSearch = query.FirstSearch;
            if (firstSearch.StartsWith("p", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals("p", firstSearch, StringComparison.OrdinalIgnoreCase))
                {
                    var actionKeyword = query.ActionKeyword;
                    var containerId = _cm.GetContainerId();
                    return new List<Result>
                    {
                        _psTipResult(containerId, actionKeyword),
                        _psAllTipResult(containerId, actionKeyword),
                        _pullTipResult(containerId, actionKeyword),
                    };
                }

                if (string.Equals(firstSearch, "ps", StringComparison.OrdinalIgnoreCase))
                {
                    var filterId = _getFilterId(query.SecondToEndSearch);
                    return QueryContainers(query, () =>
                    {
                        if (filterId != null)
                            return _cm.ListContainers(queryId: filterId);
                        return _cm.ListContainers(queryName: query.SecondToEndSearch);
                    });
                }

                if (string.Equals(firstSearch, "psa", StringComparison.OrdinalIgnoreCase))
                {
                    var filterId = _getFilterId(query.SecondToEndSearch);
                    return QueryContainers(query, () =>
                    {
                        if (filterId != null)
                            return _cm.ListContainers(all: true, queryId: filterId);
                        return _cm.ListContainers(all: true, queryName: query.SecondToEndSearch);
                    });
                }

                if (string.Equals("pu", firstSearch, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("pul", firstSearch, StringComparison.OrdinalIgnoreCase))
                {
                    return _tipPullTip(query);
                }

                if (string.Equals(firstSearch, "pull", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(query.SecondToEndSearch))
                        return _imageNameEmptyTip(query);
                    return _buildPullCommandResult(query);
                }

                return _buildNotSupportResult(query);
            }

            #endregion


            #region command with start i, images

            if (firstSearch.StartsWith("i", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(firstSearch, "i", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(firstSearch, "images", StringComparison.OrdinalIgnoreCase))
                {
                    // default filter by name
                    return QueryImages(query, () => _cm.ListImages(queryName: query.SecondToEndSearch));
                }
            }

            #endregion

            #region command with start l, load

            if (firstSearch.StartsWith("l", StringComparison.OrdinalIgnoreCase))
            {
                // load -i filePath
                if (string.Equals("load", firstSearch, StringComparison.OrdinalIgnoreCase))
                    return _loadImage(query);

                // load tip
                if (firstSearch.StartsWith("lo", StringComparison.CurrentCultureIgnoreCase) ||
                    firstSearch.StartsWith("loa", StringComparison.CurrentCultureIgnoreCase))
                    return _loadTip(query);
            }

            #endregion


            return new List<Result>();
        }


        private List<Result> buildError(string title, [CanBeNull] string error)
        {
            InnerLogger.Logger.Warn($"{title} - {error}");
            return new List<Result>
            {
                new()
                {
                    Title = title,
                    SubTitle = error,
                    CopyText = error,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(error, showDefaultNotification: false);
                        return true;
                    },
                    IcoPath = IconPath
                }
            };
        }

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            var data = selectedResult.ContextData;
            return data switch
            {
                ImageInfo imageInfo => _loadImageContextMenu(imageInfo),
                ContainerInfo containerInfo => _loadContainerContextMenu(containerInfo),
                _ => new List<Result>()
            };
        }

        #region Container

        private List<Result> QueryContainers(Query query, Func<ResultResponse<List<ContainerInfo>>> func)
        {
            var rr = func.Invoke();
            if (!rr.Success) return buildError("Containers Error", rr.Message);

            var images = rr.Result;

            return images.Select(c => new Result
            {
                Title = $"{c.GetName()}",
                SubTitle = $"ContainerId: {c.GetShortId()}. Image: {c.Image}",
                IcoPath = ContainerUtil.IsRunning(c.State) ? IconPathGreen : IconPath,
                CopyText = $"{c.GetName()}",
                ContextData = c,
                Action = _ =>
                {
                    _context.API.ChangeQuery("p id:" + c.GetShortId());
                    return false;
                }
            }).ToList();
        }


        private List<Result> _loadContainerContextMenu(ContainerInfo containerInfo)
        {
            var cmId = containerInfo.GetName();
            var isRunning = ContainerUtil.IsRunning(containerInfo.State);

            return new List<Result>
            {
                new()
                {
                    Title = isRunning ? "Stop Container" : "Start Container",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() =>
                        {
                            if (isRunning)
                                _cm.StopContainer(cmId);
                            else
                                _cm.StartContainer(cmId);
                        });
                        return true;
                    },
                },
                new()
                {
                    Title = "Restart Container",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() => { _cm.RestartContainer(cmId); });
                        return true;
                    },
                },
                new()
                {
                    Title = "Remove Container",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() => { _cm.RemoveContainer(cmId); });
                        return true;
                    },
                },
                new()
                {
                    Title = "Remove Container Force",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        if (isRunning)
                            _cm.StopContainer(cmId);
                        Task.Run(() => { _cm.RemoveContainer(cmId); });
                        return true;
                    },
                },
                new()
                {
                    Title = "Kill Container",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() => { _cm.KillContainer(cmId); });
                        return true;
                    },
                },
                new()
                {
                    Title = "Logs 100",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() => { });
                        return true;
                    },
                },
                new()
                {
                    Title = "Logs 20",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() => { });
                        return true;
                    },
                },
                new()
                {
                    Title = "Sh Shell",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() => { });
                        return true;
                    },
                },

                new()
                {
                    Title = "Bash Shell",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(() => { });
                        return true;
                    },
                },
                new()
                {
                    Title = "Copy Container Name",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard($"{containerInfo.GetName()}", showDefaultNotification: false);
                        return false;
                    },
                },

                new()
                {
                    Title = "Copy Container Id",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard($"{containerInfo.GetShortId()}", showDefaultNotification: false);
                        return false;
                    },
                },
                new()
                {
                    Title = "Copy Image Repository",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard($"{containerInfo.Image}", showDefaultNotification: false);
                        return false;
                    },
                },
            };
        }

        #endregion

        #region Image

        private List<Result> QueryImages(Query query, Func<ResultResponse<List<ImageInfo>>> func)
        {
            var rr = func.Invoke();
            if (!rr.Success) return buildError("Images Error", rr.Message);

            var images = rr.Result;
            return images.Select(i => new Result
            {
                Title = $"{i.Repository}:{i.Tag}",
                SubTitle = $"ImageId: {i.GetShortId()}  Created: {i.CreatedAt.ToString("G")}  Size: {i.Size}",
                CopyText = $"{i.Repository}:{i.Tag}",
                ContextData = i,
                AutoCompleteText = $"{query.ActionKeyword} {query.FirstSearch} {i.Repository}:{i.Tag}",
                Action = _ =>
                {
                    _context.API.CopyToClipboard($"{i.Repository}:{i.Tag}");
                    return true;
                },
                IcoPath = IconPath,
            }).ToList();
        }

        private List<Result> _loadImageContextMenu(ImageInfo imageInfo)
        {
            return new List<Result>
            {
                new()
                {
                    Title = "Export Image",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _exportImage(imageInfo);
                        return true;
                    },
                },
                new()
                {
                    Title = "Remove Image",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _removeImage(imageInfo, false);
                        return true;
                    },
                },
                new()
                {
                    Title = "Remove Image Force",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _removeImage(imageInfo, true);
                        return true;
                    },
                },
                new()
                {
                    Title = "Copy Image Repository",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard($"{imageInfo.Repository}:{imageInfo.Tag}",
                            showDefaultNotification: false);
                        return true;
                    },
                },
                new()
                {
                    Title = "Copy Image Id",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        _context.API.CopyToClipboard(imageInfo.GetShortId(), showDefaultNotification: false);
                        return true;
                    },
                },
            };
        }


        private void _exportImage(ImageInfo imageInfo)
        {
            string exportPath;
            if (string.IsNullOrEmpty(_settings.ExportDirectory))
            {
                // default desktop 
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var name = imageInfo.Repository.Replace("/", " ").Replace(":", "_");
                exportPath = $"{dir}\\{name}_{imageInfo.Tag}.tar";
            }
            else
            {
                // TODO open save file dialog
                return;
            }

            InnerLogger.Logger.Debug($"ExportImage. {imageInfo.Repository}:{imageInfo.Tag}. path: {exportPath}");

            Task.Run(() =>
            {
                var rr = _cm.SaveImage($"{imageInfo.Repository}:{imageInfo.Tag}", exportPath);
                if (rr.Success)
                    _context.API.ShowMsg("Export Success", $"Export Image Success = {exportPath}", IconPath);
                else
                    _context.API.ShowMsg("Export Failed", $"Export Image Failed = {rr.Message}", IconPath);
            });
        }

        private void _removeImage(ImageInfo imageInfo, bool force)
        {
            InnerLogger.Logger.Debug($"RemoveImage. {imageInfo.Repository}:{imageInfo.Tag}. force: {force}");
            Task.Run(() =>
            {
                var rr = _cm.RemoveImage($"{imageInfo.Repository}:{imageInfo.Tag}", force);
                if (rr.Success)
                    _context.API.ShowMsg("Remove Success", "Remove Image Success", IconPath);
                else
                    _context.API.ShowMsg("Remove Failed", $"Remove Image Failed = {rr.Message}", IconPath);
            });
        }

        #endregion


        [CanBeNull]
        private string _getFilterId(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return null;
            if (filter.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            {
                return filter[2..];
            }

            return null;
        }


        #region 一些提示

        private bool buildTipResult(Query query, List<Result> results)
        {
            if (string.IsNullOrEmpty(query.Search))
            {
                var keyword = query.ActionKeyword;
                results.Add(new Result
                {
                    IcoPath = IconPath,
                    Title = "List containers",
                    SubTitle = $"{_cm.GetContainerId()} ps",
                    CopyText = $"{_cm.GetContainerId()} ps",
                    AutoCompleteText = $"{keyword} ps",
                    Action = _ =>
                    {
                        _context.API.ChangeQuery($"{keyword} ps ");
                        return false;
                    }
                });
                results.Add(new Result
                {
                    IcoPath = IconPath,
                    Title = "Lists images",
                    SubTitle = $"{_cm.GetContainerId()} images ls",
                    CopyText = $"{_cm.GetContainerId()} images ls",
                    AutoCompleteText = $"{keyword} images ",
                    Action = _ =>
                    {
                        _context.API.ChangeQuery($"{keyword} images ");
                        return false;
                    }
                });

                return true;
            }

            return false;
        }

        private List<Result> _loadTip(Query query)
        {
            return new List<Result>()
            {
                new()
                {
                    Title = "Load Image",
                    IcoPath = IconPath,
                    AutoCompleteText = $"{query.ActionKeyword} load ",
                    Action = _ =>
                    {
                        _context.API.ChangeQuery($"{query.ActionKeyword} load ");
                        return false;
                    },
                }
            };
        }

        private List<Result> _imageNameEmptyTip(Query query)
        {
            return new List<Result>
            {
                new()
                {
                    Title = "Input the image name or id to pull",
                    IcoPath = IconPath,
                    AutoCompleteText = $"{query.ActionKeyword} pull ",
                    Action = _ =>
                    {
                        _context.API.ChangeQuery($"{query.ActionKeyword} pull ");
                        return false;
                    },
                }
            };
        }

        private List<Result> _loadImage(Query query)
        {
            var filePath = query.SecondToEndSearch;
            Result result;
            if (File.Exists(filePath))
            {
                result = new Result
                {
                    IcoPath = IconPath,
                    Title = "Start Load Image",
                    SubTitle = filePath,
                    Action = _ =>
                    {
                        Task.Run(() => { });
                        return true;
                    }
                };
            }
            else
            {
                result = new Result
                {
                    IcoPath = IconPath,
                    Title = "Image File Not Found",
                    SubTitle = filePath,
                    Action = _ => true
                };
            }

            return new List<Result>
            {
                result
            };
        }

        private List<Result> _buildNotSupportResult(Query query)
        {
            return new List<Result>
            {
                new()
                {
                    Title = "Unsupported Operation",
                    IcoPath = IconPath
                }
            };
        }

        private List<Result> _tipPullTip(Query query)
        {
            return new List<Result>
            {
                new()
                {
                    Title = "Pull Image",
                    IcoPath = IconPath,
                    AutoCompleteText = $"{query.ActionKeyword} pull ",
                    Action = _ =>
                    {
                        _context.API.ChangeQuery($"{query.ActionKeyword} pull ");
                        return false;
                    },
                }
            };
        }


        private List<Result> _buildPullCommandResult(Query query)
        {
            var pullName = query.SecondToEndSearch;
            var defaultCmd = _cm.GetPullImageCommand(pullName);
            var x64Cmd = _cm.GetPullImageCommand(pullName, ImageArch.X64);
            var arm64Cmd = _cm.GetPullImageCommand(pullName, ImageArch.Arm64);

            return new List<Result>
            {
                new()
                {
                    Title = "Pull Image",
                    SubTitle = defaultCmd,
                    CopyText = defaultCmd,
                    IcoPath = IconPath
                },
                new()
                {
                    Title = "Pull Image amd64",
                    SubTitle = x64Cmd,
                    CopyText = x64Cmd,
                    IcoPath = IconPath
                },
                new()
                {
                    Title = "Pull Image arm64",
                    SubTitle = arm64Cmd,
                    CopyText = arm64Cmd,
                    IcoPath = IconPath
                }
            };
        }

        private Result _pullTipResult(string containerId, string actionKeyword)
        {
            return new Result
            {
                Title = "Pull Image",
                SubTitle = $"{containerId} pull <image>",
                IcoPath = IconPath,
                AutoCompleteText = $"{actionKeyword} pull ",
                Action = _ =>
                {
                    _context.API.ChangeQuery($"{actionKeyword} pull ");
                    return false;
                },
            };
        }


        private Result _psTipResult(string containerId, string actionKeyword)
        {
            return new Result
            {
                Title = "List Containers",
                SubTitle = $"{containerId} ps",
                CopyText = $"{containerId} ps",
                IcoPath = IconPath,
                Action = _ =>
                {
                    _context.API.ChangeQuery($"{actionKeyword} ps");
                    return false;
                },
            };
        }

        private Result _psAllTipResult(string containerId, string actionKeyword)
        {
            return new Result
            {
                Title = "List All Containers",
                SubTitle = $"{containerId} ps -a",
                CopyText = $"{containerId} ps -a",
                IcoPath = IconPath,
                Action = _ =>
                {
                    _context.API.ChangeQuery($"{actionKeyword} psa");
                    return false;
                },
            };
        }

        #endregion

        public Control CreateSettingPanel()
        {
            return new SettingControl(_context, _settings);
        }
    }
}