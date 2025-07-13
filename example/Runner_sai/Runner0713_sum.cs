// Runner0713_sum.cs - 足し合わせ波形識別実験
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
    public static class Runner0713_sum
    {
        // 整数の最大公約数をユークリッドの互除法で計算
        static int Gcd(int a, int b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }

        // 足し合わせ音圧波形生成（freq1 + freq2の合成波）
        static byte[] GenerateSumSoundPressureWave(int freq1, int freq2, int sampleRate = 1000)
        {
            int gcd = freq2 == 0 ? freq1 : Gcd(freq2, freq1);
            int periodSamples = sampleRate / gcd;

            var buffer = new byte[periodSamples];
            for (int i = 0; i < periodSamples; i++)
            {
                double t = i / (double)sampleRate;

                // 2つの正弦波の足し合わせ
                double vibrationPressure = (Math.Sin(2 * Math.PI * freq1 * t) + Math.Sin(2 * Math.PI * freq2 * t) + 2) / 4;

                // 音圧: √振動圧
                double soundPressure = Math.Sqrt(Math.Abs(vibrationPressure));

                // 音圧を0-255の範囲に変換
                int intLevel = (int)Math.Round(soundPressure * 255.0);
                buffer[i] = (byte)intLevel;
            }

            return buffer;
        }

        // 単一正弦波の音圧波形生成
        static byte[] GenerateSingleSinSoundPressureWave(int freq, int sampleRate = 1000)
        {
            int periodSamples = sampleRate / freq;

            var buffer = new byte[periodSamples];
            for (int i = 0; i < periodSamples; i++)
            {
                double t = i / (double)sampleRate;

                double vibrationPressure = (Math.Sin(2 * Math.PI * freq * t) + 1) / 2;

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
            Thread.Sleep(3000);
            autd.Send((new Silencer(), new Null()));
            Thread.Sleep(2000);

            // --------------------------------
            // 　　　　　　　　 実験
            // --------------------------------

            Console.WriteLine("=== 足し合わせ波形識別実験開始 ===");
            Console.WriteLine("freq1単体 → freq2単体 → freq1+freq2合成波 → 判定");
            Console.WriteLine("足し合わせに感じる: ← キー, 全く別の触感: → キー, 終了: Enter");

            // CSV保存の準備
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string csvFileName = $"sum_experiment_{timestamp}.csv";
            string resultDir = Path.Combine("result", "Runner0713_sum");
            string csvPath = Path.Combine(resultDir, csvFileName);
            
            // 結果保存ディレクトリを作成
            Directory.CreateDirectory(resultDir);
            
            // CSVヘッダーを書き込み
            using (var writer = new StreamWriter(csvPath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("Timestamp,Freq1_Hz,Freq2_Hz,UserResponse,ResponseType,ResponseTime_ms");
            }
            Console.WriteLine($"実験結果は {csvFileName} に保存されます");

            // 実験設定
            int totalTrials = 0;
            int sumPerception = 0;  // 足し合わせとして感じた回数
            int newPerception = 0;  // 新しい触感として感じた回数

            var random = new Random();

            while (true)
            {
                // 周波数ペアを生成（例：2Hz, 7Hz）
                var (freq1, freq2) = RandomUtil.NextFreqPair0713();
                Console.WriteLine($"\n--- 試行 {totalTrials + 1}: {freq1}Hz + {freq2}Hz ---");

                // freq1単体の波形生成・再生
                Console.WriteLine($"1. {freq1}Hz単体を再生中...");
                var wave1 = GenerateSingleSinSoundPressureWave(freq1);
                var modulation1 = new AUTD3Sharp.Modulation.Custom(
                    buffer: wave1,
                    samplingConfig: 1000f * Hz
                );
                autd.Send((modulation1, focus));
                Thread.Sleep(3000);
                autd.Send((new Silencer(), new Null()));
                Thread.Sleep(1000);

                // freq2単体の波形生成・再生
                Console.WriteLine($"2. {freq2}Hz単体を再生中...");
                var wave2 = GenerateSingleSinSoundPressureWave(freq2);
                var modulation2 = new AUTD3Sharp.Modulation.Custom(
                    buffer: wave2,
                    samplingConfig: 1000f * Hz
                );
                autd.Send((modulation2, focus));
                Thread.Sleep(3000);
                autd.Send((new Silencer(), new Null()));
                Thread.Sleep(1000);

                // 足し合わせ波形生成・再生
                Console.WriteLine($"3. {freq1}Hz + {freq2}Hz合成波を再生中...");
                var waveSum = GenerateSumSoundPressureWave(freq1, freq2);
                var modulationSum = new AUTD3Sharp.Modulation.Custom(
                    buffer: waveSum,
                    samplingConfig: 1000f * Hz
                );
                autd.Send((modulationSum, focus));
                Thread.Sleep(4000);
                autd.Send((new Silencer(), new Null()));

                Console.WriteLine($"判定してください:");
                Console.WriteLine($"← キー: {freq1}Hzと{freq2}Hzの足し合わせに感じる");
                Console.WriteLine($"→ キー: 全く別の新しい触感に感じる");
                Console.WriteLine($"Enter: 実験終了");

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
                        Console.WriteLine($"足し合わせとして感じた: {sumPerception}回 ({(totalTrials > 0 ? (double)sumPerception/totalTrials*100 : 0):F1}%)");
                        Console.WriteLine($"新しい触感として感じた: {newPerception}回 ({(totalTrials > 0 ? (double)newPerception/totalTrials*100 : 0):F1}%)");
                        Console.WriteLine($"結果は {csvFileName} に保存されました");
                        autd.Close();
                        return;
                    }
                    if (key == ConsoleKey.LeftArrow)
                    {
                        stopwatch.Stop();
                        long responseTime = stopwatch.ElapsedMilliseconds;
                        
                        totalTrials++;
                        sumPerception++;

                        // CSVに結果を保存
                        using (var writer = new StreamWriter(csvPath, true, System.Text.Encoding.UTF8))
                        {
                            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            writer.WriteLine($"{currentTime},{freq1},{freq2},LeftArrow,SumPerception,{responseTime}");
                        }

                        Console.WriteLine($"回答: 足し合わせとして感じる (応答時間: {responseTime}ms)");
                        Console.WriteLine($"現在の結果: 足し合わせ {sumPerception}/{totalTrials} ({(double)sumPerception/totalTrials*100:F1}%)");
                        break;
                    }
                    if (key == ConsoleKey.RightArrow)
                    {
                        stopwatch.Stop();
                        long responseTime = stopwatch.ElapsedMilliseconds;
                        
                        totalTrials++;
                        newPerception++;

                        // CSVに結果を保存
                        using (var writer = new StreamWriter(csvPath, true, System.Text.Encoding.UTF8))
                        {
                            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            writer.WriteLine($"{currentTime},{freq1},{freq2},RightArrow,NewPerception,{responseTime}");
                        }

                        Console.WriteLine($"回答: 新しい触感として感じる (応答時間: {responseTime}ms)");
                        Console.WriteLine($"現在の結果: 新触感 {newPerception}/{totalTrials} ({(double)newPerception/totalTrials*100:F1}%)");
                        break;
                    }
                    Console.WriteLine("無効なキーです。← または → または Enter を押してください");
                }

                Thread.Sleep(2000); // 次の試行まで間隔
            }
        }
    }
}
