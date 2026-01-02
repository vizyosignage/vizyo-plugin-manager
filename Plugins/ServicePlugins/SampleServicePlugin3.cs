using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class SampleService 
{
    private System.Timers.Timer? timer;

    public SampleService()
    {
        Debug.WriteLine("SampleService loaded");
        Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

        timer = new System.Timers.Timer(5000);
        timer.AutoReset = true;
        timer.Elapsed += (s, e) =>
        {
            DoWork();
        };
        timer.Start();
    }

    private async void DoWork()
    {
        SendMessage("info", "SampleService working..");
    }

    private void SendMessage(string type, string message)
    {
        var data = ("SampleService", type, message);
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<(string, string, string)>(data), "PluginMessage");
    }
}
