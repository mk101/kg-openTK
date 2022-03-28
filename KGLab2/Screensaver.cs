using KGLab2.Common;
using OpenTK.Graphics.OpenGL4;
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
    private readonly double _waitTime;
    
    private List<Triangle> _movableTriangles;
    private List<Triangle>? _triangles;
    private MoveController _moveController;
    private WaitController? _waitController;

    public Screensaver(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, IRenderer renderer)
        : base(gameWindowSettings, nativeWindowSettings) {
        _triangleWidth = 25;
        _duration = 10;
        _waitTime = 10;
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
        _triangles = null;
        if (_waitController != null) {
            _waitController.EndWaiting -= WaitControllerOnEndWaiting;
        }
        _waitController = new WaitController(_time, _waitTime);
        _waitController.EndWaiting += WaitControllerOnEndWaiting;
    }

    private void WaitControllerOnEndWaiting() {
        _moveController.EndMoving -= MoveControllerOnEndMoving;
        _movableTriangles = new List<Triangle>(Triangle.ShuffleTriangles(_finalTriangles, _triangleWidth));
        _moveController = new MoveController(_time, _finalTriangles, new List<Triangle>(_movableTriangles), _duration);
        _moveController.EndMoving += MoveControllerOnEndMoving;
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        _time += args.Time;
        if (_moveController.IsMoving) {
            _triangles = _moveController.Move(_triangles ?? _movableTriangles, _time).ToList();
        }
        _waitController?.Tick(args.Time);

        _renderer.RenderFrame(_triangles ?? _finalTriangles);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        if (KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
        
        #if DEBUG
        Title = $"Screensaver собачка | time {_time} | mov {_moveController.IsMoving} | wait {_waitController?.IsWaiting}";
        #endif
        base.OnUpdateFrame(args);
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload() {
        _moveController.EndMoving -= MoveControllerOnEndMoving;
        if (_waitController != null) {
            _waitController.EndWaiting -= WaitControllerOnEndWaiting;
        }
        _renderer.Unload();

        base.OnUnload();
    }
}