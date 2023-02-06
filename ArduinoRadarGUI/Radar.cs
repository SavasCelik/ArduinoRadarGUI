namespace ArduinoRadarGUI;

public class Radar
{
    private readonly Pen _pen;
    private readonly Brush _targetBrush;
    private int _direction;
    private int _angleInDegrees;
    private double _angleInRadians;
    private int _radius;
    private Point _radarOriginPoint;
    private IDictionary<Point, RadarTarget> _targets;

    public Radar()
    {
        _pen = new Pen(Color.Green, 3);
        _targetBrush = Brushes.Red;
        _targets = new Dictionary<Point, RadarTarget>();
    }

    public void AddTarget(int targetRawAngle, int targetDistance)
    {
        var targetAngleInDegrees = AngleHelper.MapAngle(targetRawAngle);
        var targetAngleInRadians = AngleHelper.DegreesToRadians(targetAngleInDegrees);

        int x = _radarOriginPoint.X + (int)(targetDistance * Math.Cos(targetAngleInRadians));
        int y = _radarOriginPoint.Y + (int)(targetDistance * Math.Sin(targetAngleInRadians));
        var position = new Point(x, y);

        if (_targets.TryGetValue(position, out var foundTarget))
        {
            foundTarget.ResetDeathTime();
        }
        else
        {
            var radarTarget = new RadarTarget(position);
            _targets.Add(position, radarTarget);
        }
    }

    public void Update()
    {
        if (_angleInDegrees <= -180)
        {
            _angleInDegrees = -180;
            _direction = 3;
        }
        else if (_angleInDegrees >= 0)
        {
            _angleInDegrees = 0;
            _direction = -3;
        }

        var deltaAngle = _angleInDegrees + _direction;
        SetAngle(deltaAngle);
    }

    public void Update(int angleInDegrees)
    {
        SetAngle(angleInDegrees);
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
            gfx.FillEllipse(_targetBrush, target.Position.X, target.Position.Y, 10, 10);
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
        int x = _radarOriginPoint.X + (int)(_radius * Math.Cos(_angleInRadians));
        int y = _radarOriginPoint.Y + (int)(_radius * Math.Sin(_angleInRadians));
        gfx.DrawLine(_pen, _radarOriginPoint.X, _radarOriginPoint.Y, x, y);
    }

    private void SetAngle(int angleInDegrees)
    {
        _angleInDegrees= angleInDegrees;
        _angleInRadians = AngleHelper.DegreesToRadians(_angleInDegrees);
    }
}
