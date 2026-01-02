using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class SampleService 
{
    private PeriodicTimer? timer;

    public SampleService()
    {
        Debug.WriteLine("SampleService loaded");

        timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        DoWork();
        
    }

    private async void DoWork()
    {
        while (await timer.WaitForNextTickAsync())
        {
            SendMessage("info", "SampleService working..");
        }
    }

    private void SendMessage(string type, string message)
    {
        var data = ("SampleService", type, message);
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<(string, string, string)>(data), "PluginMessage");
    }
}
