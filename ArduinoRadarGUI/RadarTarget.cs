namespace ArduinoRadarGUI;

public class RadarTarget
{
	private static readonly TimeSpan _lifeSpan = TimeSpan.FromSeconds(1);
	private static readonly Brush _targetBrush = Brushes.Red;
	private const int _size = 10;
    private DateTime _discoveryDate;
	public Point Position { get; init; }

	public RadarTarget(Point position)
	{
        _discoveryDate = DateTime.Now;
        Position = position;
	}

	public void ResetDeathTime() => _discoveryDate = DateTime.Now;
	public bool IsDead() => (DateTime.Now - _discoveryDate) > _lifeSpan;

	public void Draw(Graphics gfx)
	{
        gfx.FillEllipse(_targetBrush, Position.X, Position.Y, _size, _size);
    }
}
