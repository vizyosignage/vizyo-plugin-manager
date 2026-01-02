using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class SampleView : UserControl, IDisposable
{
    private PeriodicTimer? timer1;
    private DispatcherTimer? timer2;
    private Button button;
    private TextBlock text;

    public SampleView()
    {
        this.Loaded += (_, _) =>
        {
            Debug.WriteLine("SampleView Loaded");
            text = this.Find<TextBlock>("text");
            text.Text = "SampleView Loaded";

            timer2 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer2.Tick += (s, e) =>
            {
                Debug.WriteLine("timer2 tick");
            };
            timer2.Start();

            SendMessage("info", "Loaded");
        };

        this.Unloaded += (_, _) =>
        {
            Debug.WriteLine("SampleView Unloaded");
        };
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var count = 0;
        button = this.Find<Button>("button");
        button.Click += (sender, e) => button.Content = $"Clicked: {++count}";

        timer1 = new PeriodicTimer(TimeSpan.FromSeconds(1));
        int i = 1;
        while (await timer1.WaitForNextTickAsync())
        {
            Dispatcher.UIThread.Post(() =>
            {
                text.Text = $"Text {i}: ";
                Debug.WriteLine("timer1 tick");
            });

            i++;

            //break;
        }

        Debug.WriteLine("stopped");
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispose();
        base.OnDetachedFromVisualTree(e);
    }

    private void SendMessage(string type, string message)
    {
        var data = ("SampleView", type, message);
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<(string, string, string)>(data), "PluginMessage");
    }

    public void Dispose()
    {
        timer1?.Dispose();
        timer1 = null;
        timer2?.Stop();
        timer2 = null;
        
        Debug.WriteLine("SampleView Dispose");
        GC.SuppressFinalize(this);
    }
}
