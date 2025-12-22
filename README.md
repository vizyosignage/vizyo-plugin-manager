# vizyo-plugin-manager
Plugin system development for Vizyo Digital Signage application.

The goal of this project is to determine the plugin infrastructure for the Vizyo Digital Signage application.

We are considering three different plugin types, and we must ensure that they work seamlessly on Windows, Linux, Android and  WASM Browser.

1. View.axaml (.axaml file-based)
* For custom text, image, animation, etc. plugins
* All operations that do not require C# code
2. View.axaml + View.cs (.axaml and .cs file-based)
* C# code support in addition to axaml
* For example, weather plugins that retrieve data from a server
3. View.dll (compiled View.axaml + View.cs + if available, ViewModel.cs, Assets, Resources and additional .cs files)
* For much more complex plugins

Thanks

+ [Avalonia](https://github.com/AvaloniaUI/Avalonia)
+ [XamlPlayground](https://github.com/AvaloniaUI/XamlPlayground)
+ [Prise](https://github.com/merken/Prise)
