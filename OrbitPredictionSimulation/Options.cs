using Silk.NET.Input;

namespace OrbitPredictionSimulation;

public static class Options
{
    public const SimulationMethod SimMethod = SimulationMethod.Kepler;
    public static readonly (int width, int height) ScreenSize = (1200, 1200);
    public const int FontSize = 20;
    
    // Camera options
    public static readonly ScientificDecimal DefaultCamZoom = new (8);
    public const float CamMoveSpeed = 0.01f;
    public const float CamZoomSpeed = 0.05f;

    public const int TimeWarpIncrement = 10;
    
    // Key binds
    public const Key TrackNextKey = Key.R;
    public const Key TrackPreviousKey = Key.F;
    public const Key FocusTrackKey = Key.G;
    public const Key MoveUpKey = Key.W;
    public const Key MoveDownKey = Key.S;
    public const Key MoveLeftKey = Key.A;
    public const Key MoveRightKey = Key.D;
    public const Key ZoomOutKey = Key.Q;
    public const Key ZoomInKey = Key.E;
    public const Key TimeWarpUpKey = Key.Period;
    public const Key TimeWarpDownKey = Key.Comma;
}