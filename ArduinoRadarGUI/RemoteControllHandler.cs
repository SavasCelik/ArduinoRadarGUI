using System.IO.Ports;

namespace ArduinoRadarGUI;

public class RemoteControllHandler
{
    private readonly Radar _radar;
    private readonly SerialPort _serialPort;

    public RemoteControllHandler(Radar radar)
    {
        _radar = radar;
        _serialPort = new SerialPort("COM6", 9600);
        _serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
    }

    public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var message = _serialPort.ReadLine();

        if (int.TryParse(message, out var servoAngleInDegree))
        {
            _radar.Update(-servoAngleInDegree);
        }
    }

    public void Start()
    {
        _serialPort.Open();
    }

    public void Stop()
    {
        _serialPort.Close();
    }
}
