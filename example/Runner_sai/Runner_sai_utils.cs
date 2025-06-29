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

        // public static (int carrierFreq, int envelopeFreq) NextFreqPair()
        // {
        //     int[] carrierList  = { 10, 40, 200 };
        //     int[] envelopeList = { 2, 5, 10 };

        //     int i1, i2;
        //     do
        //     {
        //         i1 = _rng.Next(carrierList.Length);
        //         i2 = _rng.Next(envelopeList.Length);
        //     }
        //     while (carrierList[i1] == envelopeList[i2]);

        //     return (carrierList[i1], envelopeList[i2]);
        // }

        public static (int A_carrier_Freq, int B_carrier_Freq) NextFreqPair()
        {
            int[] A_carrier_List = {0, 18, 30, 60, 120, 200};
            int[] B_carrier_List = {0, 18, 30, 60, 120, 200};

            int i1, i2;
            do
            {
                i1 = _rng.Next(A_carrier_List.Length);
                i2 = _rng.Next(B_carrier_List.Length);
            }
            while (i1 == i2);

            return (A_carrier_List[i1], B_carrier_List[i2]);
        }
    }
}