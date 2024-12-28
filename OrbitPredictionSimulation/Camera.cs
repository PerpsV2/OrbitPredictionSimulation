namespace OrbitPredictionSimulation;

public delegate void CameraEventHandler(object sender, EventArgs e);

public class Camera(Vector2 position, ScientificDecimal width, ScientificDecimal height)
{
    public Vector2 Position { get; private set; } = position;
    public ScientificDecimal Width { get; private set; } = width;
    public ScientificDecimal Height { get; private set; } = height;
    public ScientificDecimal Left => Position.X - Width / 2;
    public ScientificDecimal Top => Position.Y - Height / 2;
    public ScientificDecimal Right => Position.X + Width / 2;
    public ScientificDecimal Bottom => Position.Y + Height / 2;
    public event CameraEventHandler? OnChange;

    public void MoveTo(Vector2 position)
    {
        Position = position;
        OnChange?.Invoke(this, EventArgs.Empty);
    }
    
    public void MoveBy(Vector2 position)
    {
        Position += position;
        OnChange?.Invoke(this, EventArgs.Empty);
    }
    
    public void ScaleZoom(float scale)
    {
        Width *= scale;
        Height *= scale;
        OnChange?.Invoke(this, EventArgs.Empty);
    }
    
    public void GoToBody(Body body)
    {
        MoveTo(new Vector2(body.Position.X, body.Position.Y));
    }
}