using SkiaSharp;

namespace OrbitPredictionSimulation;

public struct DrawOptions(SKCanvas canvas, SKPaint paint, Camera camera, (int X, int Y) screenSize)
{
    public SKCanvas Canvas { get; set; } = canvas;
    public SKPaint Paint { get; set; } = paint;
    public Camera Camera { get; set; } = camera;
    public (int X, int Y) ScreenSize { get; set; } = screenSize;
}