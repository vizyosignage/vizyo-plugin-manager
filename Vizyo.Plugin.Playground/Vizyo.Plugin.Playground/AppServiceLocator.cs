using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Vizyo.Plugin.Playground
{
    public static class AppServiceLocator
    {
        private static bool isConfigured;
        private static IServiceProvider internalServiceProvider;

        internal static void Configure(IServiceProvider serviceProvider)
        {
            if (isConfigured)
                throw new NotSupportedException("This AppServiceLocator is already configured");

            internalServiceProvider = serviceProvider;
            isConfigured = true;
        }

        public static T GetService<T>() => internalServiceProvider.GetRequiredService<T>();
    }
}
