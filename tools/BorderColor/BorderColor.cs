// BorderColor - sets the Windows 11 one-pixel window border color for every top-level window.
// Usage: BorderColor.exe [activeRRGGBB] [inactiveRRGGBB]   (defaults: 8C8C8C and 5A5A5A)
//        BorderColor.exe --reset                            (restore Windows default borders)
// Uses DwmSetWindowAttribute(DWMWA_BORDER_COLOR). Runs hidden; stop it with: taskkill /im BorderColor.exe
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

static class BorderColor {
    [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref uint val, int size);
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumProc cb, IntPtr lp);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] static extern IntPtr GetAncestor(IntPtr h, uint flags);
    [DllImport("user32.dll")] static extern IntPtr SetWinEventHook(uint min, uint max, IntPtr mod, WinEventProc cb, uint pid, uint tid, uint flags);
    [DllImport("user32.dll")] static extern bool UnhookWinEvent(IntPtr h);
    delegate bool EnumProc(IntPtr h, IntPtr lp);
    delegate void WinEventProc(IntPtr hook, uint ev, IntPtr hwnd, int idObject, int idChild, uint tid, uint time);

    const int  DWMWA_BORDER_COLOR = 34;
    const uint DWMWA_COLOR_DEFAULT = 0xFFFFFFFF;
    const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    const uint EVENT_OBJECT_CREATE = 0x8000;
    const uint EVENT_OBJECT_SHOW = 0x8002;
    const uint GA_ROOT = 2;

    static uint active, inactive;
    static IntPtr lastFg = IntPtr.Zero;
    static WinEventProc hookProc;   // kept referenced so the GC never collects the callback
    static EnumProc sweepProc, resetProc;

    static uint Parse(string hex) {           // "RRGGBB" -> COLORREF (0x00BBGGRR)
        uint rgb = Convert.ToUInt32(hex.TrimStart('#'), 16);
        return ((rgb >> 16) & 0xFF) | (((rgb >> 8) & 0xFF) << 8) | ((rgb & 0xFF) << 16);
    }
    static void Apply(IntPtr h, uint c) { if (h == IntPtr.Zero) return; uint v = c; DwmSetWindowAttribute(h, DWMWA_BORDER_COLOR, ref v, 4); }
    static void Sweep() {
        IntPtr fg = GetForegroundWindow();
        sweepProc = (h, lp) => { if (IsWindowVisible(h)) Apply(h, h == fg ? active : inactive); return true; };
        EnumWindows(sweepProc, IntPtr.Zero);
        lastFg = fg;
    }
    static void Reset() {
        resetProc = (h, lp) => { if (IsWindowVisible(h)) Apply(h, DWMWA_COLOR_DEFAULT); return true; };
        EnumWindows(resetProc, IntPtr.Zero);
    }

    [STAThread]
    static void Main(string[] args) {
        if (args.Length > 0 && args[0] == "--reset") { Reset(); return; }
        bool created; var mutex = new Mutex(true, "ClearVisionBorderColor", out created);
        if (!created) return;                 // already running
        active   = Parse(args.Length > 0 ? args[0] : "8C8C8C");
        inactive = Parse(args.Length > 1 ? args[1] : "5A5A5A");
        Sweep();
        hookProc = (hook, ev, hwnd, idObject, idChild, tid, time) => {
            if (idObject != 0 || hwnd == IntPtr.Zero) return;
            if (ev == EVENT_SYSTEM_FOREGROUND) {
                if (lastFg != IntPtr.Zero && lastFg != hwnd) Apply(lastFg, inactive);
                Apply(hwnd, active); lastFg = hwnd;
            } else if (ev == EVENT_OBJECT_SHOW || ev == EVENT_OBJECT_CREATE) {
                if (GetAncestor(hwnd, GA_ROOT) == hwnd) Apply(hwnd, hwnd == GetForegroundWindow() ? active : inactive);
            }
        };
        IntPtr h1 = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, hookProc, 0, 0, 0);
        IntPtr h2 = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_SHOW, IntPtr.Zero, hookProc, 0, 0, 0);
        var timer = new System.Windows.Forms.Timer { Interval = 3000 };   // fallback sweep for anything the hooks miss
        timer.Tick += (s, e) => Sweep();
        timer.Start();
        Application.ApplicationExit += (s, e) => { UnhookWinEvent(h1); UnhookWinEvent(h2); };
        Application.Run();
        GC.KeepAlive(mutex);
    }
}
