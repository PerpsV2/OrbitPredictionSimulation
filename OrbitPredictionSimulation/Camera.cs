using System.Numerics;

namespace OrbitPredictionSimulation;

public class Camera(Vector2 position, BigDecimal width, BigDecimal height)
{
    public Vector2 Position { get; set; } = position;
    public BigDecimal Width { get; set; } = width;
    public BigDecimal Height { get; set; } = height;
    public BigDecimal Left => Position.X - Width / 2;
    public BigDecimal Top => Position.Y - Height / 2;
    public BigDecimal Right => Position.X + Width / 2;
    public BigDecimal Bottom => Position.Y + Height / 2;

    public void GoToBody(Body body)
    {
        Position = new Vector2(body.Position.X, body.Position.Y);
    }
}