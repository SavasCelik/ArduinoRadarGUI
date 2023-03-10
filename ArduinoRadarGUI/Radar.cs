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

    public void StartSerialPortListening()
    {
        _serialPortHandler.StartListening();
    }

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

    public void Update(int angleInDegrees)
    {
        _angle.SetAngle(angleInDegrees);

        foreach (var target in _targets.Values.Where(x => x.IsDead()))
        {
            _targets.Remove(target.Position);
        }
    }

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

    private void DetermineRadarSize(Size clientSize)
    {
        _radarOriginPoint = new Point(clientSize.Width / 2, clientSize.Height - 10);
    }

    private void DetermineRadius()
    {
        Radius = Math.Min(_radarOriginPoint.X, _radarOriginPoint.Y);
    }

    private void DrawOutherArc(Graphics gfx)
    {
        var rect = new Rectangle(_radarOriginPoint.X - Radius, _radarOriginPoint.Y - Radius, Radius * 2, Radius * 2);
        gfx.DrawArc(_pen, rect, 180, 180);
        gfx.DrawString(MaxDistance + "cm", _font, _brush, _radarOriginPoint.X - 25, rect.Y - 25);
    }

    private void DrawInnerArc(Graphics gfx)
    {
        var rect = new Rectangle(_radarOriginPoint.X - Radius / 2, _radarOriginPoint.Y - Radius / 2, Radius, Radius);
        gfx.DrawArc(_pen, rect, 180, 180);
        gfx.DrawString((MaxDistance / 2) + "cm", _font, _brush, _radarOriginPoint.X - 25, rect.Y - 25);
    }

    private void DrawBottomLine(Graphics gfx)
    {
        gfx.DrawLine(_pen, _radarOriginPoint.X - Radius, _radarOriginPoint.Y, _radarOriginPoint.X + Radius, _radarOriginPoint.Y);
    }

    private void DrawScanLine(Graphics gfx)
    {
        var lineEndPoint = WorldToScreen(_angle, Radius);
        gfx.DrawLine(_pen, _radarOriginPoint.X, _radarOriginPoint.Y, lineEndPoint.X, lineEndPoint.Y);
    }

    private Point WorldToScreen(Angle angle, int distance)
    {
        var zX = (int)(distance * Math.Cos(angle.Radians));
        var zY = (int)(distance * Math.Sin(angle.Radians));
        var dX = _radarOriginPoint.X + zX;
        var dY = _radarOriginPoint.Y + zY;

        return new Point(dX, dY);
    }
}
