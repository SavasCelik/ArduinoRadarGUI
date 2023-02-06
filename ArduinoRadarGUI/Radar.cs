using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRadarGUI;

public class Radar
{
    private readonly Pen _pen;
    private int _direction;
    private int _angle;
    private int _radius;
    private Point _radarOriginPoint;

    public Radar()
    {
        _pen = new Pen(Color.Green, 3);
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

    public void Draw(Graphics gfx, Size clientSize)
    {
        DetermineRadarSize(clientSize);
        DetermineRadius();
        DrawOutherArc(gfx);
        DrawInnerArc(gfx);
        DrawBottomLine(gfx);
        DrawScanLine(gfx);
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
