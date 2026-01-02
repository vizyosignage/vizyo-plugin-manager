using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Vizyo.Plugin.Shared.Contracts;

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
        PluginApi.SendMessage(name:"SampleService", type:"info", message:"SampleService working..");
    }
}
