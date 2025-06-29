import numpy as np
import matplotlib.pyplot as plt

fs = 2000
x = np.linspace(0, 1, fs, endpoint=False)

for f1 in {10, 30, 200}:
    y = np.sin(2 * np.pi * f1 * x)
    plt.figure(figsize=(10, 4))
    plt.plot(x, y ** 2, label='modulated wave^2')
    plt.fill_between(x, y ** 2, alpha=0.3)
    plt.xlabel('time(s)')
    plt.ylabel('Amplitude')
    plt.title(f'radiation pressure (cFreq:{f1})')
    plt.legend()
    plt.xlim(0, 1)
    plt.tight_layout()
    plt.savefig(f"example/Other/img0606/rp_c{f1}Hz.png")
    plt.close()

    for f2 in {3, 6, 10}:  #range(10, 200, 20):
        #if f1 != f2:
            envelope = np.sin(2 * np.pi * f1 * x)
            carrier = np.sin(2 * np.pi * f2 * x)
            y = envelope * carrier

            plt.figure(figsize=(10, 4))
            plt.plot(x, y ** 2, label='modulated wave^2')
            plt.plot(x, envelope ** 2, 'r--', label='envelope^2')
            plt.fill_between(x, y ** 2, alpha=0.3)
            plt.xlabel('time(s)')
            plt.ylabel('Amplitude')
            plt.title(f'radiation pressure (cFreq:{f1} eFreq:{f2})')
            plt.legend()
            plt.xlim(0, 1)
            plt.tight_layout()
            plt.savefig(f"example/Other/img0606/rp_c{f1}Hz_e{f2}Hz.png")
            plt.close()
