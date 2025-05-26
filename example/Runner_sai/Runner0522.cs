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
    public static class RandomUtil
    {
        private static readonly Random _rng = new Random();
        public static (int carrierFreq, int envelopeFreq) NextFreqPair()
        {
            int[] carrierList  = { 10, 30, 200 };
            int[] envelopeList = { 3, 5, 10 };

            int i1, i2;
            do
            {
                i1 = _rng.Next(carrierList.Length);
                i2 = _rng.Next(envelopeList.Length);
            }
            while (carrierList[i1] == envelopeList[i2]);

            return (carrierList[i1], envelopeList[i2]);
        }
    }

    public static class Runner0522
    {
        public static void Run(Controller autd)
        {
            // 無音状態の初期化
            autd.Send(new Silencer());

            // フォーカス位置
            var focus = new Focus(
                pos: autd.Center() + new Vector3(0, 0, 150),
                option: new FocusOption()
            );
            autd.Send(focus);

            var m = new Sine(freq: 150 * Hz, option: new SineOption());
            autd.Send(m);
            Thread.Sleep(5000);
            autd.Send(new Silencer());
            
            while (true)
            {
                // ① 周波数をランダム取得->波形作成
                var (cFreq, eFreq) = RandomUtil.NextFreqPair();
                Console.WriteLine($"→ carrier={cFreq}Hz, envelope={eFreq}Hz");

                const int sampleRate = 1000;  // 1000 サンプル/s
                const double duration = 10; // 10s
                int length = (int)(sampleRate * duration);
                var wave = new double[length];


                for (int i = 0; i < length; i++)
                {
                    double t = i / (double)sampleRate;
                    double carrier = Math.Sin(2 * Math.PI * cFreq * t);
                    double envelope = Math.Sin(2 * Math.PI * eFreq * t);
                    wave[i] = carrier * envelope;
                }

                // new Custom(
                //     buffer: [0xFF, 0x00],
                //     samplingConfig: 4000f * Hz
                // );


                // ② 波形送信(10s)
                for (int i = 0; i < length; i++)
                {
                    byte intensity = (byte)((wave[i] + 1.0) * 0.5 * 255.0);
                    var igain = new Uniform(
                        intensity: new EmitIntensity(intensity),
                        phase: Phase.Zero
                    );
                    autd.Send(igain);
                    Thread.Sleep(1000 / sampleRate);
                    Console.WriteLine($"{i}");

                }

                // ③ 有効なキーが押されるまで繰り返し待ち
                autd.Send(new Silencer());
                Console.WriteLine("一個前の波と同じ:s 異なる:d / 終了:enter");

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
                    Console.WriteLine("無効なキーです。Enter または D/S を押してください");
                }
            }
        }
    }
}


