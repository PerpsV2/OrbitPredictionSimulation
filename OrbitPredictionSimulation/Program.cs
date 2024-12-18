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

Body sun = new Body(30, new BigDecimal(696340, 3), Vector2.Zero, Vector2.Zero);
Body venus = new Body(30, new BigDecimal(6052, 3), 
    new Vector2(new BigDecimal(10839,7), 0), 
    Vector2.Zero);
Body earth = new Body(30, new BigDecimal(6378, 3), 
    new Vector2(new BigDecimal(14721,7), 0), 
    Vector2.Zero);
Body moon = new Body(30, new BigDecimal(1737, 3), 
    new Vector2(new BigDecimal(14721,7) + new BigDecimal(3844, 5), 0), 
    Vector2.Zero);
Body mars = new Body(30, new BigDecimal(3390, 3), 
    new Vector2(new BigDecimal(24042,7), 0), 
    Vector2.Zero);
Body jupiter = new Body(30, new BigDecimal(69911, 3), 
    new Vector2(new BigDecimal(75922,7), 0), 
    Vector2.Zero);
Body saturn = new Body(30, new BigDecimal(60268, 3), 
    new Vector2(new BigDecimal(14405,8), 0), 
    Vector2.Zero);
Body uranus = new Body(30, new BigDecimal(25362, 3), 
    new Vector2(new BigDecimal(29243,8), 0), 
    Vector2.Zero);
Body neptune = new Body(30, new BigDecimal(24622, 3), 
    new Vector2(new BigDecimal(43,11), 0), 
    Vector2.Zero);
Body pluto = new Body(30, new BigDecimal(1188, 3), 
    new Vector2(new BigDecimal(59,11), 0), 
    Vector2.Zero);
List<Body> bodies = new List<Body>{ sun, venus, earth, moon, mars, jupiter, saturn, uranus, neptune, pluto };
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
    if (keyboard.IsKeyPressed(Key.F)) {
        camera.Width *= 0.95;
        camera.Height *= 0.95;
    }
}

void OnRender(double _)
{
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