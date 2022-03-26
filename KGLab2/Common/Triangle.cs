namespace KGLab2.Common; 

public class Triangle {
    public IEnumerable<float> Vertices => _vertices;
    private readonly float[] _vertices;

    public Triangle(float[] vertices) {
        if (vertices.Length != 15) {
            throw new ArgumentException("Vertices array must has 3 values");
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
        var rand = new Random();
        for (int i = 0; i < triangleWidth; i++) {
            for (int j = 0; j < triangleWidth; j++) {
                float randX = rand.NextSingle(-1f, 1f);
                yield return new Triangle( new[] {
                    x + cordStep, y,            0, tX + texStep, tY,
                    x,            y - cordStep, 0, tX,           tY + texStep,
                    x,            y,            0, tX,           tY,
                }); 
                
                randX = rand.NextSingle(-1f, 1f);

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

    public static IEnumerable<Triangle> ShuffleTriangles(IEnumerable<Triangle> triangles) {
        var random = new Random();
        
        return triangles.Select(t => {
            float[] vertices = t.Vertices.ToArray();

            for (int i = 0; i < 15; i += 5) {
                vertices[i] = random.NextSingle(-1f, 1f);
                vertices[i + 1] = random.NextSingle(-1f, 1f);
            }

            return new Triangle(vertices);
        });
    }
}