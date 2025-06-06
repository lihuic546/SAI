import numpy as np
import matplotlib.pyplot as plt

fs = 2000
x = np.linspace(0, 1, fs, endpoint=False)

for f1 in {6}:
    for f2 in range(10, 210, 10):
        if f1 != f2:
            envelope = np.sin(2 * np.pi * f1 * x)
            carrier = np.sin(2 * np.pi * f2 * x)
            y = envelope * carrier

            plt.figure(figsize=(10, 4))
            plt.plot(x, y ** 2, label='modulated wave^2')
            plt.plot(x, envelope ** 2, 'r--', label='envelope^2')
            plt.xlabel('time(s)')
            plt.ylabel('Amplitude')
            plt.title(f'radiation pressure (eFreq:{f1} cFreq:{f2})')
            plt.legend()
            plt.xlim(0, 1)
            plt.tight_layout()
            plt.savefig(f"example/Other/img0601/rp_e{f1}Hz_c{f2}Hz.png")
            plt.close()
