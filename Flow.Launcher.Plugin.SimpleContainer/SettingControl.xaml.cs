using System.IO;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.SimpleContainer.Common;

namespace Flow.Launcher.Plugin.SimpleContainer;

public partial class SettingControl : UserControl
{
    private readonly PluginInitContext _context;
    private readonly Settings _settings;

    public SettingControl(PluginInitContext context, Settings settings)
    {
        _context = context;
        _settings = settings;
        InitializeComponent();

        _initViewRender();
    }

    private void _initViewRender()
    {
        if (_settings.ContainerType == ContainerType.Podman)
        {
            RadioButtonPodman.IsChecked = true;
        }
        else
        {
            RadioButtonDocker.IsChecked = true;
        }
    }

    private void ContainerType_Docker_Checked(object sender, RoutedEventArgs e)
    {
        RadioButtonDocker.IsChecked = true;
        _settings.ContainerType = ContainerType.Docker;
        _context.API.SavePluginSettings();
    }

    private void ContainerType_Podman_Checked(object sender, RoutedEventArgs e)
    {
        RadioButtonPodman.IsChecked = true;
        _settings.ContainerType = ContainerType.Podman;
        _context.API.SavePluginSettings();
    }

    private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
    {
        _settings.ContainerType = ContainerType.Docker;
        _settings.ContainerExecutePath = null;
        _context.API.SavePluginSettings();
    }

    private void ButtonSavePath_OnClick(object sender, RoutedEventArgs e)
    {
        _settings.ContainerExecutePath = TextBoxExecutePath.Text.Trim();
        _context.API.SavePluginSettings();
    }


    private void ButtonSetExportPath_OnClick(object sender, RoutedEventArgs e)
    {
        var dir = TextBoxExportDirectory.Text.Trim();

        if (Directory.Exists(dir))
        {
            _settings.ExportDirectory = dir;
            _context.API.SavePluginSettings();
        }
        else
        {
            _context.API.ShowMsgError("Error", "The directory does not exist");
        }
    }

    private void ButtonResetExportPath_OnClick(object sender, RoutedEventArgs e)
    {
        _settings.ExportDirectory = "";
        _context.API.SavePluginSettings();
    }
}