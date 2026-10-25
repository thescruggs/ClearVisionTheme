# ClearVisionTheme

Two dark Windows 11 themes plus a small tool that sets the window border colour. Grew out of
[GreyEveTheme](https://github.com/nitschis/GreyEveTheme) by eversins, whose high contrast trick this keeps.

| File | What it is |
|---|---|
| `ClearVisionDark.theme` | Regular dark theme: Aero styles, dark mode flags, dark blue accent, black desktop. Normal Explorer selection (blue bar, white text). |
| `ClearVisionTheme.theme` | High contrast variant: near-black (18 18 18) surfaces forced on every app, grey outlines, blue selection. |
| `tools/BorderColor` | Sets the one-pixel border of every window (light grey active, darker inactive). Works with either theme. |
| `*.deskthemepack` | The same themes packed as CAB archives; `build.ps1` regenerates them. |

Double-click a `.theme` file to apply it. Switch back with any stock theme in Settings > Personalization.

## Install (Windows)

```powershell
.\install.ps1                      # compile + start BorderColor at sign-in, apply ClearVisionDark
.\install.ps1 -Theme HighContrast  # apply the high contrast variant instead
.\install.ps1 -Theme None          # border tool only
.\tools\BorderColor\uninstall.ps1  # stop the tool, remove it from startup, default borders back
```

BorderColor is compiled on the spot with the C# compiler that ships with Windows
(`%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe`), so there is no binary in the repo.

## How a .theme works

A `.theme` is a plain INI file. There is nothing to compile: `[VisualStyles]` picks the `.msstyles`
and the dark mode flags, `[Control Panel\Colors]` sets the classic system colours. The high contrast
variant points at the stock `AeroLite.msstyles` with `HighContrast=3`, which is what makes those
colours apply everywhere.

## High contrast colour keys, measured on Windows 11

These took pixel probes to find; nothing documents them.

| Key | What it actually draws |
|---|---|
| `TitleText` | Outline of the active window (and its caption text) |
| `InactiveTitleText` | Outline of inactive windows |
| `ButtonText` | Most text in Explorer, control outlines, **and the fill of a focused selected row** |
| `WindowText` | Secondary text (nav pane items), taskbar top line |
| `Hilight` / `HilightText` | Unfocused selection bar / text on any selection |
| `HotTrackingColor` | Hover outline and hyperlinks |
| `SystemMode=Dark`, `AppMode=Dark` | Under `[VisualStyles]`; without them the theme does not register as dark mode |

The `ButtonText` double duty is why a high contrast theme cannot have a dark selection bar with light
text: the focused bar is always the same colour as the text. `ClearVisionDark` exists because of that.

## Window borders

Neither theme type can set the border colour. The DWM border attribute can, from any process, even
under high contrast, so `BorderColor.exe` applies it to every top-level window and re-applies on
new and activated windows. The taskbar is a separate story: Windows 11 paints it at #202020 in its
own layer, and only an Explorer-hooking tool such as TranslucentTB can take it darker
(see `tools/TranslucentTB`).
