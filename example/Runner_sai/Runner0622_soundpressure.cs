// Runner0622_soundpressure.cs - 音圧波形識別実験
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
    public static class Runner0622_soundpressure
    {
        // 整数の最大公約数をユークリッドの互除法で計算
        static int Gcd(int a, int b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }

        // 音圧波形生成（envelope × carrier → √(envelope × carrier)）
        static byte[] GenerateSoundPressureWave(int envelopeFreq, int carrierFreq, int sampleRate = 1000)
        {
            int gcd = carrierFreq == 0 ? envelopeFreq : Gcd(carrierFreq, envelopeFreq);
            int periodSamples = sampleRate / gcd;

            var buffer = new byte[periodSamples];
            for (int i = 0; i < periodSamples; i++)
            {
                double t = i / (double)sampleRate;

                double envelope = (1 + Math.Sin(2 * Math.PI * envelopeFreq * t)) / 2;

                // 振動圧
                double vibrationPressure;
                if (carrierFreq == 0)
                {
                    vibrationPressure = envelope;
                }
                else
                {
                    double carrier = (1 + Math.Sin(2 * Math.PI * carrierFreq * t)) / 2;
                    vibrationPressure = envelope * carrier;
                }

                // 音圧: √振動圧
                double soundPressure = Math.Sqrt(Math.Abs(vibrationPressure));

                // 音圧を0-255の範囲に変換
                int intLevel = (int)Math.Round(soundPressure * 255.0);
                buffer[i] = (byte)intLevel;
            }

            return buffer;
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
                pos: autd.Center() + new Vector3(0, 0, 2 * AUTD3.DeviceWidth),
                option: new FocusOption()
            );

            // 焦点位置確認用
            var m = new Sine(freq: 30 * Hz, option: new SineOption());
            autd.Send((m, focus));
            Thread.Sleep(5000);
            autd.Send((new Silencer(), new Null()));
            Thread.Sleep(2000);

            // --------------------------------
            // 　　　　　　　　 実験
            // --------------------------------

            Console.WriteLine("=== 音圧波形識別実験開始 ===");
            Console.WriteLine("波形A → 波形B → ランダム波形 → キー入力");
            Console.WriteLine("Aと同じ: ← キー, Bと同じ: → キー, 終了: Enter");

            // 実験設定
            int envelopeFreq = 6; // Hz
            int totalTrials = 0;
            int correctAnswers = 0;

            while (true)
            {
                var (A_carrier_Freq, B_carrier_Freq) = RandomUtil.NextFreqPair();
                Console.WriteLine($"\n--- A: {A_carrier_Freq} B: {B_carrier_Freq} ---");

                // 波形A生成
                var waveA = GenerateSoundPressureWave(envelopeFreq, A_carrier_Freq);
                var modulationA = new AUTD3Sharp.Modulation.Custom(
                    buffer: waveA,
                    samplingConfig: 1000f * Hz
                );

                // 波形B生成
                var waveB = GenerateSoundPressureWave(envelopeFreq, B_carrier_Freq);
                var modulationB = new AUTD3Sharp.Modulation.Custom(
                    buffer: waveB,
                    samplingConfig: 1000f * Hz
                );

                // 5回の試行
                for (int trial = 1; trial <= 5; trial++)
                {
                    Console.WriteLine($"\n試行 {trial}/5:");

                    // 波形A再生（5秒）
                    Console.WriteLine("波形A再生中...");
                    autd.Send((modulationA, focus));
                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));
                    Thread.Sleep(1000);

                    // 波形B再生（5秒）
                    Console.WriteLine("波形B再生中...");
                    autd.Send((modulationB, focus));
                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));
                    Thread.Sleep(1000);

                    // ランダムに波形AまたはBを選択
                    bool isWaveA = random.Next(2) == 0;
                    var testWave = isWaveA ? modulationA : modulationB;
                    string correctAnswer = isWaveA ? "A" : "B";

                    Console.WriteLine("テスト波形再生中...");
                    autd.Send((testWave, focus));
                    Thread.Sleep(5000);
                    autd.Send((new Silencer(), new Null()));

                    Console.WriteLine($"この波形は A か B か？ (← キー: A, → キー: B, Enter: 終了)");

                    // キー入力待ち
                    while (true)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.Enter)
                        {
                            Console.WriteLine("実験を終了します");
                            autd.Close();
                            return;
                        }
                        if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
                        {
                            string userAnswer = key == ConsoleKey.LeftArrow ? "A" : "B";
                            bool isCorrect = userAnswer == correctAnswer;
                            
                            totalTrials++;
                            if (isCorrect) correctAnswers++;

                            Console.WriteLine($"回答: {userAnswer}, 正解: {correctAnswer} → {(isCorrect ? "正解!" : "不正解")}");
                            Console.WriteLine($"現在の正答率: {correctAnswers}/{totalTrials} ({(double)correctAnswers/totalTrials*100:F1}%)");
                            break;
                        }
                        Console.WriteLine("無効なキーです。← または → または Enter を押してください");
                    }

                    Thread.Sleep(2000); // 次の試行まで間隔
                }
            }

            Console.WriteLine($"\n=== 実験完了 ===");
            Console.WriteLine($"最終正答率: {correctAnswers}/{totalTrials} ({(double)correctAnswers/totalTrials*100:F1}%)");
            autd.Close();
        }
    }
}
