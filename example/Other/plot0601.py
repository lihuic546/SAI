import numpy as np
import matplotlib.pyplot as plt

fs = 2000
x = np.linspace(0, 1, fs, endpoint=False)

for f1 in {2, 5, 10}:
    for f2 in {10, 40, 200}:
        # if f1 != f2:
            envelope = np.sin(2 * np.pi * f1 * x)
            carrier = np.sin(2 * np.pi * f2 * x)
            y = envelope * carrier

            plt.figure(figsize=(10, 4))
            plt.plot(x, y, label='modulated wave')
            plt.plot(x, envelope, 'r--', label='envelope')
            plt.xlabel('time(s)')
            plt.ylabel('Amplitude')
            plt.title('modulated wave')
            plt.legend()
            plt.xlim(0, 1)
            plt.tight_layout()
            plt.savefig(f"example/Other/img0601/modulated_wave_e{f1}Hz_c{f2}Hz.png")
            plt.close()
