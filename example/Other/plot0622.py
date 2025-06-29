import numpy as np
import matplotlib.pyplot as plt
import os

# 出力ディレクトリの作成
output_dir = "example/Other/img0622"
os.makedirs(output_dir, exist_ok=True)

# パラメータ設定
fs = 2000  # サンプリング周波数
duration = 1  # 時間長（秒）
x = np.linspace(0, duration, fs, endpoint=False)

# envelope周波数を固定（正弦波）
envelope_freq = 6  # Hz

# carrier周波数のリスト（envelope周波数の整数倍で綺麗な周期パターン）
carrier_freqs = [0, 6, 10, 14,  18, 30, 60, 120, 200]  # envelope * (2n-1) Hz だと綺麗 / 十分大きい時(>50)はそんなに気にしなくてもいいかも

print(f"Vibration pressure -> Sound pressure conversion plot generation started")
print(f"Envelope frequency: {envelope_freq}Hz (sine wave)")
print(f"Carrier frequencies: {carrier_freqs}")

for carrier_freq in carrier_freqs:
    # envelope波形（0~1の範囲）
    envelope = (1 + np.sin(2 * np.pi * envelope_freq * x - np.pi / 2)) / 2
    
    if carrier_freq == 0:
        # carrierなしの場合
        modulated_wave = envelope
        title_suffix = "no carrier"
        filename_suffix = f"e{envelope_freq}Hz_c0Hz"
    else:
        # carrier波形（0~1の範囲）
        carrier = (1 + np.sin(2 * np.pi * carrier_freq * x - np.pi / 2)) / 2
        # 変調波形（envelope × carrier）
        modulated_wave = envelope * carrier
        title_suffix = f"carrier {carrier_freq}Hz"
        filename_suffix = f"e{envelope_freq}Hz_c{carrier_freq}Hz"
    
    # 振動圧（変調波形そのもの）
    vibration_pressure = modulated_wave
    
    # 音圧（振動圧の平方根）
    sound_pressure = np.sqrt(np.abs(vibration_pressure))
    
    # envelope圧力（比較用）
    envelope_pressure = envelope
    envelope_sound_pressure = np.sqrt(np.abs(envelope_pressure))
    
    # プロット作成（2段構成）
    fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(12, 8))
    
    # 1. 振動圧
    ax1.plot(x, vibration_pressure, 'g-', label='Vibration pressure (modulated wave)', linewidth=1)
    ax1.plot(x, envelope_pressure, 'r--', label='Envelope pressure (envelope)', linewidth=2)
    ax1.fill_between(x, vibration_pressure, alpha=0.3, color='green')
    ax1.set_xlabel('Time (s)')
    ax1.set_ylabel('Vibration pressure')
    ax1.set_title(f'Vibration pressure: envelope {envelope_freq}Hz, {title_suffix}')
    ax1.legend()
    ax1.grid(True, alpha=0.3)
    ax1.set_xlim(0, min(1, duration))
    
    # 2. 音圧（振動圧の平方根）
    ax2.plot(x, sound_pressure, 'm-', label='Sound pressure (√vibration pressure)', linewidth=1)
    ax2.plot(x, envelope_sound_pressure, 'r--', label='Envelope sound pressure (√envelope pressure)', linewidth=2)
    ax2.fill_between(x, sound_pressure, alpha=0.3, color='magenta')
    ax2.set_xlabel('Time (s)')
    ax2.set_ylabel('Sound pressure')
    ax2.set_title(f'Sound pressure: envelope {envelope_freq}Hz, {title_suffix}')
    ax2.legend()
    ax2.grid(True, alpha=0.3)
    ax2.set_xlim(0, min(1, duration))
    
    plt.tight_layout()
    
    # ファイル保存
    output_path = f"{output_dir}/vibration_to_sound_{filename_suffix}.png"
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    plt.close()
    
    print(f"保存完了: {output_path}")

# 比較用：全carrier周波数の音圧を重ねて表示
plt.figure(figsize=(14, 8))

colors = ['red', 'blue', 'green', 'orange', 'purple', 'brown', 'pink']
for i, carrier_freq in enumerate(carrier_freqs):
    envelope = (1 + np.sin(2 * np.pi * envelope_freq * x)) / 2
    
    if carrier_freq == 0:
        modulated_wave = envelope
        label = f"no carrier (envelope {envelope_freq}Hz)"
    else:
        carrier = (1 + np.sin(2 * np.pi * carrier_freq * x)) / 2
        modulated_wave = envelope * carrier
        label = f"carrier {carrier_freq}Hz"
    
    # 音圧計算
    vibration_pressure = modulated_wave
    sound_pressure = np.sqrt(np.abs(vibration_pressure))
    
    plt.plot(x, sound_pressure, color=colors[i % len(colors)], 
             label=label, linewidth=2, alpha=0.8)

plt.xlabel('Time (s)')
plt.ylabel('Sound pressure')
plt.title(f'Sound pressure comparison: envelope {envelope_freq}Hz, carrier frequency variation')
plt.legend(bbox_to_anchor=(1.05, 1), loc='upper left')
plt.grid(True, alpha=0.3)
plt.xlim(0, min(0.5, duration))  # 最初の0.5秒を表示
plt.tight_layout()

comparison_path = f"{output_dir}/sound_pressure_comparison_e{envelope_freq}Hz.png"
plt.savefig(comparison_path, dpi=300, bbox_inches='tight')
plt.close()

print(f"比較プロット保存完了: {comparison_path}")
print(f"\n全プロット生成完了！")
print(f"出力ディレクトリ: {output_dir}")
