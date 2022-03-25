namespace KGLab2.Common; 

public class Triangle {
    public float[] Vertices { get; } //TODO: find save method

    public Triangle(float[] vertices) {
        if (vertices.Length != 9) {
            throw new ArgumentException("Vertices array must has 3 values");
        }
        Vertices = vertices;
    }

    public static float[] GetAllVertices(IEnumerable<Triangle> triangles) {
        var result = new float[9 * triangles.Count()];
        int i = 0;
        foreach (var triangle in triangles) {
            for (int j = 0; j < 9; j++) {
                result[i + j] = triangle.Vertices[j];
            }
            i += 9;
        }

        return result;
    }

    public static IEnumerable<Triangle> GeneratePlane(int triangleWidth) {
        var (x, y) = (-1f, 1f);
        float step = 2f / triangleWidth;
        
        for (int i = 0; i < triangleWidth; i++) {
            for (int j = 0; j < triangleWidth; j++) {
                yield return new Triangle( new[] {
                    x,        y,        0,
                    x + step, y,        0,
                    x,        y - step, 0
                });
                
                yield return new Triangle( new[] {
                    x + step, y,        0,
                    x,        y - step, 0,
                    x + step, y - step, 0
                });
                
                x += step;
            }

            x = -1f;
            y -= step;
        }
    }
}