using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace SampleViewModelPlugin;

public partial class PluginView : UserControl, IDisposable
{
    private readonly PluginViewModel _vm;
    public PluginView()
    {
        InitializeComponent();
        _vm = new PluginViewModel();
        DataContext = _vm;

        _vm.PropertyChanged += (_, e) =>
        {
            //Dispatcher.UIThread.Post(() =>
            //{
                Debug.WriteLine(e.PropertyName);
            //});
        };
    }

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
            _vm.Dispose();
        }

        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}