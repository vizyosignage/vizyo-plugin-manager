using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualBasic;
using Vizyo.Plugin.Playground.Services;
using Vizyo.Plugin.Shared.Contracts;
using Microsoft.Extensions.Hosting;

namespace Vizyo.Plugin.Playground.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        //private readonly PluginHost _pluginHost;
        private readonly PluginManager _pluginManager;


        [ObservableProperty] private ObservableCollection<SampleViewModel> _samples;
        [ObservableProperty] private SampleViewModel? _currentSample;
        [ObservableProperty] private Control? _control;

        private (Assembly? Assembly, AssemblyLoadContext? Context)? _previous;
        private IStorageFile? _openXamlFile;
        private IStorageFile? _openCodeFile;
        private bool _update;

        public IStorageProvider? StorageProvider { get; set; }

        public ICommand RunCommand { get; }

        public ICommand OpenDllFileCommand { get; }

        public ICommand OpenXamlAndCodeFileCommand { get; }

        public ICommand OpenXamlFileCommand { get; }

        public ICommand SaveXamlFileCommand { get; }

        public ICommand OpenCodeFileCommand { get; }

        public ICommand SaveCodeFileCommand { get; }

        public ICommand OpenFolderCommand { get; }

        public MainViewModel() 
        {
            LoadLogo();

            //_pluginHost = new PluginHost();
            //_pluginManager = new PluginManager(_pluginHost);
            //_pluginHost.OnPluginMessage += (name, msg) =>
            //{
            //    Dispatcher.UIThread.Post(() => {
            //        Debug.WriteLine($"{name}: {msg}");
            //    });
            //};

            _pluginManager = new PluginManager();

            OpenXamlFileCommand = new AsyncRelayCommand(async () => await OpenXamlFile());
            SaveXamlFileCommand = new AsyncRelayCommand(async () => await SaveXamlFile());
            OpenCodeFileCommand = new AsyncRelayCommand(async () => await OpenCodeFile());
            SaveCodeFileCommand = new AsyncRelayCommand(async () => await SaveCodeFile());
            OpenXamlAndCodeFileCommand = new AsyncRelayCommand(async () => await OpenXamlAndCodeFiles());
            OpenDllFileCommand = new AsyncRelayCommand(async () => await OpenDllFile());
            OpenFolderCommand = new AsyncRelayCommand(async () => await OpenFolder());
            RunCommand = new RelayCommand(() => Run(_currentSample?.Xaml, _currentSample?.Code));

            RegisterPluginMessages();
        }

        private void RegisterPluginMessages()
        {
            WeakReferenceMessenger.Default.Register<MainViewModel, ValueChangedMessage<(string, string, string)>, string>(
                this,
                "PluginMessage",
                (r, m) =>
                {
                    var (plugin, type, message) = m.Value;
                    Debug.WriteLine($"Plugin: {plugin} Type: {type} Message: {message}");
                });

            //WeakReferenceMessenger.Default.Register<PluginMessage>(this, (r, m) =>
            //{
            //    Debug.WriteLine($"Plugin: {m.PluginName} Type: {m.MessageType} Message: {m.Message}");
            //});
        }

    
        private static List<FilePickerFileType> GetXamlAndCodeFileTypes()
        {
            return new List<FilePickerFileType>
        {
            StorageService.Axaml,
            StorageService.Xaml,
            StorageService.CSharp,
            StorageService.All
        };
        }

        private static List<FilePickerFileType> GetXamlFileTypes()
        {
            return new List<FilePickerFileType>
        {
            StorageService.Axaml,
            StorageService.Xaml,
            StorageService.All
        };
        }

        private static List<FilePickerFileType> GetCodeFileTypes()
        {
            return new List<FilePickerFileType>
        {
            StorageService.CSharp,
            StorageService.All
        };
        }

        private static List<FilePickerFileType> GetDllFileTypes()
        {
            return new List<FilePickerFileType>
        {
            StorageService.DLL,
            StorageService.All
        };
        }

        private async void Run(string? xaml, string? code)
        {
            await RunInternal(xaml, code);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026")]
        [UnconditionalSuppressMessage("Trimming", "IL2072")]
        // PasswordBox="*" uses Char.Parse for some reason, so we need to preserve that. TODO: fix compiler.
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(char))]
        private async Task RunInternal(string? xaml, string? code)
        {
            if (_update)
                return;

            _update = true;

            try
            {
                Control = null;
                if (!OperatingSystem.IsBrowser())
                {
                    // TODO: Unload previously loaded assembly.
                    if (_previous is { })
                    {
                        _previous?.Context?.Unload();
                        _previous = null;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                Assembly? scriptAssembly = null;

                if (code is { } && !string.IsNullOrWhiteSpace(code))
                {
                    try
                    {
                        _previous = CompilerService.GetScriptAssembly(code);
                        if (_previous?.Assembly is { })
                        {
                            scriptAssembly = _previous?.Assembly;
                            Debug.WriteLine($"Compiled assembly: {scriptAssembly?.GetName().Name}");
                        }
                        else
                        {
                            throw new Exception("Failed to compile code.");
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                        return;
                    }
                }

                if (scriptAssembly is { })
                {
                    var types = scriptAssembly.GetTypes();
                    //foreach (var t in types)
                    //{
                    //    Debug.WriteLine(t.Name);
                    //}

                    var type = types.FirstOrDefault(x => x.Name.Contains("View")); //(x => x.Name == "SampleView");
                    if (type != null)
                    {
                        var rootInstance = Activator.CreateInstance(type);

                        await using var stream = new MemoryStream();
                        await using var writer = new StreamWriter(stream);
                        await writer.WriteAsync(xaml);
                        await writer.FlushAsync();
                        stream.Position = 0;

                        var control = AvaloniaRuntimeXamlLoader.Load(stream, scriptAssembly, rootInstance);
                        Control = (Control)control;
                    }
                    else
                    {
                        type = types.FirstOrDefault(x => x.Name.Contains("Service"));
                        if (type != null)
                        {
                            Activator.CreateInstance(type);
                        }
                    }
                }
                else
                {
                    var control = AvaloniaRuntimeXamlLoader.Parse<Control?>(xaml, null);
                    Control = control;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
            finally
            {
                _update = false;
            }
        }

        private async Task OpenXamlFile()
        {
            if (CurrentSample is null)
            {
                CurrentSample = new SampleViewModel("","","",null,null);
            }

            if (StorageProvider is null)
            {
                return;
            }

            var result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open xaml",
                FileTypeFilter = GetXamlFileTypes(),
                AllowMultiple = false
            });

            var file = result.FirstOrDefault();
            if (file is not null)
            {
                try
                {
                    _openXamlFile = file;
                    await using var stream = await _openXamlFile.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var fileContent = await reader.ReadToEndAsync();
                    CurrentSample.Xaml = fileContent;
                    Run(CurrentSample.Xaml, CurrentSample.Code);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        private async Task SaveXamlFile()
        {
            if (CurrentSample is null)
            {
                return;
            }

            if (_openXamlFile is null)
            {
                if (StorageProvider is null)
                {
                    return;
                }

                var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save xaml",
                    FileTypeChoices = GetXamlFileTypes(),
                    SuggestedFileName = Path.GetFileNameWithoutExtension("playground"),
                    DefaultExtension = "axaml",
                    ShowOverwritePrompt = true
                });

                if (file is not null)
                {
                    try
                    {
                        _openXamlFile = file;
                        await using var stream = await _openXamlFile.OpenWriteAsync();
                        await using var writer = new StreamWriter(stream);
                        await writer.WriteAsync(CurrentSample.Xaml);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                    }
                }
            }
            else
            {
                await using var stream = await _openXamlFile.OpenWriteAsync();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(CurrentSample.Xaml);
            }
        }

        private async Task OpenCodeFile()
        {
            if (CurrentSample is null)
            {
                CurrentSample = new SampleViewModel("", "", "", null, null);
            }

            if (StorageProvider is null)
            {
                return;
            }

            var result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open code",
                FileTypeFilter = GetCodeFileTypes(),
                AllowMultiple = false
            });

            var file = result.FirstOrDefault();
            if (file is not null)
            {
                try
                {
                    _openCodeFile = file;
                    await using var stream = await _openCodeFile.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var fileContent = await reader.ReadToEndAsync();
                    CurrentSample.Code = fileContent;
                    //Run(CurrentSample.Xaml, CurrentSample.Code);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        private async Task SaveCodeFile()
        {
            if (CurrentSample is null)
            {
                return;
            }

            if (_openCodeFile is null)
            {
                if (StorageProvider is null)
                {
                    return;
                }

                var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save code",
                    FileTypeChoices = GetCodeFileTypes(),
                    SuggestedFileName = Path.GetFileNameWithoutExtension("playground"),
                    DefaultExtension = "cs",
                    ShowOverwritePrompt = true
                });

                if (file is not null)
                {
                    try
                    {
                        _openCodeFile = file;
                        await using var stream = await _openCodeFile.OpenWriteAsync();
                        await using var writer = new StreamWriter(stream);
                        await writer.WriteAsync(CurrentSample.Code);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                    }
                }
            }
            else
            {
                await using var stream = await _openCodeFile.OpenWriteAsync();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(CurrentSample.Code);
            }
        }

        private async Task OpenDllFile()
        {

            if (StorageProvider is null)
            {
                return;
            }

            var result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open dll",
                FileTypeFilter = GetDllFileTypes(),
                AllowMultiple = false
            });

            var file = result.FirstOrDefault();
            if (file is not null)
            {
                try
                {
                    using var stream = await file.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);

                    Control = await _pluginManager.LoadPluginFromByte(memoryStream.ToArray());
                    //OnPropertyChanged(nameof(Control));
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        private async Task OpenFolder()
        {
            CurrentSample = new SampleViewModel("", "", "", null, null);
            Control = null;

            if (StorageProvider is null)
            {
                return;
            }

            var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select folder",
                AllowMultiple = false
            });

            var folder = result.FirstOrDefault();
            if (folder is not null)
            {
                try
                {
                    string xaml = "", code = "";

                    var items = folder.GetItemsAsync();

                    await foreach (var item in items)
                    {
                        Debug.WriteLine($"{item.Name}");
                        if (item is IStorageFile file)
                        {
                            using var stream = await file.OpenReadAsync();
                            using var reader = new StreamReader(stream);
                            var fileContent = await reader.ReadToEndAsync();

                            if (file.Name.EndsWith(".axaml")) xaml = fileContent;
                            if (file.Name.EndsWith(".cs")) code = fileContent;

                        }
                    }

                    Run(xaml, code);

                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        private async Task OpenXamlAndCodeFiles()
        {
            CurrentSample = new SampleViewModel("", "", "", null, null);
            Control = null;

            if (StorageProvider is null)
            {
                return;
            }

            var result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open xaml and code",
                FileTypeFilter = GetXamlAndCodeFileTypes(),
                AllowMultiple = true
            });

            string xaml = "", code = "";

            foreach (var item in result)
            {
                Debug.WriteLine($"{item.Name}");
                if (item is IStorageFile file)
                {
                    using var stream = await file.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var fileContent = await reader.ReadToEndAsync();

                    if (file.Name.EndsWith(".axaml")) xaml = fileContent;
                    if (file.Name.EndsWith(".cs")) code = fileContent;

                }
            }

            Run(xaml, code);
        }

        public string GetAndroidActualPath(string path)
        {
            // /tree/primary:Download/Folder -> /storage/emulated/0/Download/Folder
            if (path.Contains("primary:"))
            {
                var suffix = path.Split("primary:").Last();
                return $"/storage/emulated/0/{suffix}";
            }
            return path;
        }

        private void LoadLogo()
        {
            var img = new Image()
            {
                Source = new Bitmap(AssetLoader.Open(new Uri("avares://Vizyo.Plugin.Playground/Assets/vizyo_logo.jpg"))),
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 256,
                Height = 256
            };

            var grid = new Grid()

            {
                //Background = Brushes.AliceBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            
            grid.Children.Add(img);

            Control = grid;
        }
    }
}
