using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Vizyo.Plugin.Playground;
using Vizyo.Plugin.Playground.Services;

[assembly: SupportedOSPlatform("browser")]
internal sealed partial class Program
{
    private static void Initialize(string baseUri)
    {
        CompilerService.BaseUri = baseUri;
    }
    private static Task Main(string[] args) => BuildAvaloniaApp()
            .WithInterFont()
            .AfterSetup(_ => Initialize(args[0]))
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}