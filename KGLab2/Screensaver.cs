using KGLab2.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KGLab2; 

public sealed class Screensaver : GameWindow {
    private readonly List<Triangle> _finalTriangles;
    private double _time;
    private readonly IRenderer _renderer;

    private List<Triangle> _movableTriangles;
    private MoveController _moveController;

    public Screensaver(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, IRenderer renderer)
        : base(gameWindowSettings, nativeWindowSettings) {
        _finalTriangles = new List<Triangle>(Triangle.GeneratePlane(10));
        _movableTriangles = new List<Triangle>(Triangle.ShuffleTriangles(_finalTriangles));
        _renderer = renderer;
    }

    protected override void OnLoad() {
        base.OnLoad();

        _moveController = new MoveController(0.0, _finalTriangles, 10000.0);
        _moveController.EndMoving += MoveControllerOnEndMoving;
        
        _renderer.Load(_movableTriangles);
    }

    private void MoveControllerOnEndMoving() {
        _movableTriangles.Clear();
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        _time += args.Time;

        IEnumerable<Triangle>? triangles = _finalTriangles;
        if (_moveController.IsMoving) {
            triangles = _movableTriangles;
        }
        _renderer.RenderFrame(triangles);

        if (_moveController.IsMoving) {
            _movableTriangles = new List<Triangle>(_moveController.Move(_movableTriangles, _time));
        }

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        if (KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
        
        base.OnUpdateFrame(args);
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload() {
        _moveController.EndMoving -= MoveControllerOnEndMoving;
        _renderer.Unload();

        base.OnUnload();
    }
}