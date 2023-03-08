using System.IO.Ports;

namespace ArduinoRadarGUI;

public class SerialPortHandler
{
    private readonly Radar _radar;
    private readonly SerialPort _serialPort;

    public SerialPortHandler(Radar radar)
    {
        _radar = radar;
        _serialPort = new SerialPort("COM6", 9600);
        _serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
    }

    public void StartListening()
    {
        _serialPort.Open();
    }

    public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var message = _serialPort.ReadLine();

        if (int.TryParse(message, out var servoAngleInDegree))
        {
            _radar.Update(-servoAngleInDegree);
        }
    }
}
