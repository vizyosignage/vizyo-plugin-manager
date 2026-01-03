using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prise.Proxy;

namespace Vizyo.Plugin.Playground.Converters
{
    public class AvaloniaPluginResultConverter : ResultConverter
    {
        public override object Deserialize(Type localType, Type remoteType, object value)
        {
            // No conversion, no backwards compatibility
            // When the host upgrades any Avalonia dependency, it will break
            return value;
        }
    }
}
