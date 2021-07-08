using System;
using System.Collections.Generic;

namespace OSA.Services
{
    public interface IWaveformService
    {
        Dictionary<int,double> Wave { get; }
        int Peak { get; }
        int MinSetWavelength { get; }
        int MaxSetWavelength { get; }
        int Marker1 { get; set; }
        int Marker2 { get; set; }
        void ToggleMarkers();
    }
}
