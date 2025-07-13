// Program.cs
using System;
using System.Linq;
using AUTD3Sharp;
using AUTD3Sharp.Utils;       // ← Point3, Quaternion, EulerAngles をここから
using AUTD3Sharp.Link;
using static AUTD3Sharp.Units; // rad, Hz など
using Runner_sai;

namespace TwinCAT_sai
{
    class Program
    {
        // Quaternion同士の掛け算を実装
        static Quaternion MultiplyQuaternions(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(
                q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z,  // W
                q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y,  // X
                q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X,  // Y
                q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W   // Z
            );
        }

        static void Main(string[] args)
        {
            // System.Environment.SetEnvironmentVariable("RUST_LOG", "autd3=DEBUG");
            // AUTD3Sharp.Tracing.Init();
            
            // AUTD位置設定用の定数
            const float OscillatorRadius = 0.00508f; // 振動子の半径
            const float width_x_right = 0.0004f;  // 向かって右側のAUTDと正面のAUTDのAUTD座標におけるx軸方向のずれ
            const float width_y_right = 0.0005f;  // 向かって右側のAUTDと正面のAUTDのAUTD座標におけるy軸方向のずれ
            const float width_x_left = -0.001f;  // 向かって左側のAUTDと正面のAUTDのAUTD座標におけるx軸方向のずれ
            const float width_y_left = 0.0005f;  // 向かって左側のAUTDと正面のAUTDのAUTD座標におけるy軸方向のずれ
            const float WidthOriginToOscillatorEdge = 0.1778f; //AUTDの原点位置から、width方向(x軸)に進んで、一番端のオシレーターの端までの長さ[m]

            AUTD3[] devices = new AUTD3[]
            {
                // 1~4
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left, WidthOriginToOscillatorEdge+AUTD3.DeviceWidth + width_y_left, -AUTD3.DeviceHeight),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, -MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left, WidthOriginToOscillatorEdge+AUTD3.DeviceWidth + width_y_left, 0),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, -MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left, WidthOriginToOscillatorEdge + width_y_left, 0),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, -MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left, WidthOriginToOscillatorEdge + width_y_left, -AUTD3.DeviceHeight),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, -MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
                

                // 5~8
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, 0, -AUTD3.DeviceHeight),
                        rot: new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0)),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, 0, 0),
                        rot: new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0)),
                new AUTD3(
                        pos: new Point3(0, 0, 0),
                        rot: new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0)),
                new AUTD3(
                        pos: new Point3(0, 0, -AUTD3.DeviceHeight),
                        rot: new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0)),
                
                // 9~12
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, OscillatorRadius + width_y_right, -AUTD3.DeviceHeight),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, OscillatorRadius + width_y_right, 0),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, OscillatorRadius+AUTD3.DeviceWidth + width_y_right, 0),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, OscillatorRadius+AUTD3.DeviceWidth + width_y_right, -AUTD3.DeviceHeight),
                        rot: MultiplyQuaternions(new Quaternion(MathF.Cos(MathF.PI / 4), 0, 0, MathF.Sin(MathF.PI / 4)), new Quaternion(MathF.Cos(MathF.PI / 4), -MathF.Sin(MathF.PI / 4), 0, 0))),
            };

            // TwinCAT 経由でコントローラをオープン
            using var autd = Controller.Open(devices, new TwinCAT());

            // Runner で一連のデモを実行
            Runner0713_sum.Run(autd);
        }
    }
}
