using Timer = System.Windows.Forms.Timer;

namespace ArduinoRadarGUI;

public partial class RadarForm : Form
{
    private readonly Timer _timer;
    private readonly Radar _radar;

    public RadarForm()
    {
        InitializeComponent();
        DoubleBuffered = true;
        BackColor = Color.Black;
        _radar = new Radar();

        // Set up the timer for update
        _timer = new Timer();
        _timer.Interval = 20;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _radar.Update();
        Refresh();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        _radar.Draw(e.Graphics, ClientSize);
    }

    private void RadarForm_KeyPress(object sender, KeyPressEventArgs e)
    {
        var distanz = 50 * _radar._radius / 100;
        var angleInDegrees = new Angle(360 - 45);
        _radar.AddTarget(angleInDegrees, distanz);
    }
}