using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vizyo.Plugin.Shared.Contracts
{
    public interface IPluginHost
    {
        void ShowMessage(string pluginName, string message);

        void Log(string log);
    }
}
