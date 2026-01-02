using Avalonia.Controls;
//using Prise.Plugin;
using Vizyo.Plugin.Shared.Contracts;

namespace SampleViewControlPlugin
{
    //[Plugin(PluginType = typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        //private IPluginHost? _host;

        //public void Initialize(IPluginHost host)
        //{
        //    _host = host;
        //}

        public UserControl Load()
        {
            //_host?.ShowMessage(GetName(), "Plugin loaded");
            return new PluginView();
        }

        public string GetName()
        {
            return "SampleViewControlPlugin";
            //return nameof(Plugin);
        }
    }
}
