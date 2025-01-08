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
    Size = new Vector2D<int>(800, 800),
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
var renderTarget = new GRBackendRenderTarget(800, 800, 0, 8, 
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
    new SKColor(255, 255, 255, 255)
    );
Body mercury = new Body(
    "Mercury",
    new ScientificDecimal(3.285m, 23),
    new ScientificDecimal(2.4397m, 6),
    new Vector2(new ScientificDecimal(-5.7910403m, 10), new ScientificDecimal(-7.28489453m, 8)),
    new Vector2(new ScientificDecimal(-9.5219453m, 3), new ScientificDecimal(-4.66180626m, 4)),
    new SKColor(140, 140, 140, 255),
    sun
    );
Body venus = new Body(
    "Venus",
    new ScientificDecimal(4.867m, 24),
    new ScientificDecimal(6.0518m, 6),
    new Vector2(new ScientificDecimal(8.099679m, 10), new ScientificDecimal(7.1657153m, 10)),
    new Vector2(new ScientificDecimal(-2.33109495m, 4), new ScientificDecimal(2.60801434m, 4)),
    new SKColor(230, 160, 40, 255),
    sun
);
Body earth = new Body(
    "Earth",
    new ScientificDecimal(5.9722m, 24), 
    new ScientificDecimal(6.378m, 6), 
    new Vector2(new ScientificDecimal(-9.4260948m,9), new ScientificDecimal(1.4615465m, 11)), 
    new Vector2(new ScientificDecimal(-3.0211005m, 4), new ScientificDecimal(-1.8512639m, 3)),
    new SKColor(100, 200, 255, 255),
    sun
    );
Body mars = new Body(
    "Mars",
    new ScientificDecimal(6.39m, 23),
    new ScientificDecimal(3.3895m, 6),
    new Vector2(new ScientificDecimal(-6.6538489m, 10), new ScientificDecimal(2.30876679m, 11)),
    new Vector2(new ScientificDecimal(-2.23662636m, 4), new ScientificDecimal(-4.64996864m, 3)),
    new SKColor(255, 100, 100, 255),
    sun
    );
Body jupiter = new Body(
    "Jupiter",
    new ScientificDecimal(1.898m, 27),
    new ScientificDecimal(6.9911m, 7),
    new Vector2(new ScientificDecimal(1.646840029m, 11), new ScientificDecimal(7.41960946m, 11)),
    new Vector2(new ScientificDecimal(-1.2918988m, 4), new ScientificDecimal(3.44874193m, 3)),
    new SKColor(150, 120, 80, 255),
    sun
    );
Body saturn = new Body(
    "Saturn",
    new ScientificDecimal(5.683m, 26),
    new ScientificDecimal(5.8232m, 7),
    new Vector2(new ScientificDecimal(1.41471083m, 12), new ScientificDecimal(-2.6889596m, 11)),
    new Vector2(new ScientificDecimal(1.26040197m, 3), new ScientificDecimal(9.4750489m, 3)),
    new SKColor(150, 150, 80, 255),
    sun
    );
Body uranus = new Body(
    "Uranus",
    new ScientificDecimal(8.681m, 25),
    new ScientificDecimal(2.5362m, 7),
    new Vector2(new ScientificDecimal(1.6640173m, 12), new ScientificDecimal(2.4058560m, 12)),
    new Vector2(new ScientificDecimal(-5.6637003m, 3), new ScientificDecimal(3.562345m, 3)),
    new SKColor(170, 200, 255, 255),
    sun
    );
Body neptune = new Body(
    "Neptune",
    new ScientificDecimal(1.024m, 26),
    new ScientificDecimal(2.4622m, 7),
    new Vector2(new ScientificDecimal(4.4699374m, 12), new ScientificDecimal(-9.7710185m, 10)),
    new Vector2(new ScientificDecimal(7.1819887m, 1), new ScientificDecimal(5.4722076m, 3)),
    new SKColor(100, 120, 200, 255),
    sun
    );
Body pluto = new Body(
    "Pluto",
    // mass of pluto and charon
    new ScientificDecimal(1.309m, 22) + new ScientificDecimal(1.590m, 21),
    new ScientificDecimal(1.1883m, 6),
    new Vector2(new ScientificDecimal(2.724516m, 12), new ScientificDecimal(-4.489976m, 12)),
    new Vector2(new ScientificDecimal(4.7806206m, 3), new ScientificDecimal(1.6245027m, 3)),
    new SKColor(200, 100, 200, 255),
    sun
    );
Body sedna = new Body(
    "Sedna",
    new ScientificDecimal(2.2m, 21),
    new ScientificDecimal(0.6m, 6),
    new Vector2(new ScientificDecimal(6.0317233m, 12), new ScientificDecimal(-1.0598047m, 13)), 
    new Vector2(new ScientificDecimal(-4.29328267m, 3), new ScientificDecimal(1.0710045m, 3)),
    new SKColor(200, 100, 100, 255),
    sun
    );
