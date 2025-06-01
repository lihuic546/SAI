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
            autd.Send(focus);

            // 焦点位置確認用
            var m = new Sine(freq: 150 * Hz, option: new SineOption());
            autd.Send(m);
            Thread.Sleep(5000);
            autd.Send(new Silencer());

            while (true)
            {
                // ① 周波数をランダム取得->波形作成
                var (cFreq, eFreq) = RandomUtil.NextFreqPair();
                Console.WriteLine($"→ carrier={cFreq}Hz, envelope={eFreq}Hz");

                const int sampleRate = 6000; // サンプリング周波数 
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
                var wave = new Custom(
                    buffer: buffer,
                    samplingConfig: sampleRate * Hz
                );
                autd.Send(wave);

                Thread.Sleep(10000);
                autd.Send(new Silencer());
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
                    Console.WriteLine("無効なキーです。Enter または D/S を押してください");
                }
            }
        }
    }
}


