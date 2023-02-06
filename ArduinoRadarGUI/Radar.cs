namespace ArduinoRadarGUI;

public class Radar
{
    private readonly Pen _pen;
    private readonly Brush _targetBrush;
    private int _direction;
    public int _angle;
    private int _radius;
    private Point _radarOriginPoint;
    private IDictionary<Point, RadarTarget> _targets;

    public Radar()
    {
        _pen = new Pen(Color.Green, 3);
        _targetBrush = Brushes.Red;
        _targets = new Dictionary<Point, RadarTarget>();
    }

    public void AddTarget(int targetAngle, int targetDistance)
    {
        var mappedAngle = AngleMapper.MapAngle(targetAngle);
        int x = _radarOriginPoint.X + (int)(targetDistance * Math.Cos(mappedAngle * Math.PI / 180));
        int y = _radarOriginPoint.Y + (int)(targetDistance * Math.Sin(mappedAngle * Math.PI / 180));
        var position = new Point(x, y);

        if (!_targets.ContainsKey(position))
        {
            var radarTarget = new RadarTarget(position);
            _targets.Add(position, radarTarget);
        }
    }

    public void Update()
    {
        if (_angle <= -180)
        {
            _angle = -180;
            _direction = 3;
        }
        else if (_angle >= 0)
        {
            _angle = 0;
            _direction = -3;
        }

        _angle += _direction;
    }

    public void Update(int angle)
    {
        _angle = angle;
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
        int x = _radarOriginPoint.X + (int)(_radius * Math.Cos(_angle * Math.PI / 180));
        int y = _radarOriginPoint.Y + (int)(_radius * Math.Sin(_angle * Math.PI / 180));
        gfx.DrawLine(_pen, _radarOriginPoint.X, _radarOriginPoint.Y, x, y);
    }
}
