using System.Numerics;
using OrbitPredictionSimulation;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;
using Silk.NET.Input;
using SkiaSharp;
using Vector2 = OrbitPredictionSimulation.Vector2;
// ReSharper disable AccessToDisposedClosure

WindowOptions options = WindowOptions.Default with
{
    Size = new Vector2D<int>(800, 700),
    Title = "Orbit Prediction Simulation",
    PreferredStencilBufferBits = 8,
    PreferredBitDepth = new Vector4D<int>(8, 8, 8, 8),
};

GlfwWindowing.Use();
IWindow window = Window.Create(options);
window.Initialize();

using GRGlInterface grGlInterface = GRGlInterface.Create(
    name => window.GLContext!.TryGetProcAddress(name, out var addr) ? addr : 0);
grGlInterface.Validate();
using GRContext grContext = GRContext.CreateGl(grGlInterface);
var renderTarget = new GRBackendRenderTarget(800, 600, 0, 8, 
    new GRGlFramebufferInfo(0, 0x8058)); // 0x8058 = GL_RGBA8`
using SKSurface surface = SKSurface.Create(grContext, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
using SKCanvas canvas = surface.Canvas;

IInputContext input = window.CreateInput();

Body sun = new Body(
    "Sun",
    new BigDecimal(1989, 27), 
    new BigDecimal(696340, 3), 
    Vector2.Zero, 
    Vector2.Zero,
    new SKColor(255, 255, 255, 255)
    );
Body earth = new Body(
    "Earth",
     BigDecimal.Create(5.97219m, 24), 
    new BigDecimal(6378, 3), 
    new Vector2(BigDecimal.Create(-9.421670287m,9), BigDecimal.Create(1.4615677115m, 11)), 
    new Vector2(BigDecimal.Create(-3.021621743m, 4), BigDecimal.Create(-1.8407641746m, 3)),
    new SKColor(100, 200, 255, 255),
    sun
    );
Body moon = new Body(
    "Moon",
    30, 
    new BigDecimal(1737, 3), 
    new Vector2(new BigDecimal(3844, 5), 0), 
    Vector2.Zero,
    new SKColor(180, 180, 180, 255),
    earth
    );
List<Body> bodies = [sun, earth, moon];
Camera camera = new Camera(Vector2.Zero, 100, 100);

int trackingIndex = 0;
Body? tracking = null;

void HandleKeyPresses(IKeyboard keyboard, Key key, int keyCode)
{
    if (key == Key.Comma)
    {
        trackingIndex++;
        trackingIndex %= bodies.Count + 1;
        tracking = trackingIndex >= bodies.Count ? null : bodies[trackingIndex];
    }
}
input.Keyboards[0].KeyDown += HandleKeyPresses;

void HandleInput(IKeyboard keyboard)
{
    if (keyboard.IsKeyPressed(Key.W)) camera.Position.Y -= camera.Height / 100;
    if (keyboard.IsKeyPressed(Key.S)) camera.Position.Y += camera.Height / 100;
    if (keyboard.IsKeyPressed(Key.A)) camera.Position.X -= camera.Width / 100;
    if (keyboard.IsKeyPressed(Key.D)) camera.Position.X += camera.Width / 100;
    
    if (keyboard.IsKeyPressed(Key.R))
    {
        camera.Width *= 1.05;
        camera.Height *= 1.05;
    }
    if (keyboard.IsKeyPressed(Key.F)) 
    {
        camera.Width *= 0.95;
        camera.Height *= 0.95;
    }
}

BigDecimal eccentricity = earth.CalculateEccentricity();
Console.WriteLine(eccentricity);

void OnRender(double _)
{
    //canvas.Scale(window.Size.X, window.Size.Y);
    grContext.ResetContext();
    canvas.Clear(SKColors.Black);
    SKPaint blackPaint = new SKPaint
    {
        Color = SKColors.White,
    };
    SKFont font = new SKFont()
    {
        Size = 40,
    };
    DrawOptions drawOptions = new DrawOptions(
        canvas,
        blackPaint,
        camera,
        (window.Size.X, window.Size.Y)
        );
    if (tracking != null)
        camera.GoToBody(tracking);
    foreach (Body body in bodies) body.Draw(drawOptions);
    HandleInput(input.Keyboards[0]);
    canvas.DrawText(camera.Position.X.ToString(), 20, 30, font, blackPaint);
    canvas.DrawText(camera.Position.Y.ToString(), 20, 60, font, blackPaint);
    canvas.Flush();
}
window.Render += OnRender;

window.Run();