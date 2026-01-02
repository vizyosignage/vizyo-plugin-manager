using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vizyo.Plugin.Shared.Contracts;

namespace Vizyo.Plugin.Playground.Services
{
    public class PluginHost : IPluginHost
    {
        public event Action<string, string>? OnPluginMessage;

        public void ShowMessage(string pluginName, string message)
        {
            OnPluginMessage?.Invoke(pluginName, message);
        }

        public void Log(string log)
        {
            Debug.WriteLine(log);
        }
    }
}
