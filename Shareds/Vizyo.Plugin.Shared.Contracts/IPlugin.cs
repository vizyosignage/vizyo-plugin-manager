using System;
using Avalonia.Controls;

namespace Vizyo.Plugin.Shared.Contracts
{
    public interface IPlugin
    {
        //void Initialize(IPluginHost host);
        string GetName();
        UserControl Load();
    }
}
