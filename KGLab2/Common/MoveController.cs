﻿using System.Linq;
using OpenTK.Mathematics;

namespace KGLab2.Common; 

public class MoveController {
    private readonly double _startTime;
    private readonly List<Triangle> _finalTriangles;
    private readonly double _duration;

    private const float Eps = 1e-9f;

    public bool IsMoving { get; private set; }
    public event Action? EndMoving;
    
    public MoveController(double startTime, List<Triangle> finalTriangles, double duration) {
        _startTime = startTime;
        _finalTriangles = finalTriangles;
        _duration = duration;
        IsMoving = true;
    }

    public IEnumerable<Triangle> Move(IEnumerable<Triangle> triangles, double curTime) {
        if (!IsMoving) {
            throw new ArgumentException("IsMoving is false");
        }

        double time = (curTime - _startTime) / _duration;

        IEnumerable<Triangle> result = triangles.Select(t => MoveTriangle(t, time));

        if (time >= 1f) {
            IsMoving = false;
            EndMoving?.Invoke();
        }

        return result;
    }

    private Triangle MoveTriangle(Triangle triangle, double time) {
        float[] vertices = triangle.Vertices.ToArray();
        var finalTriangle = _finalTriangles.Find(t => MatchTriangle(t, vertices))!;
        float[] finalVertices = finalTriangle.Vertices.ToArray();
        
        for (int i = 0; i < 15; i += 5) {
            var startVector = new Vector3(vertices[i], vertices[i + 1], 0);
            var endVector = new Vector3(finalVertices[i], finalVertices[i + 1], 0);

            var (x, y, _) = Vector3.Lerp(startVector, endVector, (float)time);
            vertices[i] = x;
            vertices[i + 1] = y;
        }

        return new Triangle(vertices);
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