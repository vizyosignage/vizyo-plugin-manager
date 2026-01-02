using System;
using System.Collections.Generic;
using System.Text;

namespace Vizyo.Plugin.Shared.Contracts
{
    public class PluginInfo
    {
        public string Author { get; set; } = string.Empty;
        public string Home { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PluginType { get; set; } = string.Empty; // "View", "ViewModel", "ViewControl", "ViewScript", "Script"
        public string MediaType { get; set; } = string.Empty; // Text, Image, Video, Ticker, Custom etc..
        public string[] Platform { get; set; } = []; // windows, linux, android (macOS and iOS are not useful for digital signage. too expensive for a client device.)
        public string EntryFile { get; set; } = string.Empty; // DLL veya Axaml dosyasının adı
    }
}
