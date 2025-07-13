using System;

class QuaternionAnalysis
{
    static void Main()
    {
        Console.WriteLine("Quaternion Rotation Analysis");
        Console.WriteLine("============================");
        
        // Quaternions from your code
        var q1 = new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, -MathF.Sin(MathF.PI / 4));
        var q2 = new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, -MathF.Sin(MathF.PI / 4));
        var q3 = new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0);
        var q4 = new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, MathF.Sin(MathF.PI / 4));
        var q5 = new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0);
        
        AnalyzeQuaternion("Q1", q1);
        AnalyzeQuaternion("Q2", q2);
        AnalyzeQuaternion("Q3", q3);
        AnalyzeQuaternion("Q4", q4);
        AnalyzeQuaternion("Q5", q5);
    }
    
    static void AnalyzeQuaternion(string name, Quaternion q)
    {
        Console.WriteLine($"\n{name}: ({q.W:F3}, {q.X:F3}, {q.Y:F3}, {q.Z:F3})");
        
        // Convert quaternion to Euler angles (in radians)
        var (roll, pitch, yaw) = QuaternionToEuler(q);
        
        // Convert to degrees
        var rollDeg = roll * 180.0f / MathF.PI;
        var pitchDeg = pitch * 180.0f / MathF.PI;
        var yawDeg = yaw * 180.0f / MathF.PI;
        
        Console.WriteLine($"  Roll (X-axis):  {rollDeg:F1}°");
        Console.WriteLine($"  Pitch (Y-axis): {pitchDeg:F1}°");
        Console.WriteLine($"  Yaw (Z-axis):   {yawDeg:F1}°");
        
        // Also show the axis-angle representation
        var (axis, angle) = QuaternionToAxisAngle(q);
        var angleDeg = angle * 180.0f / MathF.PI;
        Console.WriteLine($"  Axis-Angle: ({axis.X:F3}, {axis.Y:F3}, {axis.Z:F3}) @ {angleDeg:F1}°");
    }
    
    static (float roll, float pitch, float yaw) QuaternionToEuler(Quaternion q)
    {
        // Convert quaternion to Euler angles (ZYX order)
        float sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        float cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        float roll = MathF.Atan2(sinr_cosp, cosr_cosp);

        float sinp = 2 * (q.W * q.Y - q.Z * q.X);
        float pitch = MathF.Abs(sinp) >= 1 ? MathF.CopySign(MathF.PI / 2, sinp) : MathF.Asin(sinp);

        float siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        float cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        float yaw = MathF.Atan2(siny_cosp, cosy_cosp);

        return (roll, pitch, yaw);
    }
    
    static ((float X, float Y, float Z) axis, float angle) QuaternionToAxisAngle(Quaternion q)
    {
        // Normalize quaternion
        float norm = MathF.Sqrt(q.W * q.W + q.X * q.X + q.Y * q.Y + q.Z * q.Z);
        q = new Quaternion(q.W / norm, q.X / norm, q.Y / norm, q.Z / norm);
        
        float angle = 2 * MathF.Acos(MathF.Abs(q.W));
        float s = MathF.Sqrt(1 - q.W * q.W);
        
        if (s < 0.001f) // avoid divide by zero
        {
            return ((1, 0, 0), 0);
        }
        
        return ((q.X / s, q.Y / s, q.Z / s), angle);
    }
}

public struct Quaternion
{
    public float W, X, Y, Z;
    
    public Quaternion(float w, float x, float y, float z)
    {
        W = w; X = x; Y = y; Z = z;
    }
}
