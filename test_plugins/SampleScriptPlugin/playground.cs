using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Threading;
using System;
using System.Diagnostics;

public class SampleView : UserControl, IDisposable
{
    private PeriodicTimer? timer;
    private Button button;
    private TextBlock text;

    public SampleView()
    {
        this.Loaded += (_, _) =>
        {
            Console.WriteLine("SampleView Loaded");
            text = this.Find<TextBlock>("text");
            text.Text = "SampleView Loaded";
        };
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var count = 0;
        button = this.Find<Button>("button");
        button.Click += (sender, e) => button.Content = $"Clicked: {++count}";

        timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        int i = 1;
        while (await timer.WaitForNextTickAsync())
        {
            Dispatcher.UIThread.Post(() =>
            {
                text.Text = $"Text {i}: ";
                Debug.WriteLine("timer tick");
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

    public void Dispose()
    {
        timer?.Dispose();
        timer = null;
        Debug.WriteLine("SampleView Dispose");
        GC.SuppressFinalize(this);
    }
}
