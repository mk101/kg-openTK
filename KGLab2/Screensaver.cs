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
    private readonly double _duration;
    private readonly int _triangleWidth;

    private List<Triangle> _movableTriangles;
    private MoveController _moveController;

    public Screensaver(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, IRenderer renderer)
        : base(gameWindowSettings, nativeWindowSettings) {
        _triangleWidth = 25;
        _duration = 10;
        _finalTriangles = new List<Triangle>(Triangle.GeneratePlane(_triangleWidth));
        _movableTriangles = new List<Triangle>(Triangle.ShuffleTriangles(_finalTriangles, _triangleWidth));
        _renderer = renderer;

        _moveController = new MoveController(0, _finalTriangles, new List<Triangle>(_movableTriangles), _duration);
        _moveController.EndMoving += MoveControllerOnEndMoving;
    }

    protected override void OnLoad() {
        base.OnLoad();

        VSync = VSyncMode.On;

        _renderer.Load(_movableTriangles);
    }

    private void MoveControllerOnEndMoving() {
        _moveController.EndMoving -= MoveControllerOnEndMoving;
        _movableTriangles = new List<Triangle>(Triangle.ShuffleTriangles(_finalTriangles, _triangleWidth));
        _moveController = new MoveController(_time, _finalTriangles, new List<Triangle>(_movableTriangles), _duration);
        _moveController.EndMoving += MoveControllerOnEndMoving;
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        _time += args.Time;

        IEnumerable<Triangle> triangles = _finalTriangles;
        if (_moveController.IsMoving) {
            triangles = _movableTriangles;
        }
        _renderer.RenderFrame(triangles);

        if (_moveController.IsMoving) {
            _movableTriangles = _moveController.Move(_movableTriangles, _time).ToList();
        }

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        if (KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
        
        #if DEBUG
        Title = $"Screensaver собачка | time {_time} | ct {(_time % 10) / 10.0}";
        #endif
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