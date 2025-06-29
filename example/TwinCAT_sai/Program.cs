// Program.cs
using System;
using System.Linq;
using AUTD3Sharp;
using AUTD3Sharp.Utils;       // ← Point3, Quaternion, EulerAngles をここから
using AUTD3Sharp.Link;
using Runner_sai;
using static AUTD3Sharp.Units; // rad, Hz など

namespace TwinCAT_sai
{
    class Program
    {
        static void Main(string[] args)
        {
            // System.Environment.SetEnvironmentVariable("RUST_LOG", "autd3=DEBUG");
            // AUTD3Sharp.Tracing.Init();
            
            AUTD3[] devices = new AUTD3[]
            {
                // 1~4
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, 0, 2 * AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, -AUTD3.DeviceHeight, 2 * AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, -AUTD3.DeviceHeight, AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(-AUTD3.DeviceWidth, 0, AUTD3.DeviceWidth),
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
                        pos: new Point3(AUTD3.DeviceWidth, 0, AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(AUTD3.DeviceWidth, -AUTD3.DeviceHeight, AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(AUTD3.DeviceWidth, -AUTD3.DeviceHeight, 2 * AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
                new AUTD3(
                        pos: new Point3(AUTD3.DeviceWidth, 0, 2 * AUTD3.DeviceWidth),
                        rot: EulerAngles.Zyz(0 * rad, - MathF.PI / 2 * rad, 0 * rad)),
            };

            // TwinCAT 経由でコントローラをオープン
            using var autd = Controller.Open(devices, new TwinCAT());

            // Runner で一連のデモを実行
            Runner0606_constCarrier.Run(autd);
        }
    }
}
