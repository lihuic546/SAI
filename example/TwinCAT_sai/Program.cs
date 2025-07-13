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
        static void Main(string[] args)
        {
            // System.Environment.SetEnvironmentVariable("RUST_LOG", "autd3=DEBUG");
            // AUTD3Sharp.Tracing.Init();

            const float OscillatorRadius = 0.00508f * 1000; // 振動子の半径 AUTD位置設定用
            const float width_x_right = 0.0004f * 1000;  // 向かって右側のAUTDと正面のAUTDのAUTD座標におけるx軸方向のずれ(正面のAUTDのtransducerの端に対する、右側のAUTDのtransducerの面の位置)
            const float width_z_right = 0.0005f * 1000;  // 向かって右側のAUTDと正面のAUTDのAUTD座標におけるy軸方向のずれ(正面のAUTDのtransducerの面に対する、右側のAUTDのtransducerの端の位置)
            const float width_x_left = 0.001f * 1000;  // 向かって左側のAUTDと正面のAUTDのAUTD座標におけるx軸方向のずれ(正面のAUTDのtransducerの端に対する、左側のAUTDのtransducerの面の位置)
            const float width_z_left = 0.0005f * 1000;  // 向かって左側のAUTDと正面のAUTDのAUTD座標におけるy軸方向のずれ(正面のAUTDのtransducerの面に対する、左側のAUTDのtransducerの端の位置)
            const float WidthOriginToOscillatorEdge = 0.1778f * 1000; //AUTDの原点位置から、width方向(x軸)に進んで、一番端のオシレーターの端までの長さ[m]


            AUTD3[] devices = new AUTD3[]
            {
                // 1~4
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius - width_x_left, 0, width_z_left + WidthOriginToOscillatorEdge + AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius - width_x_left, -AUTD3.DeviceHeight, width_z_left + WidthOriginToOscillatorEdge + AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius - width_x_left, -AUTD3.DeviceHeight, width_z_left + WidthOriginToOscillatorEdge),
                        rot: EulerAngles.Zyz(0 * rad, MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth - OscillatorRadius - width_x_left, 0, width_z_left + WidthOriginToOscillatorEdge),
                        rot: EulerAngles.Zyz(0 * rad, MathF.PI / 2 * rad, 0 * rad)),

                // 5~8
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, 0, 0),
                        rot: Quaternion.Identity),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, -AUTD3.DeviceHeight, 0),
                        rot: Quaternion.Identity),
                new AUTD3(
                        pos: new Point3(0, -AUTD3.DeviceHeight, 0),
                        rot: Quaternion.Identity),
                new AUTD3(
                        pos: Point3.Origin,
                        rot: Quaternion.Identity),

                // 9~12
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, 0, width_z_right + OscillatorRadius),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, -AUTD3.DeviceHeight, width_z_right + OscillatorRadius),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, -AUTD3.DeviceHeight, width_z_right + OscillatorRadius + AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(WidthOriginToOscillatorEdge + width_x_right, 0, width_z_right + OscillatorRadius + AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
            };
            Console.WriteLine($"width={AUTD3.DeviceWidth}, 変数={WidthOriginToOscillatorEdge}");

            // TwinCAT 経由でコントローラをオープン
            using var autd = Controller.Open(devices, new TwinCAT());

            // Runner で一連のデモを実行
            Runner0622_soundpressure.Run(autd);
        }
    }
}
