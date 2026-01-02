using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Vizyo.Plugin.Shared.Contracts;

namespace SampleViewModelPlugin
{
    public class PluginViewModel : INotifyPropertyChanged, IDisposable
    {
        public string PluginText { get; set; } = "Vizyo ViewModel Sample Plugin!";
        
        public event PropertyChangedEventHandler? PropertyChanged;

        private PeriodicTimer? timer;

        public PluginViewModel()
        {
            SendMessage("info", "Loaded");
            TestMethod();
        }


        private async void TestMethod()
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            int i = 1;
            while (await timer.WaitForNextTickAsync())
            {
                Dispatcher.UIThread.Post(() =>
                {
                    PluginText = "Vizyo ViewModel Sample Plugin! " + i.ToString();

                    OnPropertyChanged(nameof(PluginText));
                });

                i++;
            }
        }

        private void SendMessage(string type, string message)
        {
            var data = ("SampleViewModelPlugin", type, message);
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<(string, string, string)>(data), "PluginMessage");
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        protected bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                timer?.Dispose();
                timer = null;
                Debug.WriteLine("SampleViewModelPlugin Dispose");
            }
                
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
