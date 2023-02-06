namespace ArduinoRadarGUI;

public class RadarTarget
{
	private readonly TimeSpan _lifeSpan = TimeSpan.FromSeconds(2);
    public DateTime DiscoveryDate { get; init; }
	public Point Position { get; init; }

	public RadarTarget(Point position)
	{
		DiscoveryDate= DateTime.Now;
        Position = position;
	}

	public bool IsDead() => (DateTime.Now - DiscoveryDate) > _lifeSpan;
}
