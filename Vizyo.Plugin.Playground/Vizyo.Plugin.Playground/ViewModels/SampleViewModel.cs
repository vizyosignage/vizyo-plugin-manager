using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Vizyo.Plugin.Playground.ViewModels;

public partial class SampleViewModel : ViewModelBase
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _xaml;
    [ObservableProperty] private string _code;

    public SampleViewModel(string name, string xaml, string code, Action<SampleViewModel> open, Action<SampleViewModel> autoRun)
    {
        _name = name;
        _xaml  = xaml ;
        _code = code ;
        
        OpenCommand = new RelayCommand( () => open(this));
    }

    public ICommand OpenCommand { get; }
}
