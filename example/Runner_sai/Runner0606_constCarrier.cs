// Runner0522.cs
using System;
using System.Diagnostics;
using System.Threading;
using AUTD3Sharp;
using AUTD3Sharp.Gain;
using AUTD3Sharp.Modulation;
using AUTD3Sharp.Utils;
using static AUTD3Sharp.Units;


namespace Runner_sai
{
    public static class Runner0606_constCarrier
    {
        // 整数の最大公約数をユークリッドの互除法で計算
        static int Gcd(int a, int b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }
        public static void Run(Controller autd)
        {
            // 無音状態の初期化
            autd.Send(new Silencer());

            // 焦点位置設定
            var focus = new Focus(
                pos: autd.Center() + new Vector3(0, 0, 150),
                option: new FocusOption()
            );

            // 焦点位置確認用
            var m = new Sine(freq: 30 * Hz, option: new SineOption());
            autd.Send((m, focus));
            Thread.Sleep(10000);
            autd.Send((new Silencer(), new Null()));
            Thread.Sleep(3000);

            int[] carrierList = { 10, 30, 200 };
            int[] envelopeList = { 3, 6, 10 };

            
            foreach (int cFreq in carrierList)
            {
                foreach (int eFreq in envelopeList)
                {
                    Console.WriteLine($"→carrier={cFreq}Hz");

                    var sin_wave = new Sine(freq: eFreq * Hz, option: new SineOption());
                    autd.Send((sin_wave, focus));
                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));
                    
                    Console.WriteLine($"→ carrier={cFreq}Hz, envelope={eFreq}Hz");

                    // ① 波形生成
                    const int sampleRate = 1000; // サンプリング周波数 
                    int gcd = Gcd(cFreq, eFreq);  // carrier と envelope の最大公約数
                    int periodSamples = sampleRate / gcd; // 波形周期: 1/gcd(s) * sampleRate(data/s)

                    var buffer = new byte[periodSamples];
                    for (int i = 0; i < periodSamples; i++)
                    {
                        double t = i / (double)sampleRate;

                        double carrier = Math.Sin(2 * Math.PI * cFreq * t);
                        double envelope = Math.Sin(2 * Math.PI * eFreq * t);
                        double val = carrier * envelope;

                        int intLevel = (int)Math.Round((val * 0.5 + 0.5) * 255.0);
                        buffer[i] = (byte)intLevel;
                    }

                    // ② 波形送信(10s)
                    var wave = new AUTD3Sharp.Modulation.Custom(
                        buffer: buffer,
                        samplingConfig: 1000f * Hz
                    );
                    autd.Send((wave, focus));

                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));
                    Console.WriteLine("一個前の波と同じ:s 異なる:d / 終了:enter");

                    // ③ 有効なキーが押されるまで繰り返し待ち
                    while (true)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.Enter)
                        {
                            Console.WriteLine("終了します");
                            autd.Close();
                            return;
                        }
                        if (key == ConsoleKey.D || key == ConsoleKey.S)
                        {
                            Console.WriteLine($"{key}");
                            break;
                        }
                        Console.WriteLine("無効なキーです。Enter または d/s を押してください");
                    }

                }
            }
        }
    }
}


