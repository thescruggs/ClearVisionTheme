# Taskbar color (TranslucentTB)

Windows 11 draws the taskbar background in its own XAML layer at #202020 and offers no setting darker than
that. TranslucentTB hooks Explorer to replace that background, so it can make the taskbar near-black.

1. Install:  `winget install --id CharlesMilette.TranslucentTB --exact`
2. Start TranslucentTB once from the Start menu so it creates its settings file.
3. Right-click its tray icon > Desktop > Opaque, then set the colour to `#121212`
   (or edit the settings file: tray icon > Advanced > Open settings file, and set the
   `desktop_appearance` block as in `settings-snippet.json`).
4. Tray icon > Open at boot, so it survives sign-out.

`#121212` matches the 18 18 18 surfaces used by ClearVisionTheme.theme.
