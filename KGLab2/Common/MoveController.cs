using System.Linq;
using OpenTK.Mathematics;

namespace KGLab2.Common; 

public class MoveController {
    private double _startTime;
    private readonly List<Triangle> _startTriangles;
    private readonly List<Triangle> _finalTriangles;
    private readonly double _duration;

    private const float Eps = 1e-9f;

    public bool IsMoving { get; private set; }
    public event Action? EndMoving;
    
    public MoveController(double startTime, List<Triangle> finalTriangles, List<Triangle> startTriangles, double duration) {
        _startTime = startTime;
        _finalTriangles = finalTriangles;
        _startTriangles = startTriangles;
        _duration = duration;
        IsMoving = true;
    }

    public void Start(double startTime) {
        IsMoving = true;
        _startTime = startTime;
    }

    public void Stop() {
        IsMoving = false;
        EndMoving?.Invoke();
    }

    public IEnumerable<Triangle> Move(IEnumerable<Triangle> triangles, double curTime) {
        if (!IsMoving) {
            throw new ArgumentException("IsMoving is false");
        }

        double time = (curTime - _startTime) / _duration;

        IEnumerable<Triangle> result = triangles.Select(t => MoveTriangle(t, time));

        if (time >= 1f) {
            Stop();
        }

        return result;
    }

    private Triangle MoveTriangle(Triangle triangle, double time) {
        float[] vertices = (float[])triangle.Vertices;
        Triangle startTriangle = _startTriangles.Find(t => MatchTriangle(t, vertices))!;
        Triangle finalTriangle = _finalTriangles.Find(t => MatchTriangle(t, vertices))!;

        float[] startVertices = (float[]) startTriangle.Vertices;
        float[] finalVertices = (float[])finalTriangle.Vertices;
        
        for (int i = 0; i < 15; i += 5) {
            var startVector = new Vector3(startVertices[i], startVertices[i + 1], 0);
            var endVector = new Vector3(finalVertices[i], finalVertices[i + 1], 0);

            var (x, y, _) = Vector3.Lerp(startVector, endVector, (float)time);
            vertices[i] = x;
            vertices[i + 1] = y;
        }

        return triangle;
    }

    private bool MatchTriangle(Triangle t, float[] matchVertices) {
        float[] vertices = (float[]) t.Vertices;

        return Math.Abs(vertices[3] - matchVertices[3]) < Eps
               && Math.Abs(vertices[4] - matchVertices[4]) < Eps
               && Math.Abs(vertices[8] - matchVertices[8]) < Eps
               && Math.Abs(vertices[9] - matchVertices[9]) < Eps
               && Math.Abs(vertices[13] - matchVertices[13]) < Eps
               && Math.Abs(vertices[14] - matchVertices[14]) < Eps;
    }
}