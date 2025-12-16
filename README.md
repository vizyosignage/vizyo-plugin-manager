# vizyo-plugin-manager
Plugin system development for Vizyo Digital Signage application.

The aim of this project is to determine the plugin infrastructure for the Vizyo Digital Signage application.

We are considering 3 different models:
1. View.axaml (.axaml file-based)
* For custom text, image, animation, etc. plugins
* All operations that do not require C# code
2. View.axaml + View.cs (.axaml and .cs file-based)
* C# code support in addition to axaml
* For example, weather plugins that retrieve data from a server
3. ViewModel.dll (compiled axaml + view.cs + model.cs if available + additional .cs files if available)
* For much more complex plugins
