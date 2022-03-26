namespace KGLab2.Common; 

public interface IRenderer {
    public void Load(IEnumerable<Triangle> triangles);
    public void RenderFrame(IEnumerable<Triangle> triangles);
    public void Unload();
}