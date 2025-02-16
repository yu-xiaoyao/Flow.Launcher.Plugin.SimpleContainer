using System;
using System.Runtime.InteropServices;
using Flow.Launcher.Plugin.SimpleContainer.Docker;
using Flow.Launcher.Plugin.SimpleContainer.Podman;
using Flow.Launcher.Plugin.SimpleContainer.Util;

namespace Flow.Launcher.Plugin.SimpleContainer;

public class Main_Test
{
    private static IContainerManage cm;

    public static void Main()
    {
        InnerLogger.SetAsConsoleLogger(LoggerLevel.DEBUG);

        // test_podman();
        test_docker_container_port_resolve();
    }


    #region Podman Test

    private static void test_podman()
    {
        cm = new PodmanCmdContainerManage();

        // Console.WriteLine("IsRunning: " + cm.IsRunning());
        // Console.WriteLine("GetVersion: " + cm.GetVersion());

        // test_podman_images();
        // test_podman_image();

        // test_podman_save_image();

        // test_podman_containers();

        // test_podman_start_container();
        // test_podman_stop_container();
        // test_podman_restart_container();
    }


    private static void test_podman_start_container()
    {
        var rr = cm.StartContainer("redis");
        Console.WriteLine("StartContainer: " + rr.Success);

        Console.WriteLine("Result:" + rr.Result);
        Console.WriteLine("Message:" + rr.Message);
    }

    private static void test_podman_stop_container()
    {
        var rr = cm.StopContainer("redis", 6);
        Console.WriteLine("StopContainer: " + rr.Success);
        Console.WriteLine("Result:" + rr.Result);
        Console.WriteLine("Message:" + rr.Message);
    }

    private static void test_podman_restart_container()
    {
        var rr = cm.RestartContainer("redis");
        Console.WriteLine("RestartContainer: " + rr.Success);
        Console.WriteLine("Result:" + rr.Result);
        Console.WriteLine("Message:" + rr.Message);
    }


    private static void test_podman_save_image()
    {
        InnerLogger.Logger.Debug("start save");
        var rr = cm.SaveImage("docker.io/library/redis:latest", "C:\\Users\\farben\\Desktop\\redis.tar");
        InnerLogger.Logger.Debug($"save result = {rr.Success}. message: {rr.Message}");
    }

    private static void test_podman_images()
    {
        var rr = cm.ListImages();
        if (!rr.Success)
        {
            Console.WriteLine("error = " + rr.Message);
            return;
        }

        var images = rr.Result;
        Console.WriteLine($"image size {images.Count}");

        foreach (var i in images)
        {
            Console.WriteLine(
                $"Image = {i.Repository}:{i.Tag}. Id = {i.Id}, CreatedAt = {i.CreatedAt}, Size = {i.Size}, Digest = {i.Digest}, Containers = {i.Containers}");
        }
    }

    private static void test_podman_image()
    {
        var containerId = "9b6a628072eb";
        var rr = cm.GetContainer(containerId);
        if (rr.Success)
        {
            Console.WriteLine($"inspect: {rr.Result}");
        }
    }

    private static void test_podman_containers()
    {
        var rr = cm.ListContainers();
        if (!rr.Success)
        {
            Console.WriteLine("error = " + rr.Message);
            return;
        }

        if (rr.Success)
        {
            var containers = rr.Result;
            Console.WriteLine($"Container.size = {containers.Count}");

            foreach (var c in containers)
            {
                Console.WriteLine($"Image = {c.Image}. Id = {c.GetShortId()}. Name = {c.GetName()}");
            }
        }
    }

    #endregion

    #region Docker Test

    private static void test_docker_container_port_resolve()
    {
        cm = new DockerCmdContainerManage();
        cm.ListContainers();
    }


    private static void test_docker()
    {
        cm = new DockerCmdContainerManage();
        test_docker_images();
        // test_docker_containers();
    }

    private static void test_docker_version()
    {
        var version = cm.GetVersion();
        if (version.Item1)
        {
            Console.WriteLine("version: " + version.Item2);
        }
        else
        {
            Console.WriteLine("error info: " + version.Item2);
        }
    }

    private static void test_docker_containers()
    {
        var containers = cm.ListContainers(queryName: "1");
        if (containers.Success)
        {
            var list = containers.Result;
            foreach (var info in list)
            {
                Console.WriteLine($"name = {info.Names}. Id = {info.Id}");
            }


            // cm.GetContainer("10a92b3c8f8e");
        }
        else
        {
            Console.WriteLine("error info: " + containers.Message);
        }
    }

    private static void test_docker_images()
    {
        var images = cm.ListImages();
        if (images.Success)
        {
            var list = images.Result;
            foreach (var info in list)
            {
                Console.WriteLine($"repository = {info.Repository}:{info.Tag}. Id = {info.Id}");
            }
        }
        else
        {
            Console.WriteLine("error info: " + images.Message);
        }
    }

    #endregion
}