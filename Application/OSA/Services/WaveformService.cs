using System;
using System.Collections.Generic;

namespace OSA.Services
{
    public class WaveformService : IWaveformService
    {
        public int MinWavelength = 350;
        public int MaxWavelength = 1750;
        public WaveformService()
        {
            Wave = new Dictionary<int, double>(MaxWavelength - MinWavelength);
            for (int i = MinWavelength; i < MaxWavelength; i++)
            {
                Wave.Add(i, System.Math.Sin(i * 0.05) * 200 + (i * 0.3));
            }
            Peak = FindPeak();
            Marker1 = 400;
            Marker2 = 700;
        }
        public Dictionary<int, double> Wave { get; set; }

        public int Peak { get; private set; }
        public int MinSetWavelength  { get; private set; } = 350;
        public int MaxSetWavelength { get; private set; } = 1000;

        public int Marker1 { get;  set; } = -1;

        public int Marker2 { get;  set; } = -1;

        public void ToggleMarkers()
        {
            if(Marker1 == -1 || Marker2 == -1)
            {
                Marker1 = MinSetWavelength;
                Marker2 = MaxSetWavelength;
            }
            else
            {
                Marker1 = -1;
                Marker2 = -1;
            }
        }

        private int FindPeak()
        {
            double value = 0;
            int p = 0;
            for (int i = MinSetWavelength; i < MaxSetWavelength; i++)
            {
                if (Wave[i] > value)
                {
                    value = Wave[i];
                    p = i;   
                }
            }
            return p;
        }
    }
}
