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

#region BodyData

Body sun = new Body(
    "Sun",
    new ScientificDecimal(1.989m, 30),
    new ScientificDecimal(6.96340m, 8),
    Vector3.Zero,
    Vector3.Zero,
    new ScientificDecimal(1.32712440018m, 20),
    new SKColor(255, 255, 255, 255)
);
Body mercury = new Body(
    "Mercury",
    new ScientificDecimal(3.285m, 23),
    new ScientificDecimal(2.4397m, 6),
    new Vector3(
        new ScientificDecimal(-5.6940545m, 10), 
        new ScientificDecimal( 3.2977160m, 9), 
        new ScientificDecimal( 5.4921780m, 9)
    ), 
    new Vector3(
        new ScientificDecimal(-1.2946428m, 4), 
        new ScientificDecimal(-4.6540563m, 4), 
        new ScientificDecimal(-2.6159023m, 3)
    ),
    new ScientificDecimal(2.20320m, 13),
    new SKColor(140, 140, 140, 255),
    sun
    );
Body venus = new Body(
    "Venus",
    new ScientificDecimal(4.867m, 24),
    new ScientificDecimal(6.0518m, 6),
    new Vector3(
        new ScientificDecimal( 8.2978939m, 10), 
        new ScientificDecimal( 6.9376114m, 10), 
        new ScientificDecimal(-3.8351672m, 9)
    ), 
    new Vector3(
        new ScientificDecimal(-2.2569107m, 4), 
        new ScientificDecimal( 2.6718186m, 4), 
        new ScientificDecimal( 1.6691963m, 3)
    ),
    new ScientificDecimal(3.24858592m, 14),
    new SKColor(230, 160, 40, 255),
    sun
    );
Body earth = new Body(
    "Earth",
    new ScientificDecimal(5.9722m, 24), 
    new ScientificDecimal(6.378m, 6), 
    new Vector3(
        new ScientificDecimal(-8.5613233m, 9), 
        new ScientificDecimal( 1.4688537m, 11), 
        new ScientificDecimal(-8.2420883m, 6)
        ), 
    new Vector3(
        new ScientificDecimal(-3.0223357m, 4), 
        new ScientificDecimal(-1.8447646m, 3), 
        new ScientificDecimal( 2.7092976m, -1)
        ),
    new ScientificDecimal(3.986004418m, 14),
    new SKColor(100, 200, 255, 255),
    sun
);
Body moon = new Body(
    "Moon",
    new ScientificDecimal(7.349m, 22), 
    new ScientificDecimal(1.737m, 6), 
    new Vector3(
        new ScientificDecimal(-3.6413936m, 8), 
        new ScientificDecimal(-1.7481022m, 8),
        new ScientificDecimal(-1.4438294m, 7)
        ), 
    new Vector3(
        new ScientificDecimal( 4.2899598m, 2), 
        new ScientificDecimal(-8.6413934m, 2),
        new ScientificDecimal(-7.7273784m, 1)
        ),
        new ScientificDecimal(4.9048694m, 12),
    new SKColor(180, 180, 180, 255),
    earth
);
Body mars = new Body(
    "Mars",
    new ScientificDecimal(6.39m, 23),
    new ScientificDecimal(3.3895m, 6),
    new Vector3(
        new ScientificDecimal(-6.4603691m, 10), 
        new ScientificDecimal( 2.3127019m, 11),
        new ScientificDecimal( 6.4306793m, 9)
    ), 
    new Vector3(
        new ScientificDecimal(-2.2420469m, 4), 
        new ScientificDecimal(-4.6499686m, 3),
        new ScientificDecimal( 4.5107334m, 2)
    ),
    new ScientificDecimal(4.282837m, 13),
    new SKColor(255, 100, 100, 255),
    sun
    );

