using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using Prise.DependencyInjection;
using Prise.Proxy;
using Vizyo.Plugin.Playground.Infrastructure;
using Vizyo.Plugin.Playground.ViewModels;
using Vizyo.Plugin.Playground.Views;

namespace Vizyo.Plugin.Playground
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        //public override void OnFrameworkInitializationCompleted()
        //{
        //    var services = new ServiceCollection();
        //    services
        //       .AddPrise()
        //       .AddFactory<IResultConverter>(() => new AvaloniaPluginResultConverter()
        //   );

        //    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        //    {
        //        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        //        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        //        DisableAvaloniaDataAnnotationValidation();
        //        desktop.MainWindow = new MainWindow
        //        {
        //            DataContext = new MainViewModel()
        //        };
        //    }
        //    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        //    {
        //        singleViewPlatform.MainView = new MainView
        //        {
        //            DataContext = new MainViewModel()
        //        };
        //    }

        //    base.OnFrameworkInitializationCompleted();
        //}

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            DisableAvaloniaDataAnnotationValidation();
            //services.AddApplication<ManagerDomainSharedModule>();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                var topLevel = TopLevel.GetTopLevel(mainWindow);

                //services.AddNotificationServices(topLevel);

                //services.AddSingleton<LocalizationService>();
                services.AddTransient<MainViewModel>();
                services
                   .AddPrise()
                   .AddFactory<IResultConverter>(() => new AvaloniaPluginResultConverter()
               );

                var provider = services.BuildServiceProvider();
                AppServiceLocator.Configure(provider);
                //App.Services = provider;

                //var localizationService = provider.GetRequiredService<LocalizationService>();
                //var culture = localizationService.GetSavedCultureOrSystem();
                //localizationService.SetCulture("en");

                var vm = provider.GetRequiredService<MainViewModel>();
                vm.StorageProvider = topLevel?.StorageProvider;
                mainWindow.DataContext = vm;

                desktop.MainWindow = mainWindow;
                //vm.ShowSuccessToast("desktop");
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                var mainView = new MainView();
                singleViewPlatform.MainView = mainView;

                var topLevel = TopLevel.GetTopLevel(mainView);
                //services.AddNotificationServices(topLevel);

                //services.AddSingleton<LocalizationService>();
                services.AddTransient<MainViewModel>();
                services
                   .AddPrise()
                   .AddFactory<IResultConverter>(() => new AvaloniaPluginResultConverter()
               );

                var provider = services.BuildServiceProvider();
                AppServiceLocator.Configure(provider);
                //App.Services = provider;

                //var localizationService = provider.GetRequiredService<LocalizationService>();
                //var culture = localizationService.GetSavedCultureOrSystem();
                //localizationService.SetCulture("en");

                var vm = provider.GetRequiredService<MainViewModel>();
                vm.StorageProvider = topLevel?.StorageProvider;
                singleViewPlatform.MainView.DataContext = vm;
                //vm.ShowSuccessToast("mobile");
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}