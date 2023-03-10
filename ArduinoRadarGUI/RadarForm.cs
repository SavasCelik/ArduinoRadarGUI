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
        _radar.StartSerialPortListening();

        // Set up the timer for update
        _timer = new Timer
        {
            Interval = 20
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        Refresh();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        _radar.Draw(e.Graphics, ClientSize);
    }
}