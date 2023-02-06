namespace ArduinoRadarGUI;

public class Angle
{
    public int Degrees { get; private set; }
    public double Radians { get; private set;}

    public Angle(int angleInDegrees)
    {
        SetAngle(angleInDegrees);
    }

    public void SetAngle(int angleInDegrees)
    {
        Degrees = angleInDegrees;
        Radians = Degrees * Math.PI / 180;
    }
}
