# TAS

This is a [BepInEx 5] plugin which plays the game all by itself in the fastest way possible.

[BepInEx 5]: https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21

## Installation

Copy these game files from `identity_Data\Managed` into the `./lib/` folder:
- `Assembly-CSharp.dll`
- `Unity.InputSystem.dll`
- `UnityEngine.CoreModule.dll`
- `UnityEngine.UIModule.dll`

Run `.\build.ps1`.

All output binaries should go into a BepInEx plugin folder.
Example folder: `BepInEx\plugins\Ident.TAS`.

It's recommended to create a softlink to the output directory with Powershell/CMD (requires admin):

```ps
cmd /c mklink /d "C:\Program Files (x86)\Steam\steamapps\common\Tron Identity\BepInEx\plugins\Ident.TAS" %cd%\bin\Release\net48
```

## Used tools

- [dnSpy]
- [RuntimeUnityEditor]

[dnSpy]: https://github.com/dnSpy/dnSpy/releases
[RuntimeUnityEditor]: https://github.com/ManlyMarco/RuntimeUnityEditor/releases
