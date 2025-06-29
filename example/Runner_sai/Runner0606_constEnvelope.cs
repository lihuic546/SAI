// envelope={2, 5, 10}固定
// "carrier:なしの波 10秒 → carrier:fHzの波 10秒"を順番に送信
// キー：一個前の波と同じ:s 異なる:d
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
    public static class Runner0606_constEnvelope
    {
        // 整数の最大公約数をユークリッドの互除法で計算
        static int Gcd(int a, int b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }
        public static void Run(Controller autd)
        {
            // --------------------------------
            // 　　　　　　　　 準備
            // --------------------------------

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

            // --------------------------------
            // 　　　　　　　　 実験
            // --------------------------------

            int[] carrierList = { 10, 40, 200 };
            int[] envelopeList = { 2, 5, 10 };

            foreach (int eFreq in envelopeList)
            {
                for (int cFreq = 10; cFreq < 200; cFreq += 20)
                {
                    // ----------
                    // no carrier
                    // ----------

                    // ① 周波数->波形作成
                    Console.WriteLine($"→envelope={eFreq}Hz");
                    var sin_wave = new Sine(freq: eFreq * Hz, option: new SineOption());
                    
                    // ② 波形送信(10s)
                    autd.Send((sin_wave, focus));
                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));
                
                    // ------------
                    // with carrier
                    // ------------

                    // ① 周波数->波形作成
                    Console.WriteLine($"→ carrier={cFreq}Hz, envelope={eFreq}Hz");

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

                    // --------------
                    
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


