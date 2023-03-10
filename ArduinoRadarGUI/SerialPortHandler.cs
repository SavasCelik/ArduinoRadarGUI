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

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var message = _serialPort.ReadLine();

        if (!string.IsNullOrWhiteSpace(message) && message.Contains(';'))
        {
            var messageSplit = message.Split(';');

            if (int.TryParse(messageSplit[0], out var servoAngleInDegree))
            {
                _radar.Update(-servoAngleInDegree);
            }

            if (int.TryParse(messageSplit[1], out var targetDistance) && targetDistance > 0)
            {
                var mappedDistance = targetDistance * _radar.Radius / _radar.MaxDistance;
                _radar.AddTarget(mappedDistance);
            }
        }
    }
}
