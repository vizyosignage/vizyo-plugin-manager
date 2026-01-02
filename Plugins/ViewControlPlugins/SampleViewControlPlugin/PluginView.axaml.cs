using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Vizyo.Plugin.Shared.Contracts;

namespace SampleViewControlPlugin;

public partial class PluginView : UserControl, IDisposable
{
    private PeriodicTimer? timer;
    public PluginView()
    {
        InitializeComponent();

        SetControls();

        TestMethod();

        this.Loaded += (_, _) =>
        {
            Debug.WriteLine("SampleViewControlPlugin Loaded");
            PluginApi.SendMessage("SampleViewControlPlugin", "info", "Loaded");
        };

        this.Unloaded += (_, _) =>
        {
            Debug.WriteLine("SampleViewControlPlugin Unloaded");
        };
    }

    private void SetControls()
    {
        MainText.Text = "Vizyo ViewControl Sample Plugin!";

        var pathToThisProgram = Assembly.GetExecutingAssembly() // this assembly location (/bin/Debug/net9.0)
                .Location;

        //var pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //Debug.WriteLine(pluginPath);
        //var imagePath = Path.Combine(pluginPath, "Assets", "vizyo_logo1.png");
        //MainImage.Source = new Bitmap(imagePath);

        //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "vizyo_logo1.png");
        //MainImage.Source = new Bitmap(imagePath);

        //var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //var imagePath = Path.Combine(appDirectory, "Assets", "vizyo_logo1.png");
        //MainImage.Source = new Bitmap(imagePath);
    }

    private async void TestMethod()
    {
        timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        int i = 1;
        while (await timer.WaitForNextTickAsync())
        {
            Dispatcher.UIThread.Post(() =>
            {
                MainText.Text = "Vizyo ViewControl Sample Plugin! " + i.ToString();
                //Debug.WriteLine("timer tick");
            });

            i++;
        }
    }

    //private void SendMessage(string type, string message)
    //{
    //    var data = ("SampleViewControlPlugin", type, message);
    //    WeakReferenceMessenger.Default.Send(new ValueChangedMessage<(string, string, string)>(data), "PluginMessage");
    //}

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        Dispose();
    }

    protected bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed && disposing)
        {
            timer?.Dispose();
            timer = null;
            Debug.WriteLine("SampleViewControlPlugin Dispose");
        }

        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}