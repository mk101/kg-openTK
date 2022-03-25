using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KGLab2; 

public sealed class Screensaver : GameWindow {
    public Screensaver(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) 
        : base(gameWindowSettings, nativeWindowSettings) { }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        if (KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
        
        base.OnUpdateFrame(args);
    }
}