using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Flow.Launcher.Plugin.SimpleContainer.Common;
using Flow.Launcher.Plugin.SimpleContainer.Util;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.SimpleContainer.Command;

/// <summary>
/// execute command util
/// </summary>
public class CommandUtil
{
    /// <summary>
    /// execute command
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="arguments"></param>
    /// <param name="exitTimeout"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static ResultResponse<string> ExecuteWithArgs(string fileName, string arguments,
        int exitTimeout = 0,
        string separator = "\n")
    {
        InnerLogger.Logger.Info($"{fileName} {arguments}");

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        try
        {
            using var process = Process.Start(psi);
            if (process == null)
            {
                return new ResultResponse<string>
                {
                    Success = false,
                    Message = "Running Progress is Empty"
                };
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            var exitCode = process.ExitCode;

            if (exitTimeout > 0)
                process.WaitForExit(exitTimeout);
            else
                process.WaitForExit();


            // if (!process.HasExited)
            // {
            //     Thread.Sleep(1000);
            // }

            if (exitCode != 0)
            {
                InnerLogger.Logger.Warn($"ExecuteCmd: {fileName} {arguments}, exitCode: {exitCode}");

                return new ResultResponse<string>
                {
                    Success = false,
                    Message = error
                };
            }

            var lines = output.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            // windows line seq
            var result = string.Join("\r\n", lines);

            return new ResultResponse<string>
            {
                Success = true,
                Result = result
            };
        }
        catch (Exception ex)
        {
            InnerLogger.Logger.Error($"ExecuteCmd: {fileName} {arguments}", ex);
            return new ResultResponse<string>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}