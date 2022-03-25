using KGLab2.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KGLab2; 

public sealed class Screensaver : GameWindow {
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private readonly Triangle _triangle;
    private Shader _shader;

    public Screensaver(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings) {
        _triangle = new Triangle(new[] {
            -1f, -1f, 0.0f, // Bottom-left vertex
             1f, -1f, 0.0f, // Bottom-right vertex
             0f,  1f, 0.0f  // Top vertex
        });
    }

    protected override void OnLoad() {
        base.OnLoad();
        
        GL.ClearColor(1f, 1f, 1f, 1f);

        // We need to send our vertices over to the graphics card so OpenGL can use them.
        // To do this, we need to create what's called a Vertex Buffer Object (VBO).
        // These allow you to upload a bunch of data to a buffer, and send the buffer to the graphics card.
        // This effectively sends all the vertices at the same time.

        // First, we need to create a buffer. This function returns a handle to it, but as of right now, it's empty.
        _vertexBufferObject = GL.GenBuffer();
        // Now, bind the buffer. OpenGL uses one global state, so after calling this,
        // all future calls that modify the VBO will be applied to this buffer until another buffer is bound instead.
        // The first argument is an enum, specifying what type of buffer we're binding. A VBO is an ArrayBuffer.
        // There are multiple types of buffers, but for now, only the VBO is necessary.
        // The second argument is the handle to our buffer.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        // Finally, upload the vertices to the buffer.
        GL.BufferData(BufferTarget.ArrayBuffer, _triangle.Vertices.Length * sizeof(float), _triangle.Vertices, BufferUsageHint.StaticDraw);
        
        // One notable thing about the buffer we just loaded data into is that it doesn't have any structure to it. It's just a bunch of floats (which are actaully just bytes).
        // The opengl driver doesn't know how this data should be interpreted or how it should be divided up into vertices. To do this opengl introduces the idea of a 
        // Vertex Array Obejct (VAO) which has the job of keeping track of what parts or what buffers correspond to what data. In this example we want to set our VAO up so that 
        // it tells opengl that we want to interpret 12 bytes as 3 floats and divide the buffer into vertices using that.
        // To do this we generate and bind a VAO (which looks deceptivly similar to creating and binding a VBO, but they are different!).
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        // Now, we need to setup how the vertex shader will interpret the VBO data; you can send almost any C datatype (and a few non-C ones too) to it.
        // While this makes them incredibly flexible, it means we have to specify how that data will be mapped to the shader's input variables.

        // To do this, we use the GL.VertexAttribPointer function
        // This function has two jobs, to tell opengl about the format of the data, but also to associate the current array buffer with the VAO.
        // This means that after this call, we have setup this attribute to source data from the current array buffer and interpret it in the way we specified.
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        // Enable variable 0 in the shader.
        GL.EnableVertexAttribArray(0);
        
        // We've got the vertices done, but how exactly should this be converted to pixels for the final image?
        // Modern OpenGL makes this pipeline very free, giving us a lot of freedom on how vertices are turned to pixels.
        // The drawback is that we actually need two more programs for this! These are called "shaders".
        // Shaders are tiny programs that live on the GPU. OpenGL uses them to handle the vertex-to-pixel pipeline.
        // Check out the Shader class in Common to see how we create our shaders, as well as a more in-depth explanation of how shaders work.
        // shader.vert and shader.frag contain the actual shader code.
        _shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");
        
        // Now, enable the shader.
        // Just like the VBO, this is global, so every function that uses a shader will modify this one until a new one is bound instead.
        _shader.Use();
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);
        
        // This clears the image, using what you set as GL.ClearColor earlier.
        // OpenGL provides several different types of data that can be rendered.
        // You can clear multiple buffers by using multiple bit flags.
        // However, we only modify the color, so ColorBufferBit is all we need to clear.
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // To draw an object in OpenGL, it's typically as simple as binding your shader,
        // setting shader uniforms (not done here, will be shown in a future tutorial)
        // binding the VAO,
        // and then calling an OpenGL function to render.

        // Bind the shader
        _shader.Use();
        
        // Bind the VAO
        GL.BindVertexArray(_vertexArrayObject);
        // And then call our drawing function.
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        
        // OpenTK windows are what's known as "double-buffered". In essence, the window manages two buffers.
        // One is rendered to while the other is currently displayed by the window.
        // This avoids screen tearing, a visual artifact that can happen if the buffer is modified while being displayed.
        // After drawing, call this function to swap the buffers. If you don't, it won't display what you've rendered.
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        if (KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
        
        base.OnUpdateFrame(args);
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload() {
        // Unbind all the resources by binding the targets to 0/null.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        // Delete all the resources.
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);

        GL.DeleteProgram(_shader.Handle);

        base.OnUnload();
    }
}