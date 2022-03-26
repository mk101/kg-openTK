namespace KGLab2.Common; 

public static class RandomExtensions {
    public static float NextSingle(this Random rand, float min, float max) {
        return (max - min)*rand.NextSingle() + min;
    }
}