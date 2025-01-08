namespace OrbitPredictionSimulation;

public delegate void CameraEventHandler(object sender, EventArgs e);

public class Camera(Vector2 position, ScientificDecimal width, ScientificDecimal height)
{
    private Vector2 _localPosition = position;
    private Vector2 _origin = Vector2.Zero;
    public Vector2 Position => _localPosition + _origin;
    public ScientificDecimal Width { get; private set; } = width;
    public ScientificDecimal Height { get; private set; } = height;
    public ScientificDecimal Left => Position.X - Width / 2;
    public ScientificDecimal Top => Position.Y - Height / 2;
    public ScientificDecimal Right => Position.X + Width / 2;
    public ScientificDecimal Bottom => Position.Y + Height / 2;

    public void MoveTo(Vector2 position) => _localPosition = position;
    
    public void MoveBy(Vector2 position) => _localPosition += position;
    
    public void ScaleZoom(ScientificDecimal scale)
    {
        Width *= scale;
        Height *= scale;
    }
    
    public void GoToBody(Body body)
    {
        SetOrigin(body.AbsolutePosition);
        MoveTo(Vector2.Zero);
    }
    
    public void SetOrigin(Vector2 origin) => _origin = origin;
}