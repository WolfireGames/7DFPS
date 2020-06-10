# UnityDearImgui

Unity plugin for Dear ImGui (https://github.com/ocornut/imgui).  
Based on ImGui.NET (https://github.com/mellinoe/ImGui.NET).

# Manual Unity setup

- In `PlayerSettings` -> `Other Settings` -> `Configuration` you must set `Scripting Runtime version` to `.NET 4.x Equivalent`, `Api Compatibility Level` to `.NET 4.x` and check `Allow 'unsafe' code`.
- Drag `ImGuiUnity` prefab to scene.

The setup script should perform these steps automatically.

NOTICE THAT UNITY REQUIRES YOU TO RESTART THE EDITOR FOR THE CHANGES TO TAKE EFFECT!

`Custom Fonts` should contain file names (without path) of any custom fonts you wish to use, and the actual font files placed under `/Assets/StreamingAssets` in your Unity project. `Font Pixel Size` specifies the size of custom fonts.  
`Icon Font` allows merging an icon font, see https://github.com/ocornut/imgui/tree/master/misc/fonts.  
There is a list of Texture2D:s you can use for custom cursors.

# Known issues

- Texture clipping doesn't work correctly on OpenGL, this is a Unity bug.
- Ctrl-C/Ctrl-X/Ctrl-V keys for clipboard access don't work in Unity editor.
- Game controller input support is still experimental.
- Some ImGui functions with callback function parameters are not supported.

# Android limitations

- Text input is not currently supported on Android.
- Custom and icon fonts are not currently supported on Android.
