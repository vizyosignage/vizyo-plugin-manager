using Avalonia;
using Avalonia.Controls;
using Vizyo.Plugin.Playground.ViewModels;

namespace Vizyo.Plugin.Playground.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        //protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        //{
        //    ((MainViewModel)DataContext!).StorageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
        //    base.OnAttachedToVisualTree(e);
        //}
    }
}