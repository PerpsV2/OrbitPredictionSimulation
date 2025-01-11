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
    Size = new Vector2D<int>(Options.ScreenSize.width, Options.ScreenSize.height),
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
var renderTarget = new GRBackendRenderTarget(Options.ScreenSize.width, Options.ScreenSize.height, 0, 
    8, new GRGlFramebufferInfo(0, 0x8058)); // 0x8058 = GL_RGBA8`
using SKSurface surface = SKSurface.Create(grContext, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
using SKCanvas canvas = surface.Canvas;
IInputContext input = window.CreateInput();

Body3D sun3d = new Body3D(
    "Sun",
    new ScientificDecimal(1.989m, 30),
    new ScientificDecimal(6.96340m, 8),
    Vector3.Zero,
    Vector3.Zero,
    new ScientificDecimal(1.32712440018m, 20),
    new SKColor(255, 255, 255, 255)
);
Body3D earth3d = new Body3D(
    "Earth",
    new ScientificDecimal(5.9722m, 24), 
    new ScientificDecimal(6.378m, 6), 
    new Vector3(
        new ScientificDecimal(-9.4260948m,9), 
        new ScientificDecimal(1.4615465m, 11), 
        new ScientificDecimal(-8.066655m, 6)), 
    new Vector3(
        new ScientificDecimal(-3.0211005m, 4), 
        new ScientificDecimal(-1.8512639m, 3), 
        new ScientificDecimal(1.2098514m, 0)),
    new ScientificDecimal(3.986004418m, 14),
    new SKColor(100, 200, 255, 255),
    sun3d
);

Body3D[] bodies3d = [sun3d, earth3d];

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
Body moon = new Body(
    "Moon",
    new ScientificDecimal(7.349m, 22), 
    new ScientificDecimal(1.737m, 6), 
    new Vector2(new ScientificDecimal(-3.1899866m, 8), new ScientificDecimal(-2.44970212m, 8)), 
    new Vector2(new ScientificDecimal(6.1224179m, 2), new ScientificDecimal(-7.5360303m, 2)),
    new SKColor(180, 180, 180, 255),
    earth
    );

Body[] bodies = [sun, mercury, venus, earth, moon, mars, jupiter, saturn, uranus, neptune, pluto];
Camera camera = new Camera(Vector2.Zero, Options.DefaultCamZoom, Options.DefaultCamZoom);

int trackingIndex = 0;
Body? tracking = null;

SKPaint paint = new SKPaint
{
    Color = SKColors.White,
};
SKFont font = new SKFont
{
    Size = Options.FontSize,
};
DrawOptions drawOptions = new DrawOptions(
    canvas,
    paint,
    camera,
    (window.Size.X, window.Size.Y)
);

ScientificDecimal time = 0;
ScientificDecimal timeStep = new ScientificDecimal(1m, 0);
ScientificDecimal deltaTime;
DateTime previousTime = DateTime.Now;

void HandleKeyPresses(IKeyboard keyboard, Key key, int keyCode)
{
    if (key == Options.TrackNextKey || key == Options.TrackPreviousKey)
    {
        trackingIndex += key == Options.TrackNextKey ? 1 : -1;
        if (trackingIndex < 0) trackingIndex = bodies.Length;
        trackingIndex %= bodies.Length + 1;
        tracking = trackingIndex >= bodies.Length ? null : bodies[trackingIndex];
        if (tracking != null) camera.GoToBody(tracking);
    }
    
    if (key == Options.FocusTrackKey && tracking != null)
    {
        camera.GoToBody(tracking);
        camera.ScaleZoom(tracking.Radius * Options.FocusZoomRadiusMultiplier / camera.Width);
    }

    if (key == Options.TimeWarpDownKey) timeStep /= Options.TimeWarpIncrement;
    if (key == Options.TimeWarpUpKey) timeStep *= Options.TimeWarpIncrement;
}
input.Keyboards[0].KeyDown += HandleKeyPresses;

void HandleInput(IKeyboard keyboard)
{
    if (keyboard.IsKeyPressed(Options.MoveUpKey)) camera.MoveBy(new Vector2(0, camera.Height * -Options.CamMoveSpeed));
    if (keyboard.IsKeyPressed(Options.MoveDownKey)) camera.MoveBy(new Vector2(0, camera.Height * Options.CamMoveSpeed));
    if (keyboard.IsKeyPressed(Options.MoveLeftKey)) camera.MoveBy(new Vector2(camera.Height * -Options.CamMoveSpeed, 0));
    if (keyboard.IsKeyPressed(Options.MoveRightKey)) camera.MoveBy(new Vector2(camera.Height * Options.CamMoveSpeed, 0));
    
    if (keyboard.IsKeyPressed(Options.ZoomOutKey)) camera.ScaleZoom(1 + Options.CamZoomSpeed);
    if (keyboard.IsKeyPressed(Options.ZoomInKey)) camera.ScaleZoom(1 - Options.CamZoomSpeed);
}

void EulerMethod()
{
    foreach (Body body in bodies)
    {
        if (body.Parent != null)
            if (!body.IsInOrbit())
                body.SetParent(body.Parent.Parent);

        body.Velocity += body.GetInstantAcceleration(bodies) * timeStep * deltaTime;
        body.Position += body.Velocity * timeStep * deltaTime;
    }
}

void VelocityVerletMethod() 
{
    ScientificDecimal dt = timeStep * deltaTime;
    
    Vector2[] accelerations1 = new Vector2[bodies.Length];
    Vector2[] accelerations2 = new Vector2[bodies.Length];
    
    for (int i = 0; i < bodies.Length; ++i)
        accelerations1[i] = bodies[i].GetInstantAcceleration(bodies);
    
    for (int i = 0; i < bodies.Length; ++i)
        bodies[i].Position += bodies[i].Velocity * dt + accelerations1[i] * 0.5f * dt * dt;
    
    for (int i = 0; i < bodies.Length; ++i)
        accelerations2[i] = bodies[i].GetInstantAcceleration(bodies);

    for (int i = 0; i < bodies.Length; ++i)
        bodies[i].Velocity += (accelerations1[i] + accelerations2[i]) * 0.5f * dt;
}

void LeapfrogMethod() 
{
    ScientificDecimal dt = timeStep * deltaTime;
    
    Vector2[] accelerations1 = new Vector2[bodies.Length];
    Vector2[] accelerations2 = new Vector2[bodies.Length];

    for (int i = 0; i < bodies.Length; ++i)
        accelerations1[i] = bodies[i].GetInstantAcceleration(bodies);
    
    for (int i = 0; i < bodies.Length; ++i)
        bodies[i].Position += bodies[i].Velocity * dt + accelerations1[i] * 0.5f * dt * dt;

    for (int i = 0; i < bodies.Length; ++i)
        accelerations2[i] = bodies[i].GetInstantAcceleration(bodies);
    
    for (int i = 0; i < bodies.Length; ++i)
        bodies[i].Velocity += (accelerations1[i] + accelerations2[i]) * 0.5f * dt;
}

void RungeKutta4Method() 
{
    ScientificDecimal dt = timeStep * deltaTime;

    Vector2[] positionK1 = new Vector2[bodies.Length];
    Vector2[] positionK2 = new Vector2[bodies.Length];
    Vector2[] positionK3 = new Vector2[bodies.Length];
    Vector2[] positionK4 = new Vector2[bodies.Length];
    Vector2[] velocityK1 = new Vector2[bodies.Length];
    Vector2[] velocityK2 = new Vector2[bodies.Length];
    Vector2[] velocityK3 = new Vector2[bodies.Length];
    Vector2[] velocityK4 = new Vector2[bodies.Length];
    Vector2[] accelerations1 = new Vector2[bodies.Length];
    Vector2[] accelerations2 = new Vector2[bodies.Length];
    Body[] tempBodies = new Body[bodies.Length];

    for (int i = 0; i < bodies.Length; ++i)
    {
        Body body = bodies[i];
        tempBodies[i] = new Body(body.Name, body.Mass, body.Radius, body.Position, body.Velocity, body.Color,
            body.Parent);
    }
    
    for (int i = 0; i < bodies.Length; ++i)
    {
        accelerations1[i] = bodies[i].GetInstantAcceleration(bodies);
        velocityK1[i] = accelerations1[i] * dt;
        positionK1[i] = bodies[i].Velocity * dt;

        tempBodies[i].Position = bodies[i].Position + positionK1[i] * 0.5f;
        tempBodies[i].Velocity = bodies[i].Velocity + velocityK1[i] * 0.5f;
        accelerations2[i] = tempBodies[i].GetInstantAcceleration(tempBodies);

        velocityK2[i] = accelerations2[i] * dt;
        positionK2[i] = tempBodies[i].Velocity * dt;

        tempBodies[i].Position = bodies[i].Position + positionK2[i] * 0.5f;
        tempBodies[i].Velocity = bodies[i].Velocity + velocityK2[i] * 0.5f;
        accelerations2[i] = tempBodies[i].GetInstantAcceleration(tempBodies);

        velocityK3[i] = accelerations2[i] * dt;
        positionK3[i] = tempBodies[i].Velocity * dt;

        tempBodies[i].Position = bodies[i].Position + positionK3[i];
        tempBodies[i].Velocity = bodies[i].Velocity + velocityK3[i];
        accelerations2[i] = tempBodies[i].GetInstantAcceleration(tempBodies);

        velocityK4[i] = accelerations2[i] * dt;
        positionK4[i] = tempBodies[i].Velocity * dt;

        bodies[i].Velocity += (velocityK1[i] + velocityK2[i] * 2 + velocityK3[i] * 2 + velocityK4[i]) * (1f / 6f);
        bodies[i].Position += (positionK1[i] + positionK2[i] * 2 + positionK3[i] * 2 + positionK4[i]) * (1f / 6f);
    }
}

void EulerMethod3D()
{
    foreach (Body3D body in bodies3d)
    {
        body.LogPosition();
        body.CalculateOrbitScreenPoints(drawOptions);
    }
    
    foreach (Body3D body in bodies3d)
    {
        if (body.Parent != null)
            if (!body.IsInOrbit())
                body.SetParent(body.Parent.Parent);

        body.Velocity += body.GetInstantAcceleration(bodies3d) * timeStep * deltaTime;
        body.Position += body.Velocity * timeStep * deltaTime;
    }
}

void ApplyIntegratorStep(Action integrator)
{
    foreach (Body body in bodies)
    {
        bool logPositionThisFrame = true;
        if (body.Parent != null)
        {
            if (body.IsInOrbit())
            {
                Vector2 futurePosition = body.Position + body.Velocity * timeStep * deltaTime;
                if (body.GetAngularDeviationSinceLastLoggedPosition(futurePosition) < 
                    Math.Tau / Options.MaxIntegratorOrbitPoints)
                    logPositionThisFrame = false;
                else body.LogTrajectory(futurePosition);
            }
        }
        if (logPositionThisFrame) body.LogPosition();
        body.CalculateOrbitScreenPoints(drawOptions);
    }

    foreach (Body body in bodies)
        if (body.Parent != null)
            if (!body.IsInOrbit())
                body.SetParent(body.Parent.Parent);

    integrator();
    
    foreach (Body body in bodies) 
        if (body.Parent != null)
            body.ResetSpecificOrbitalEnergy();
}

void ApplyKeplerMethod()
{
    foreach (Body body in bodies)
    {
        bool logPositionThisFrame = true;
        if (body.Parent != null)
        {
            Vector2 futurePosition = body.CartesianDistanceAtAnomaly(body.TrueAnomaly(time) + 0.1f);
            if (body.GetAngularDeviationSinceLastLoggedPosition(futurePosition) <
                Math.Tau / Options.MaxKeplerOrbitPoints)
                logPositionThisFrame = false;
            else body.LogTrajectory(futurePosition);
            
            if (logPositionThisFrame)
            {
                if (timeStep * deltaTime < body.OrbitalPeriod() / Options.MinKeplerOrbitPoints)
                    body.LogPosition();
                else body.LogNullPosition();
            }
            
            body.CalculateOrbitScreenPoints(drawOptions);
        }
    }
    
    if (Options.CorrectOrbitalEnergyDrift)
        foreach (Body body in bodies)
            if (body.Parent != null) 
                body.SetRelativePosition(body.CartesianDistanceAtAnomaly(body.TrueAnomaly(time)));
}

void OnRender(double _)
{
    grContext.ResetContext();
    canvas.Clear(SKColors.Black);
    
    // increment time
    deltaTime = (DateTime.Now - previousTime).TotalSeconds;
    previousTime = DateTime.Now;
    time += deltaTime * timeStep;
    
    //foreach (Body body in bodies) body.Draw(drawOptions);
    foreach (Body3D body in bodies3d) body.Draw(drawOptions);
    
    /*
    switch (Options.SimMethod)
    {
        case SimulationMethod.Euler: ApplyIntegratorStep(EulerMethod); break;
        case SimulationMethod.VelocityVerlet: ApplyIntegratorStep(VelocityVerletMethod); break;
        case SimulationMethod.Leapfrog: ApplyIntegratorStep(LeapfrogMethod); break;
        case SimulationMethod.RungeKutta4: ApplyIntegratorStep(RungeKutta4Method); break;
        case SimulationMethod.Kepler: ApplyKeplerMethod(); break;
    }
    */
    EulerMethod3D();
    
    if (tracking != null) camera.SetOrigin(tracking.Position);
    
    //foreach(Body body in bodies) body.DrawOrbitPath(drawOptions);
    foreach (Body3D body in bodies3d) body.DrawOrbitPath(drawOptions);
    
    HandleInput(input.Keyboards[0]);
    
    canvas.DrawText("Simulation Method: " + Options.SimMethod, 20, 40, font, paint);
    canvas.DrawText("Tracking: " + (tracking?.Name ?? "Nothing"), 20, 80, font, paint);
    canvas.DrawText("Time scale (s): " + timeStep, 20, 120, font, paint);
    try {
        canvas.DrawText("Current date: " + new DateTime(2024, 12, 25).AddSeconds((double)time),
            20, 160, font, paint);
    }
    catch (ArgumentOutOfRangeException) {
        canvas.DrawText("Current date: >10000y A.D.", 20, 160, font, paint);
    }

    if (tracking != null && Options.SimMethod != SimulationMethod.Kepler)
        canvas.DrawText("Velocity (m/s): " + tracking.RelativeVelocity.Magnitude(), 20, 200, font, paint);
    
    canvas.Flush();
}

window.Render += OnRender;

window.Run();