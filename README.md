# vizyo-plugin-manager
Plugin system development (trial and error, learning) for Vizyo Digital Signage application.

The goal of this project is to determine the plugin infrastructure for the Vizyo Digital Signage application.

We are considering four different plugin types, and we must ensure that they work seamlessly on Windows, Linux, Android (for client player app) and  WASM Browser (for plugin viewer via web-based host app).

1. View.axaml (.axaml file-based)
  + For custom text, image, animation, etc. plugins
  + All operations that do not require C# code
2. View.axaml + View.cs (.axaml and .cs file-based)
  + C# code support in addition to axaml
  + For example, weather plugins that retrieve data from a server
3. View.dll (compiled View.axaml + View.cs + if available, ViewModel.cs, Assets, Resources and additional .cs files)
  + For much more complex plugins
4. Service.cs (.cs script file-based)
  + For example, a plugin that periodically deletes unused files from the media folder

[What is AXAML?](https://docs.avaloniaui.net/docs/basics/user-interface/introduction-to-xaml)

Thanks

+ [Avalonia](https://github.com/AvaloniaUI/Avalonia)
+ [XamlPlayground](https://github.com/AvaloniaUI/XamlPlayground)


[![](images/vizyo_playground.png)](images/vizyo_playground.mp4)