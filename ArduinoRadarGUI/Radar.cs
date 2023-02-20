namespace ArduinoRadarGUI;

public class Radar
{
    public int Radius { get; private set; }
    private readonly Pen _pen;
    private readonly Brush _brush;
    private readonly Font _font;
    private int _direction;
    private Angle _angle;
    private Point _radarOriginPoint;
    private IDictionary<Point, RadarTarget> _targets;

    public Radar()
    {
        _pen = new Pen(Color.Green, 3);
        _brush = Brushes.Green;
        _font = new Font(FontFamily.GenericSerif, 10);
        _targets = new Dictionary<Point, RadarTarget>();
        _angle = new Angle(0);
        var rhandler = new RemoteControllHandler(this);
        rhandler.Start();
    }

    public void AddTarget(Angle angle, int targetDistance)
    {
        var position = WorldToScreen(angle, targetDistance);

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

    public void Update()
    {
        var angleInDegrees = _angle.Degrees;

        if (angleInDegrees <= -180)
        {
            angleInDegrees = -180;
            _direction = 3;
        }
        else if (angleInDegrees >= 0)
        {
            angleInDegrees = 0;
            _direction = -3;
        }

        var deltaAngle = angleInDegrees + _direction;
        _angle.SetAngle(deltaAngle);
    }

    public void Update(int angleInDegrees)
    {
        _angle.SetAngle(angleInDegrees);
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
            if (target.IsDead())
            {
                _targets.Remove(target.Position);
                continue;
            }
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
        gfx.DrawString("100cm", _font, _brush, _radarOriginPoint.X - 25, rect.Y - 25);
    }

    private void DrawInnerArc(Graphics gfx)
    {
        var rect = new Rectangle(_radarOriginPoint.X - Radius / 2, _radarOriginPoint.Y - Radius / 2, Radius, Radius);
        gfx.DrawArc(_pen, rect, 180, 180);
        gfx.DrawString("50cm", _font, _brush, _radarOriginPoint.X - 25, rect.Y - 25);
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
        int x = _radarOriginPoint.X + (int)(distance * Math.Cos(angle.Radians));
        int y = _radarOriginPoint.Y + (int)(distance * Math.Sin(angle.Radians));

        return new Point(x, y);
    }
}