Body jupiter = new Body(
    "Jupiter",
    new ScientificDecimal(1.898m, 27),
    new ScientificDecimal(6.9911m, 7),
    new Vector3(
        new ScientificDecimal( 1.6580000m, 11), 
        new ScientificDecimal( 7.4166230m, 11),
        new ScientificDecimal(-6.7903160m, 9)
    ), 
    new Vector3(
        new ScientificDecimal(-1.2915655m, 4), 
        new ScientificDecimal( 3.4670152m, 3),
        new ScientificDecimal( 2.7448357m, 2)
    ),
    new ScientificDecimal(1.26686534m, 17),
    new SKColor(150, 120, 80, 255),
    sun
    );
Body saturn = new Body(
    "Saturn",
    new ScientificDecimal(5.683m, 26),
    new ScientificDecimal(5.8232m, 7),
    new Vector3(
        new ScientificDecimal( 1.4146019m, 12), 
        new ScientificDecimal(-2.6971440m, 11),
        new ScientificDecimal(-5.1612737m, 10)
    ), 
    new Vector3(
        new ScientificDecimal( 1.2650097m, 3), 
        new ScientificDecimal( 9.4749677m, 3),
        new ScientificDecimal(-2.1571900m, 2)
    ),
    new ScientificDecimal(3.7931187m, 16),
    new SKColor(150, 150, 80, 255),
    sun
    );
Body uranus = new Body(
    "Uranus",
    new ScientificDecimal(8.681m, 25),
    new ScientificDecimal(2.5362m, 7),
    new Vector3(
        new ScientificDecimal( 1.6645067m, 12), 
        new ScientificDecimal( 2.4055482m, 12),
        new ScientificDecimal(-1.2648282m, 10)
    ), 
    new Vector3(
        new ScientificDecimal(-5.6626764m, 3), 
        new ScientificDecimal( 3.5634117m, 3),
        new ScientificDecimal( 8.6583568m, 1)
    ),
    new ScientificDecimal(5.793939m, 15),
    new SKColor(170, 200, 255, 255),
    sun
    );
Body neptune = new Body(
    "Neptune",
    new ScientificDecimal(1.024m, 26),
    new ScientificDecimal(2.4622m, 7),
    new Vector3(
        new ScientificDecimal( 4.4699311m, 12), 
        new ScientificDecimal(-9.8183016m, 10),
        new ScientificDecimal(-1.0098417m, 11)
    ), 
    new Vector3(
        new ScientificDecimal( 7.2829293m, 1), 
        new ScientificDecimal( 5.4729751m, 3),
        new ScientificDecimal(-1.1468351m, 2)
    ),
    new ScientificDecimal(	6.836529m, 15),
    new SKColor(100, 120, 200, 255),
    sun
    );
Body pluto = new Body(
    "Pluto",
    // mass of pluto and charon
    new ScientificDecimal(1.309m, 22) + new ScientificDecimal(1.590m, 21),
    new ScientificDecimal(1.1883m, 6),
    new Vector3(
        new ScientificDecimal( 2.7241027m, 12), 
        new ScientificDecimal(-4.4901164m, 12),
        new ScientificDecimal(-3.0720153m, 11)
    ), 
    new Vector3(
        new ScientificDecimal( 4.7808422m, 3), 
        new ScientificDecimal( 1.6241702m, 3),
        new ScientificDecimal(-1.5593730m, 3)
    ),
    new ScientificDecimal(8.71m, 11),
    new SKColor(200, 100, 200, 255),
    sun
    );

#endregion

Body[] bodies = [sun, mercury, venus, earth, mars, jupiter, saturn, uranus, neptune, pluto];

Camera camera = new Camera(Vector2.Zero, Options.DefaultCamZoom, Options.DefaultCamZoom);

int trackingIndex = 0;
Body? tracking = sun;

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
    (Options.ScreenSize.width, Options.ScreenSize.height)
);

ScientificDecimal time = 0;
ScientificDecimal timeStep = new ScientificDecimal(1m, 0);
ScientificDecimal deltaTime;
DateTime previousTime = DateTime.Now;

