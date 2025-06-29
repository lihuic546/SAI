// Runner0622.cs - envelope固定でcarrierなし/carrier変更時の感触変化実験
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
    public static class Runner0622
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

            // 実験設定: envelope固定、carrierを変化させる
            int fixedEnvelopeFreq = 6; // 固定envelope周波数 (Hz)
            int[] carrierList = { 0, 10, 30, 50, 100, 150, 200 }; // 0はcarrierなし

            Console.WriteLine($"=== envelope固定({fixedEnvelopeFreq}Hz) carrier変化実験 ===");
            
            foreach (int cFreq in carrierList)
            {
                if (cFreq == 0)
                {
                    Console.WriteLine($"→ carrierなし, envelope={fixedEnvelopeFreq}Hz");
                    
                    // carrierなし（envelope波形のみ）
                    var sin_wave = new Sine(freq: fixedEnvelopeFreq * Hz, option: new SineOption());
                    autd.Send((sin_wave, focus));
                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));
                }
                else
                {
                    Console.WriteLine($"→ carrier={cFreq}Hz, envelope={fixedEnvelopeFreq}Hz");

                    // ① 波形生成（carrier * envelope）
                    const int sampleRate = 1000; // サンプリング周波数 
                    int gcd = Gcd(cFreq, fixedEnvelopeFreq);  // carrier と envelope の最大公約数
                    int periodSamples = sampleRate / gcd; // 波形周期: 1/gcd(s) * sampleRate(data/s)

                    var buffer = new byte[periodSamples];
                    for (int i = 0; i < periodSamples; i++)
                    {
                        double t = i / (double)sampleRate;

                        double carrier = Math.Sin(2 * Math.PI * cFreq * t);
                        double envelope = Math.Sin(2 * Math.PI * fixedEnvelopeFreq * t);
                        double val = carrier * envelope;

                        int intLevel = (int)Math.Round((val * 0.5 + 0.5) * 255.0);
                        buffer[i] = (byte)intLevel;
                    }

                    // ② 波形送信
                    var wave = new AUTD3Sharp.Modulation.Custom(
                        buffer: buffer,
                        samplingConfig: 1000f * Hz
                    );
                    autd.Send((wave, focus));
                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));
                }

                Console.WriteLine("前の波と同じ感触:s 異なる感触:d / 次へ:n / 終了:enter");

                // ③ 有効なキーが押されるまで繰り返し待ち
                while (true)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("実験終了します");
                        autd.Close();
                        return;
                    }
                    if (key == ConsoleKey.D || key == ConsoleKey.S || key == ConsoleKey.N)
                    {
                        Console.WriteLine($"入力: {key}");
                        break;
                    }
                    Console.WriteLine("無効なキーです。Enter, d, s, n のいずれかを押してください");
                }

                Thread.Sleep(1000); // 次の実験まで少し間隔を空ける
            }

            Console.WriteLine("=== 全実験完了 ===");
            
            // 追加実験: envelope変化でcarrier固定
            Console.WriteLine("\n=== carrier固定(50Hz) envelope変化実験 ===");
            int fixedCarrierFreq = 50;
            int[] envelopeList = { 3, 6, 10, 15 };

            foreach (int eFreq in envelopeList)
            {
                Console.WriteLine($"→ carrier={fixedCarrierFreq}Hz, envelope={eFreq}Hz");

                // ① 波形生成
                const int sampleRate2 = 1000;
                int gcd2 = Gcd(fixedCarrierFreq, eFreq);
                int periodSamples2 = sampleRate2 / gcd2;

                var buffer2 = new byte[periodSamples2];
                for (int i = 0; i < periodSamples2; i++)
                {
                    double t = i / (double)sampleRate2;

                    double carrier = Math.Sin(2 * Math.PI * fixedCarrierFreq * t);
                    double envelope = Math.Sin(2 * Math.PI * eFreq * t);
                    double val = carrier * envelope;

                    int intLevel = (int)Math.Round((val * 0.5 + 0.5) * 255.0);
                    buffer2[i] = (byte)intLevel;
                }

                // ② 波形送信
                var wave2 = new AUTD3Sharp.Modulation.Custom(
                    buffer: buffer2,
                    samplingConfig: 1000f * Hz
                );
                autd.Send((wave2, focus));
                Thread.Sleep(5000);
                autd.Send((new Silencer(), new Null()));

                Console.WriteLine("前の波と同じ感触:s 異なる感触:d / 次へ:n / 終了:enter");

                while (true)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("実験終了します");
                        autd.Close();
                        return;
                    }
                    if (key == ConsoleKey.D || key == ConsoleKey.S || key == ConsoleKey.N)
                    {
                        Console.WriteLine($"入力: {key}");
                        break;
                    }
                    Console.WriteLine("無効なキーです。Enter, d, s, n のいずれかを押してください");
                }

                Thread.Sleep(1000);
            }

            Console.WriteLine("=== 全実験完了 ===");
            autd.Close();
        }
    }
}
