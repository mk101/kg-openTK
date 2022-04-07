namespace KGLab2.Common; 

public class Triangle : ICloneable {
    public IEnumerable<float> Vertices => _vertices;
    private readonly float[] _vertices;

    public Triangle(float[] vertices) {
        if (vertices.Length != 15) {
            throw new ArgumentException("Vertices array must has 15 values");
        }

        _vertices = new float[vertices.Length];
        Array.Copy(vertices, _vertices, vertices.Length);
    }

    public static float[] GetAllVertices(IEnumerable<Triangle> triangles) {
        var result = new float[15 * triangles.Count()];
        int i = 0;
        foreach (var triangle in triangles) {
            for (int j = 0; j < 15; j++) {
                result[i + j] = triangle._vertices[j];
            }
            i += 15;
        }

        return result;
    }

    public static IEnumerable<Triangle> GeneratePlane(int triangleWidth) {
        var (x, y) = (-1f, 1f);
        var (tX, tY) = (0f, 0f);
        
        float cordStep = 2f / triangleWidth;
        float texStep = 1f / triangleWidth;

        for (int i = 0; i < triangleWidth; i++) {
            for (int j = 0; j < triangleWidth; j++) {
                yield return new Triangle( new[] {
                    x + cordStep, y,            0, tX + texStep, tY,
                    x,            y - cordStep, 0, tX,           tY + texStep,
                    x,            y,            0, tX,           tY,
                });

                yield return new Triangle( new[] {
                    x + cordStep, y,            0, tX + texStep, tY,
                    x,            y - cordStep, 0, tX,           tY + texStep,
                    x + cordStep, y - cordStep, 0, tX + texStep, tY + texStep
                });


                x += cordStep;
                tX += texStep;
            }

            x = -1f;
            y -= cordStep;

            tX = 0f;
            tY += texStep;
        }
    }

    public static IEnumerable<Triangle> ShuffleTriangles(IEnumerable<Triangle> triangles, int triangleWidth) {
        var random = new Random();
        
        return triangles.Select(t => {
            float[] vertices = t.Vertices.ToArray();

            float a = random.NextSingle(-1f, 1f);
            float b = random.NextSingle(-1f, 1f);
            float cordStep = 2f / triangleWidth;

            vertices[0] = a + cordStep;
            vertices[1] = b;
            
            vertices[5] = a;
            vertices[6] = b - cordStep;
            
            vertices[10] = a;
            vertices[11] = b;

            return new Triangle(vertices);
        });
    }

    public object Clone() {
        return new Triangle(_vertices);
    }
}