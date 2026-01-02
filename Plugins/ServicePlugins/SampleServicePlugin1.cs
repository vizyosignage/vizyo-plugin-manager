using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class SampleService : BackgroundService
{
   
    public SampleService()
    {
        Debug.WriteLine("SampleService loaded");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                SendMessage("info", "SampleService working..");
                //await DoWork();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            await Task.Delay(10000, stoppingToken);
        }

        Debug.WriteLine("SampleService stopping..");
    }

    private Task DoWork()
    {
        return Task.CompletedTask;
    }

    private void SendMessage(string type, string message)
    {
        var data = ("SampleService", type, message);
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<(string, string, string)>(data), "PluginMessage");
    }
}
