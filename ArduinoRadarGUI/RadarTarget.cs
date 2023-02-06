namespace ArduinoRadarGUI;

public class RadarTarget
{
	private readonly TimeSpan _lifeSpan = TimeSpan.FromSeconds(1);
	private DateTime _discoveryDate;
	public Point Position { get; init; }

	public RadarTarget(Point position)
	{
        _discoveryDate = DateTime.Now;
        Position = position;
	}

	public void ResetDeathTime() => _discoveryDate = DateTime.Now;
	public bool IsDead() => (DateTime.Now - _discoveryDate) > _lifeSpan;
}