double earthYearTargetAngle = ScientificDecimal.Atan2Tau(earth.Position.Y, earth.Position.X);
double earthLogPointTargetAngle = earthYearTargetAngle + Math.Tau / Options.EnergyLogPoints;

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
    
    Vector3[] accelerations1 = new Vector3[bodies.Length];
    Vector3[] accelerations2 = new Vector3[bodies.Length];
    
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
    
    Vector3[] accelerations1 = new Vector3[bodies.Length];
    Vector3[] accelerations2 = new Vector3[bodies.Length];

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

    Vector3[] positionK1 = new Vector3[bodies.Length];
    Vector3[] positionK2 = new Vector3[bodies.Length];
    Vector3[] positionK3 = new Vector3[bodies.Length];
    Vector3[] positionK4 = new Vector3[bodies.Length];
    Vector3[] velocityK1 = new Vector3[bodies.Length];
    Vector3[] velocityK2 = new Vector3[bodies.Length];
    Vector3[] velocityK3 = new Vector3[bodies.Length];
    Vector3[] velocityK4 = new Vector3[bodies.Length];
    Vector3[] accelerations1 = new Vector3[bodies.Length];
    Vector3[] accelerations2 = new Vector3[bodies.Length];
    Body[] tempBodies = new Body[bodies.Length];

    for (int i = 0; i < bodies.Length; ++i)
    {
        Body body = bodies[i];
        tempBodies[i] = new Body(body.Name, body.Mass, body.Radius, body.Position, body.Velocity, body.Mu, body.Color,
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

void ApplyIntegratorStep(Action integrator)
{
    foreach (Body body in bodies)
    {
        bool logPositionThisFrame = true;
        if (body.Parent != null)
        {
            if (body.IsInOrbit())
            {
                Vector3 futurePosition = body.Position + body.Velocity * timeStep * deltaTime;
                if (body.GetTrajectoryAngularDeviation(futurePosition) < 
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
    
    if (Options.CorrectOrbitalEnergyDrift)
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
            Vector3 futurePosition = body.CartesianDistanceAtAnomaly(body.TrueAnomaly(time) + 0.001f);
            if (body.GetTrajectoryAngularDeviation(futurePosition) <
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
    
    foreach (Body body in bodies) body.Draw(drawOptions);

    bool approachingYearFlag = false;
    if (Options.LogEarthOrbitalPeriod)
        if (ScientificDecimal.Atan2Tau(earth.Position.Y, earth.Position.X) < earthYearTargetAngle)
            approachingYearFlag = true;

    bool approachingEnergyLogPointFlag = false;
    if (Options.LogEarthOrbitalEnergy)
        if (ScientificDecimal.Atan2Tau(earth.Position.Y, earth.Position.X) < earthLogPointTargetAngle)
            approachingEnergyLogPointFlag = true;
    
    switch (Options.SimMethod)
    {
        case SimulationMethod.Euler: ApplyIntegratorStep(EulerMethod); break;
        case SimulationMethod.VelocityVerlet: ApplyIntegratorStep(VelocityVerletMethod); break;
        case SimulationMethod.Leapfrog: ApplyIntegratorStep(LeapfrogMethod); break;
        case SimulationMethod.RungeKutta4: ApplyIntegratorStep(RungeKutta4Method); break;
        case SimulationMethod.Kepler: ApplyKeplerMethod(); break;
    }
    
    if (Options.LogEarthOrbitalPeriod && approachingYearFlag)
        if (ScientificDecimal.Atan2Tau(earth.Position.Y, earth.Position.X) > earthYearTargetAngle)
            Console.WriteLine("Orbital Period: " + time);

    if (Options.LogEarthOrbitalEnergy && approachingEnergyLogPointFlag)
    {
        if (ScientificDecimal.Atan2Tau(earth.Position.Y, earth.Position.X) > earthLogPointTargetAngle)
        {
            Console.Write(earth.GetSpecificOrbitalEnergy() + ",");
            earthLogPointTargetAngle += Math.Tau / Options.EnergyLogPoints;
            earthLogPointTargetAngle %= Math.Tau;
        }
    }
    
    if (tracking != null) camera.SetOrigin(tracking.Position.Flatten());
    foreach (Body body in bodies) body.DrawOrbitPath(drawOptions);
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