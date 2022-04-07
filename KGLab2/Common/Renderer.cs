using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace KGLab2.Common; 

public sealed class Renderer : IRenderer {
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    
    private Shader? _shader;
    private Texture? _texture;
    private float[]? _vertices;

    private IEnumerable<Triangle>? _bufTriangles;

    public void Load(IEnumerable<Triangle> triangles) {
        GL.ClearColor(0f, 0f, 0f, 1f);
        
        //GL.Enable(EnableCap.DepthTest);
        
        BindVertexes(triangles);

        ApplyTexture();
    }

    private void ApplyTexture() {
        _shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/lighting.frag");
        _shader.Use();

        int vertexLocation = _shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        int texCordLocation = _shader.GetAttribLocation("aTexCord");
        GL.EnableVertexAttribArray(texCordLocation);
        GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
            3 * sizeof(float));
        
        _shader.SetVector3("light.position", new Vector3(0,0,-3f));
        _shader.SetVector3("light.direction", new Vector3(0,0, 1));
        _shader.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
        _shader.SetFloat("light.outerCutOff",  MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
        _shader.SetVector3("light.ambient", new Vector3(0.1f,0.1f, 0.1f));

        _texture = Texture.LoadFromFile("../../../Resources/sobachka.jpg", false);
        _texture.Use(TextureUnit.Texture0);
    }

    private void BindVertexes(IEnumerable<Triangle> triangles) {
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

        _vertices = Triangle.GetAllVertices(triangles);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        //GL.EnableVertexAttribArray(0);
    }

    public void RenderFrame(IEnumerable<Triangle> triangles) {
        GL.Clear(ClearBufferMask.ColorBufferBit /*| ClearBufferMask.DepthBufferBit*/);

        if (!Equals(triangles, _bufTriangles)) {
            _bufTriangles = triangles;
            _vertices = Triangle.GetAllVertices(triangles);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);
        }

        GL.BindVertexArray(_vertexArrayObject);
        
        _texture!.Use(TextureUnit.Texture0);
        _shader!.Use();

        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices!.Length / 5);
    }

    public void Unload() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);

        GL.DeleteProgram(_shader!.Handle);
        GL.DeleteTexture(_texture!.Handle);
    }
}