using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace Vizyo.Plugin.Playground
{
    public static class AvaloniaServiceCollectionExtensions
    {
        public static AppBuilder ConfigureServices(this AppBuilder app, Action<IServiceCollection> configureServices)
        {
            var services = new ServiceCollection();
            configureServices.Invoke(services);

            AppServiceLocator.Configure(services.BuildServiceProvider());

            return app;
        }
    }
}
