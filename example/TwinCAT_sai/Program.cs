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
                // 1列目
                new AUTD3(new Point3(  AUTD3.DeviceWidth,  AUTD3.DeviceHeight * 3/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(  0,                   AUTD3.DeviceHeight * 3/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(- AUTD3.DeviceWidth,   AUTD3.DeviceHeight * 3/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(-(1 + MathF.Sqrt(2)/2) * AUTD3.DeviceWidth,
                                    AUTD3.DeviceHeight * 3/2,
                                    -MathF.Sqrt(2)/2 * AUTD3.DeviceWidth),
                        Quaternion.Identity),

                // 2列目
                new AUTD3(new Point3(-(1 + MathF.Sqrt(2)/2) * AUTD3.DeviceWidth,
                                    AUTD3.DeviceHeight *   1/2,
                                    -MathF.Sqrt(2)/2 * AUTD3.DeviceWidth),
                        Quaternion.Identity),
                new AUTD3(new Point3(- AUTD3.DeviceWidth,   AUTD3.DeviceHeight *   1/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(  0,                   AUTD3.DeviceHeight *   1/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(  AUTD3.DeviceWidth,   AUTD3.DeviceHeight *   1/2,  0), Quaternion.Identity),

                // 3列目
                new AUTD3(new Point3(  AUTD3.DeviceWidth,  -AUTD3.DeviceHeight *   1/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(  0,                -AUTD3.DeviceHeight *   1/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(- AUTD3.DeviceWidth, -AUTD3.DeviceHeight *   1/2,  0), Quaternion.Identity),
                new AUTD3(new Point3(-(1 + MathF.Sqrt(2)/2) * AUTD3.DeviceWidth,
                                    -AUTD3.DeviceHeight *   1/2,
                                    -MathF.Sqrt(2)/2 * AUTD3.DeviceWidth),
                        Quaternion.Identity),
            };

            // TwinCAT 経由でコントローラをオープン
            using var autd = Controller.Open(devices, new TwinCAT());

            // Runner で一連のデモを実行
            Runner0522.Run(autd);
        }
    }
}
