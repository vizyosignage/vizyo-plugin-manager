using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Vizyo.Plugin.Shared.Contracts
{
    public static class PluginApi
    {
        public static void SendMessage(string name, string type, string message)
        {
            var data = (name, type, message);
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<(string, string, string)>(data), "PluginMessage");

            //WeakReferenceMessenger.Default.Send(new ValueChangedMessage<PluginMessage>(new PluginMessage { PluginName = name, MessageType = type, Message = message }));
        }
    }

    public class PluginMessage
    {
        public string PluginName { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
    }
}
