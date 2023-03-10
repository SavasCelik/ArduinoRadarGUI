namespace ArduinoRadarGUI;

public class Radar
{
    public int MaxDistance { get; init; }
    public int Radius { get; private set; }
    private readonly Pen _pen;
    private readonly Brush _brush;
    private readonly Font _font;
    private Angle _angle;
    private Point _radarOriginPoint;
    private IDictionary<Point, RadarTarget> _targets;
    private SerialPortHandler _serialPortHandler;

    public Radar()
    {
        _pen = new Pen(Color.Green, 3);
        _brush = Brushes.Green;
        _font = new Font(FontFamily.GenericSerif, 10);
        _targets = new Dictionary<Point, RadarTarget>();
        _angle = new Angle(0);
        _serialPortHandler = new SerialPortHandler(this);
        MaxDistance = 50;
    }

    /// <summary>
    /// Starts listening to the SerialPort messages
    /// </summary>
    public void StartSerialPortListening()
    {
        _serialPortHandler.StartListening();
    }

    /// <summary>
    /// Adds a target for the given distance
    /// </summary>
    public void AddTarget(int targetDistance)
    {
        var position = WorldToScreen(_angle, targetDistance);

        if (_targets.TryGetValue(position, out var foundTarget))
        {
            foundTarget.ResetDeathTime();
        }
        else
        {
            var newTarget = new RadarTarget(position);
            _targets.Add(position, newTarget);
        }
    }

    /// <summary>
    /// Updates the radar with the given angle in degrees
    /// </summary>
    public void Update(int angleInDegrees)
    {
        _angle.SetAngle(angleInDegrees);

        foreach (var target in _targets.Values.Where(x => x.IsDead()))
        {
            _targets.Remove(target.Position);
        }
    }

    /// <summary>
    /// Draws the Radar and its components
    /// </summary>
    public void Draw(Graphics gfx, Size clientSize)
    {
        DetermineRadarSize(clientSize);
        DetermineRadius();
        DrawOutherArc(gfx);
        DrawInnerArc(gfx);
        DrawBottomLine(gfx);
        DrawScanLine(gfx);

        foreach (var target in _targets.Values)
        {
            target.Draw(gfx);
        }
    }

    /// <summary>
    /// Calculates the Radar's size depending on the windows size (client size)
    /// </summary>
    private void DetermineRadarSize(Size clientSize)
    {
        _radarOriginPoint = new Point(clientSize.Width / 2, clientSize.Height - 10);
    }

    /// <summary>
    /// Calculates the Radius for the radar
    /// </summary>
    private void DetermineRadius()
    {
        Radius = Math.Min(_radarOriginPoint.X, _radarOriginPoint.Y);
    }

    /// <summary>
    /// Draws the outher arc
    /// </summary>
    private void DrawOutherArc(Graphics gfx)
    {
        var rect = new Rectangle(_radarOriginPoint.X - Radius, _radarOriginPoint.Y - Radius, Radius * 2, Radius * 2);
        gfx.DrawArc(_pen, rect, 180, 180);
        gfx.DrawString(MaxDistance + "cm", _font, _brush, _radarOriginPoint.X - 25, rect.Y - 25);
    }

    /// <summary>
    /// Draws the inner arc
    /// </summary>
    private void DrawInnerArc(Graphics gfx)
    {
        var rect = new Rectangle(_radarOriginPoint.X - Radius / 2, _radarOriginPoint.Y - Radius / 2, Radius, Radius);
        gfx.DrawArc(_pen, rect, 180, 180);
        gfx.DrawString((MaxDistance / 2) + "cm", _font, _brush, _radarOriginPoint.X - 25, rect.Y - 25);
    }

    /// <summary>
    /// Draws the bottom line
    /// </summary>
    private void DrawBottomLine(Graphics gfx)
    {
        gfx.DrawLine(_pen, _radarOriginPoint.X - Radius, _radarOriginPoint.Y, _radarOriginPoint.X + Radius, _radarOriginPoint.Y);
    }

    /// <summary>
    /// Draws the scan line which goes back and forth from 0 - 180 degrees
    /// </summary>
    private void DrawScanLine(Graphics gfx)
    {
        var lineEndPoint = WorldToScreen(_angle, Radius);
        gfx.DrawLine(_pen, _radarOriginPoint.X, _radarOriginPoint.Y, lineEndPoint.X, lineEndPoint.Y);
    }

    /// <summary>
    /// Calculates the pixel coordinates on the screen
    /// </summary>
    private Point WorldToScreen(Angle angle, int distance)
    {
        var zX = (int)(distance * Math.Cos(angle.Radians));
        var zY = (int)(distance * Math.Sin(angle.Radians));
        var dX = _radarOriginPoint.X + zX;
        var dY = _radarOriginPoint.Y + zY;

        return new Point(dX, dY);
    }
}
