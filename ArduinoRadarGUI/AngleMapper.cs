namespace ArduinoRadarGUI
{
    public static class AngleHelper
    {
        public static int MapAngle(int angle) => 360 - angle;
        public static double DegreesToRadians(int angle) => angle * Math.PI / 180; 
    }
}
