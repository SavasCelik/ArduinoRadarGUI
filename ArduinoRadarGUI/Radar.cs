namespace ArduinoRadarGUI;

public class Radar
{
    private readonly Pen _pen;
    private int _direction;
    private Angle _angle;
    private int _radius;
    private Point _radarOriginPoint;
    private IDictionary<Point, RadarTarget> _targets;

    public Radar()
    {
        _pen = new Pen(Color.Green, 3);
        _targets = new Dictionary<Point, RadarTarget>();
        _angle = new Angle(0);
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
        _radarOriginPoint = new Point(clientSize.Width / 2, clientSize.Height - 30);
    }

    private void DetermineRadius()
    {
        _radius = Math.Min(_radarOriginPoint.X, _radarOriginPoint.Y);
    }

    private void DrawOutherArc(Graphics gfx)
    {
        var outherArc = new Rectangle(_radarOriginPoint.X - _radius, _radarOriginPoint.Y - _radius, _radius * 2, _radius * 2);
        gfx.DrawArc(_pen, outherArc, 180, 180);
    }

    private void DrawInnerArc(Graphics gfx)
    {
        var innerArc = new Rectangle(_radarOriginPoint.X - _radius / 2, _radarOriginPoint.Y - _radius / 2, _radius, _radius);
        gfx.DrawArc(_pen, innerArc, 180, 180);
    }

    private void DrawBottomLine(Graphics gfx)
    {
        gfx.DrawLine(_pen, _radarOriginPoint.X - _radius, _radarOriginPoint.Y, _radarOriginPoint.X + _radius, _radarOriginPoint.Y);
    }

    private void DrawScanLine(Graphics gfx)
    {
        var lineEndPoint = WorldToScreen(_angle, _radius);
        gfx.DrawLine(_pen, _radarOriginPoint.X, _radarOriginPoint.Y, lineEndPoint.X, lineEndPoint.Y);
    }

    private Point WorldToScreen(Angle angle, int distance)
    {
        int x = _radarOriginPoint.X + (int)(distance * Math.Cos(angle.Radians));
        int y = _radarOriginPoint.Y + (int)(distance * Math.Sin(angle.Radians));

        return new Point(x, y);
    }
}
