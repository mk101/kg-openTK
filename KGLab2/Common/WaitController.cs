namespace KGLab2.Common; 

public class WaitController {
    private double _currentTime;
    private readonly double _finalTime;
    
    public bool IsWaiting { get; private set; }
    
    public event Action? EndWaiting;
    
    public WaitController(double startTime, double time) {
        _currentTime = startTime;
        _finalTime = startTime + time;
        IsWaiting = true;
    }

    public void Tick(double delta) {
        if (!IsWaiting) {
            return;
        }

        _currentTime += delta;
        if (_currentTime >= _finalTime) {
            IsWaiting = false;
            EndWaiting?.Invoke();
        }
    }
}