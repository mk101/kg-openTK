using KGLab2;
using KGLab2.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

var nativeWindowSettings = new NativeWindowSettings() {
    Size = new Vector2i(774, 774),
    Title = "Screensaver собачка",
};

using var screensaver = new Screensaver(GameWindowSettings.Default, nativeWindowSettings, new Renderer());
screensaver.Run();