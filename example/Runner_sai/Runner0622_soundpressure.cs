// Runner0622_soundpressure.cs - 音圧波形識別実験
using System;
using System.Diagnostics;
using System.IO;
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
            var focusPosition = autd.Center() + new Vector3(0, 0, 150);
            var focus = new Focus(
                pos: focusPosition,
                option: new FocusOption()
            );
            Console.WriteLine($"width={AUTD3.DeviceWidth}, height={AUTD3.DeviceHeight}");
            Console.WriteLine($"{focusPosition.X}, {focusPosition.Y}, {focusPosition.Z}");

            // 焦点位置確認用
            var m = new Sine(freq: 150 * Hz, option: new SineOption());
            autd.Send((m, focus));
            Thread.Sleep(10000);
            autd.Send((new Silencer(), new Null()));
            Thread.Sleep(2000);

            // --------------------------------
            // 　　　　　　　　 実験
            // --------------------------------

            Console.WriteLine("=== 音圧波形識別実験開始 ===");
            Console.WriteLine("Wave1: 波形AorB → Wave2: 波形AorB → Wave3: 波形AorB → キー入力(Wave3がWave1/Wave2のどちらと同じに感じたか)");
            Console.WriteLine("Wave1と同じ: ← キー, Wave2と同じ: → キー, 終了: Enter");

            // CSV保存の準備
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string csvFileName = $"soundpressure_experiment_{timestamp}.csv";
            string resultDir = Path.Combine("result", "Runner0622_soundpressure");
            string csvPath = Path.Combine(resultDir, csvFileName);
            Directory.CreateDirectory(resultDir);
            using (var writer = new StreamWriter(csvPath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("Timestamp,EnvelopeFreq,A_CarrierFreq,B_CarrierFreq,TrialSet,Wave1,Wave2,Wave3,Wave1_Amplitude,Wave2_Amplitude,Wave3_Amplitude,UserAnswer,ResponseTime_ms");
            }
            Console.WriteLine($"実験結果は {csvFileName} に保存されます");

            // 実験設定
            int envelopeFreq = 6; // Hz
            int totalTrials = 0;

            while (true)
            {
                var (A_carrier_Freq, B_carrier_Freq) = RandomUtil.NextFreqPair0622();
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

                var random = new Random();

                // 10回の試行セット
                for (int trialSet = 1; trialSet <= 10; trialSet++)
                {
                    Console.WriteLine($"\n試行セット {trialSet}/10:");
                    
                    string[] playedWaves = new string[3];
                    double[] amplitudeScales = new double[3];

                    // 3回のランダム波形再生
                    for (int waveIndex = 0; waveIndex < 3; waveIndex++) 
                    {
                        // ランダムに波形AまたはBを選択
                        bool isWaveA = random.Next(2) == 0;
                        var baseWave = isWaveA ? waveA : waveB;
                        playedWaves[waveIndex] = isWaveA ? "A" : "B";

                        // ランダムな振幅スケール (0.1 ~ 1.0, 0.1刻み)
                        double amplitudeScale = (random.Next(5, 11)) * 0.1; // 0.1, 0.2, ..., 1.0
                        amplitudeScales[waveIndex] = amplitudeScale;
                        
                        // 振幅スケールを適用した新しい波形を生成
                        // 振動圧をamplitudeScale倍 → 音圧は√amplitudeScale倍
                        var scaledWave = new byte[baseWave.Length];
                        for (int i = 0; i < baseWave.Length; i++)
                        {
                            scaledWave[i] = (byte)(baseWave[i] * Math.Sqrt(amplitudeScale));
                        }
                        
                        var testWave = new AUTD3Sharp.Modulation.Custom(
                            buffer: scaledWave,
                            samplingConfig: 1000f * Hz
                        );

                        Console.WriteLine($"波形{waveIndex + 1}再生中... ({playedWaves[waveIndex]}, 振幅: {amplitudeScale:F2})");
                        autd.Send((testWave, focus));
                        Thread.Sleep(5000);
                        autd.Send((new Silencer(), new Null()));
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine($"Wave1と同じ: ← キー, Wave2と同じ: → キー, 終了: Enter");
                    Console.WriteLine($"再生された波形: Wave1={playedWaves[0]}, Wave2={playedWaves[1]}, Wave3={playedWaves[2]}");

                    // 応答時間測定開始
                    var stopwatch = Stopwatch.StartNew();

                    // キー入力待ち
                    while (true)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.Enter)
                        {
                            Console.WriteLine($"\n=== 実験完了 ===");
                            Console.WriteLine($"総試行数: {totalTrials}");
                            Console.WriteLine($"結果は {csvFileName} に保存されました");
                            autd.Close();
                            return;
                        }
                        if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
                        {
                            stopwatch.Stop();
                            long responseTime = stopwatch.ElapsedMilliseconds;
                            
                            string userAnswer = key == ConsoleKey.LeftArrow ? "Wave1" : "Wave2";
                            totalTrials++;

                            // CSVに結果を保存
                            using (var writer = new StreamWriter(csvPath, true, System.Text.Encoding.UTF8))
                            {
                                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                writer.WriteLine($"{currentTime},{envelopeFreq},{A_carrier_Freq},{B_carrier_Freq},{trialSet},{playedWaves[0]},{playedWaves[1]},{playedWaves[2]},{amplitudeScales[0]:F3},{amplitudeScales[1]:F3},{amplitudeScales[2]:F3},{userAnswer},{responseTime}");
                            }

                            Console.WriteLine($"回答: {userAnswer} (応答時間: {responseTime}ms)");
                            break;
                        }
                        Console.WriteLine("無効なキーです。← または → または Enter を押してください");
                    }

                    Thread.Sleep(2000); // 次の試行まで間隔
                }
            }
        }
    }
}
