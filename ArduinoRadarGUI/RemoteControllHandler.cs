using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoRadarGUI
{
    public class RemoteControllHandler
    {
        private readonly Radar _radar;

        public RemoteControllHandler(Radar radar)
        {
            _radar = radar;
        }

        public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("");
        }
    }
}
