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
    new ScientificDecimal(1.989m, 30), 
    new ScientificDecimal(6.96340m, 8), 
    Vector2.Zero, 
    Vector2.Zero,
    0,
    new SKColor(255, 255, 255, 255)
    );
Body earth = new Body(
    "Earth",
    new ScientificDecimal(5.97219m, 24), 
    new ScientificDecimal(6.378m, 6), 
    new Vector2(new ScientificDecimal(-1.1167278m,10), new ScientificDecimal(1.4670613m, 11)), 
    new Vector2(new ScientificDecimal(-3.0195272m, 4), new ScientificDecimal(-2.3640870m, 3)),
    0,
    new SKColor(100, 200, 255, 255),
    sun
    );
Body moon = new Body(
    "Moon",
    new ScientificDecimal(7.349m, 22), 
    new ScientificDecimal(1.737m, 6), 
    new Vector2(new ScientificDecimal(3.85m,8), 0), 
    new Vector2(0, new ScientificDecimal(1.022828m, 3)),
    0,
    new SKColor(180, 180, 180, 255),
    earth
    );
List<Body> bodies = [sun, earth, moon];
Camera camera = new Camera(Vector2.Zero, 100, 100);

int trackingIndex = 0;
Body? tracking = null;

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

void HandleKeyPresses(IKeyboard keyboard, Key key, int keyCode)
{
    if (key == Key.Comma)
    {
        trackingIndex++;
        trackingIndex %= bodies.Count + 1;
        tracking = trackingIndex >= bodies.Count ? null : bodies[trackingIndex];
    }

    if (key == Key.Period)
    {
        foreach (var body in bodies) body.CalculateOrbitScreenPoints(drawOptions);
    }
}
input.Keyboards[0].KeyDown += HandleKeyPresses;

void HandleInput(IKeyboard keyboard)
{
    if (keyboard.IsKeyPressed(Key.W)) camera.MoveBy(new Vector2(0, camera.Height / -100));
    if (keyboard.IsKeyPressed(Key.S)) camera.MoveBy(new Vector2(0, camera.Height / 100));
    if (keyboard.IsKeyPressed(Key.A)) camera.MoveBy(new Vector2(camera.Height / -100, 0));
    if (keyboard.IsKeyPressed(Key.D)) camera.MoveBy(new Vector2(camera.Height / 100, 0));
    
    if (keyboard.IsKeyPressed(Key.R)) camera.ScaleZoom(1.05f);
    if (keyboard.IsKeyPressed(Key.F)) camera.ScaleZoom(0.95f);
}

void CalculateOrbitScreenPoints(object sender, EventArgs e)
{
    foreach (Body body in bodies) body.CalculateOrbitScreenPoints(drawOptions);
}

camera.OnChange += CalculateOrbitScreenPoints;

float angle = 0;
void OnRender(double _)
{
    angle += 0.01f;
    //canvas.Scale(window.Size.X, window.Size.Y);
    grContext.ResetContext();
    canvas.Clear(SKColors.Black);
    
    foreach (Body body in bodies)
    {
        if (body.Parent != null)
        {
            body.Position = body.CartesianDistanceAtAnomaly(angle);
            body.LogPosition();
            body.DrawOrbitPath(drawOptions);
        }
        if (tracking == body) camera.GoToBody(body);
        body.Draw(drawOptions);
    }
    
    HandleInput(input.Keyboards[0]);
    canvas.DrawText(camera.Position.X.ToString(), 20, 30, font, blackPaint);
    canvas.DrawText(camera.Position.Y.ToString(), 20, 60, font, blackPaint);
    canvas.Flush();
}
window.Render += OnRender;

window.Run();