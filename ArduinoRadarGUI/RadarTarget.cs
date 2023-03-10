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

	/// <summary>
	/// Resets the death time
	/// </summary>
	public void ResetDeathTime() => _discoveryDate = DateTime.Now;

	/// <summary>
	/// Returns whether or not the target is dead
	/// </summary>
	public bool IsDead() => (DateTime.Now - _discoveryDate) > _lifeSpan;

	/// <summary>
	/// Draws the radar target
	/// </summary>
	public void Draw(Graphics gfx) => gfx.FillEllipse(_targetBrush, Position.X, Position.Y, _size, _size);
}