Body moon = new Body(
    "Moon",
    new ScientificDecimal(7.349m, 22), 
    new ScientificDecimal(1.737m, 6), 
    new Vector2(new ScientificDecimal(-3.1899866m, 8), new ScientificDecimal(-2.44970212m, 8)), 
    new Vector2(new ScientificDecimal(6.1224179m, 2), new ScientificDecimal(-7.5360303m, 2)),
    new SKColor(180, 180, 180, 255),
    earth
    );
Body[] bodies = [sun, earth];
Camera camera = new Camera(Vector2.Zero, 100, 100);

int trackingIndex = 0;
Body? tracking = null;

SKPaint blackPaint = new SKPaint
{
    Color = SKColors.White,
};
SKFont font = new SKFont()
{
    Size = 20,
};
DrawOptions drawOptions = new DrawOptions(
    canvas,
    blackPaint,
    camera,
    (window.Size.X, window.Size.Y)
);

ScientificDecimal time = 0;
ScientificDecimal timeStep = new ScientificDecimal(1m, 0);
ScientificDecimal deltaTime;
DateTime previousTime = DateTime.Now;

void HandleKeyPresses(IKeyboard keyboard, Key key, int keyCode)
{
    if (key == Key.R || key == Key.F)
    {
        trackingIndex += key == Key.R ? 1 : -1;
        if (trackingIndex < 0) trackingIndex = bodies.Length;
        trackingIndex %= bodies.Length + 1;
        tracking = trackingIndex >= bodies.Length ? null : bodies[trackingIndex];
        if (tracking != null) camera.GoToBody(tracking);
    }

    if (key == Key.G) if (tracking != null) camera.GoToBody(tracking);

    if (key == Key.Comma) timeStep /= 10;
    if (key == Key.Period) timeStep *= 10;
}
input.Keyboards[0].KeyDown += HandleKeyPresses;

void HandleInput(IKeyboard keyboard)
{
    if (keyboard.IsKeyPressed(Key.W)) camera.MoveBy(new Vector2(0, camera.Height / -100));
    if (keyboard.IsKeyPressed(Key.S)) camera.MoveBy(new Vector2(0, camera.Height / 100));
    if (keyboard.IsKeyPressed(Key.A)) camera.MoveBy(new Vector2(camera.Height / -100, 0));
    if (keyboard.IsKeyPressed(Key.D)) camera.MoveBy(new Vector2(camera.Height / 100, 0));
    
    if (keyboard.IsKeyPressed(Key.Q)) camera.ScaleZoom(1.05f);
    if (keyboard.IsKeyPressed(Key.E)) camera.ScaleZoom(0.95f);
}

void ApplyEulerMethod()
{
    foreach (Body body in bodies)
    {
        body.SetAbsoluteVelocity(body.AbsoluteVelocity + body.GetInstantAcceleration(bodies.ToArray()) * timeStep * deltaTime);
        body.SetAbsolutePosition(body.AbsolutePosition + body.AbsoluteVelocity * timeStep * deltaTime);
        body.LogPosition();
        body.CalculateOrbitScreenPoints(drawOptions);
        body.DrawOrbitPath(drawOptions);
    }
}

void ApplyKeplerMethod()
{
    foreach (Body body in bodies)
    {
        if (body.Parent != null)
        {
            body.Position = body.CartesianDistanceAtAnomaly(body.TrueAnomaly(time));
            if (timeStep * deltaTime < body.OrbitalPeriod() / 15 && timeStep * deltaTime > body.OrbitalPeriod() / 3000)
            {
                body.LogPosition();
            }
            body.CalculateOrbitScreenPoints(drawOptions);
            body.DrawOrbitPath(drawOptions);
        }
        if (tracking != null) camera.SetOrigin(tracking.AbsolutePosition);
    }
}

void ApplyRungeKuttaMethod() 
{
    
}

void OnRender(double _)
{
    grContext.ResetContext();
    canvas.Clear(SKColors.Black);
    
    deltaTime = (DateTime.Now - previousTime).TotalSeconds;
    previousTime = DateTime.Now;
    time += deltaTime * timeStep;
    
    ApplyEulerMethod();
    
    foreach(Body body in bodies) body.Draw(drawOptions);
    HandleInput(input.Keyboards[0]);
    
    canvas.DrawText("Tracking: " + (tracking?.Name ?? "Nothing"), 20, 40, font, blackPaint);
    canvas.DrawText("Time scale (s): " + timeStep, 20, 80, font, blackPaint);
    canvas.DrawText("Current date: " + new DateTime(2024, 12, 25).AddSeconds((double)time), 20, 120, font, blackPaint);
    if (tracking != null)
        canvas.DrawText("Velocity: " + tracking.Velocity.Magnitude(), 20, 160, font, blackPaint);
    
    canvas.Flush();
}

window.Render += OnRender;

window.Run();