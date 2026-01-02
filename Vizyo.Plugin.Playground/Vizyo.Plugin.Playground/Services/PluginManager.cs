using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Vizyo.Plugin.Shared.Contracts;

namespace Vizyo.Plugin.Playground.Services
{
    public class PluginManager
    {
        //private readonly IPluginHost _pluginHost;

        //public PluginManager(IPluginHost pluginHost)
        //{
        //    _pluginHost = pluginHost;
        //}

        public PluginManager()
        {
        }

        public UserControl? GetViewTestPlugin()
        {
            //var dataContext = new { Text = "Deneme 123", Foreground = Brush.Parse("#fff"), Background = "Black", FontFamily = SetFontFamily("Times New Roman"), FontSize = 32, Opacity = 0.5d };

            //string json = """{ "Text" : "Deneme 123", "Foreground": "White", "Background" : "Black", "FontFamily" : "Times New Roman", "FontSize" : "32", "Opacity" : "0,5" }""";
            //string json = """{ "Text" : "Deneme 123", "FontSize" : 12}""";
            //var dataContext = JsonSerializer.Deserialize<TextBlock>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //string strContext = JsonSerializer.Serialize(dataContext, new JsonSerializerOptions { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals });
            //Debug.WriteLine(strContext);

            //GetHttp();

            //var xaml = File.ReadAllText("/SampleAnimatedViewPlugin/SampleAnimatedViewPlugin4.axaml");
            //var control = AvaloniaRuntimeXamlLoader.Parse<UserControl>(xaml);

            using var fs = File.OpenRead("D:\\PROJELER\\Vizyo\\vizyo-plugin-manager\\Plugins\\ViewPlugins\\SampleAnimatedViewPlugin\\SampleAnimatedViewPlugin4.axaml");
            var control = AvaloniaRuntimeXamlLoader.Load(fs) as UserControl ?? null;


            if (control != null)
            {
                control.Background = Brush.Parse("#ccc");
                //control.Width = 300;
                //control.Height = 200;
                //Canvas.SetLeft(control, 100);
                //Canvas.SetTop(control, 50);

                //control.DataContext = dataContext;
                return control;
            }

            return null;
        }

        public async Task<IPlugin?> LoadPluginFromPath(string dllPath)
        {
            var assembly = Assembly.LoadFrom(dllPath);
            var type = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface);
            if (type != null)
            {
                return (IPlugin)Activator.CreateInstance(type);
            }
            return null;
        }

        public async Task<IPlugin?> LoadPluginFromUrl(string dllUrl)
        {
            using var httpClient = new HttpClient();

            byte[] assemblyBytes = await httpClient.GetByteArrayAsync(dllUrl);

            // byte[] pdbBytes = await httpClient.GetByteArrayAsync(dllUrl.Replace(".dll", ".pdb"));

            var assembly = Assembly.Load(assemblyBytes);

            var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface);

            if (pluginType != null)
            {
                return (IPlugin)Activator.CreateInstance(pluginType)!;
            }

            return null;
        }

        public async Task<IPlugin?> LoadPluginFromByteX(byte[]? assemblyData)
        {
            if (assemblyData != null)
            {
                var assembly = Assembly.Load(assemblyData);

                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface);

                if (pluginType != null)
                {
                    return (IPlugin)Activator.CreateInstance(pluginType);
                }
            }

            return null;
        }

        public async Task<Control?> LoadPluginFromByte(byte[]? assemblyData)
        {
            if (assemblyData != null)
            {
                var assembly = Assembly.Load(assemblyData);
                Debug.WriteLine("assembly: " + assembly.FullName);

                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface);

                if (pluginType != null)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;
                    Debug.WriteLine("plugin: " + plugin.GetName());

                    //plugin.Initialize(_pluginHost);

                    return plugin.Load();
                }
            }

            return null;
        }

        private async void GetHttp()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // browser için test
                    var content = await client.GetStringAsync("/SampleAnimatedViewPlugin4.axaml");
                    Debug.WriteLine(content);
                }
                catch (HttpRequestException e)
                {
                    Debug.WriteLine($"Hata oluştu: {e.Message}");
                }
            }
        }

        private IBrush SetBrush(string hex)
        {
            return Brush.Parse(hex);
        }

        private FontFamily SetFontFamily(string fontName)
        {
            return new FontFamily(fontName);
            //TextBlock.FontFamily = new FontFamily(@"avares://AvaloniaApplication/Assets/Fonts#Nunito"); // For a resource-based font
            //TextBlock.FontFamily = new FontFamily("Times New Roman"); // For a system font

            // https://github.com/AvaloniaUI/Avalonia/discussions/17199
        }
    }
}
