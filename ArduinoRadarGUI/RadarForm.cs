using Timer = System.Windows.Forms.Timer;

namespace ArduinoRadarGUI;

public partial class RadarForm : Form
{
    private Timer _timer;
    private int _angle = 0;
    private int _direction = 0;

    public RadarForm()
    {
        InitializeComponent();
        DoubleBuffered = true;
        BackColor = Color.Black;

        // Set up the timer to update the display
        _timer = new Timer();
        _timer.Interval = 20;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _angle += _direction;
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
        Refresh();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        int xc = ClientSize.Width / 2;
        int yc = ClientSize.Height - 30;
        int size = Math.Min(xc, yc);

        using (Pen pen = new Pen(Color.Green, 3))
        {
            //e.Graphics.FillEllipse(Brushes.Black, xc - size, yc - size, 2 * size, 2 * size);
            var rect = new Rectangle(xc - size, yc - size, size * 2, size * 2);
            e.Graphics.DrawArc(pen, rect, 180, 180);
            var rect2 = new Rectangle((xc - size / 2), (yc - size / 2), size, size);
            e.Graphics.DrawArc(pen, rect2, 180, 180);
            var botLine = new Rectangle(xc - size, yc, size * 2, size * 2);
            e.Graphics.DrawLine(pen, xc - size, yc, xc + size, yc);
        }

        using (Pen pen = new Pen(Color.Green, 3))
        {
            int x = xc + (int)(size * Math.Cos(_angle * Math.PI / 180));
            int y = yc + (int)(size * Math.Sin(_angle * Math.PI / 180));
            e.Graphics.DrawLine(pen, xc, yc, x, y);
        }
    }
}