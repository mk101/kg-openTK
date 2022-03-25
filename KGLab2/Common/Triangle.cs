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
        var result = new float[3 * triangles.Count()];
        int i = 0;
        foreach (var triangle in triangles) {
            for (int j = 0; j < 9; j++) {
                result[i + j] = triangle.Vertices[j];
            }
            i++;
        }

        return result;
    }
}