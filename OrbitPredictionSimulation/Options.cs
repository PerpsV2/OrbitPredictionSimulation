using Silk.NET.Input;

namespace OrbitPredictionSimulation;

public static class Options
{
    public static readonly SimulationMethod SimMethod = SimulationMethod.Euler;
    public static readonly (int width, int height) ScreenSize = (800, 800);
    public const int FontSize = 20;
    
    public const int ScientificPrintPrecision = 5;
    
    // Planet graphics options
    public const int MinimumVisibleRadius = 10;
    public const int CrossSpokeSize = 7;
    public const int CrossSpokeWidth = 2;
    public const int MaxOrbitPathPositions = 1000;
    
    // This only affects bodies which are in an elliptic trajectory
    public const float MaxEulerOrbitPoints = 20;
    
    // If the number of orbital points lies outside of these bounds,
    // orbital lines for that body will no longer be updated
    public const int MinKeplerOrbitPoints = 20;
    public const int MaxKeplerOrbitPoints = 3000;
    
    // Camera options
    public static readonly ScientificDecimal DefaultCamZoom = new (8);
    public const float CamMoveSpeed = 0.01f;
    public const float CamZoomSpeed = 0.05f;
    public const float FocusZoomRadiusMultiplier = 5f;

    public const int TimeWarpIncrement = 10;
    
    // Key binds
    public const Key TrackNextKey = Key.R;
    public const Key TrackPreviousKey = Key.F;
    public const Key FocusTrackKey = Key.G;
    
    public const Key MoveUpKey = Key.W;
    public const Key MoveLeftKey = Key.A;
    public const Key MoveDownKey = Key.S;
    public const Key MoveRightKey = Key.D;
    
    public const Key ZoomOutKey = Key.Q;
    public const Key ZoomInKey = Key.E;
    
    public const Key TimeWarpUpKey = Key.Period;
    public const Key TimeWarpDownKey = Key.Comma;
}