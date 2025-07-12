using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FalconBMSArduinoConnector
{
    internal class FalconConnector
    {
        private Reader _reader = new Reader();

        public bool isFalconRunning()
        {
            FlightData fData =  _reader.GetCurrentData();
            
            bool isFalconDetected =  _reader.IsFalconRunning;
            bool isProcessRunning = Process.GetProcessesByName("Falcon BMS").Length > 0;

            if (isFalconDetected && isProcessRunning)
            {
                FlyStates falconState = new FlyStates();
                if (falconState == FlyStates.FLYING)
                {
                    return true;
                }
                else if (falconState == FlyStates.IN_UI)
                {
                    return true;
                }
                else if (falconState == FlyStates.DEAD)
                {
                    return true;
                }
            }

                    return false;
        }
        public bool IsLightOn(LightBits bit)
        {
            FlightData data = _reader.GetCurrentData();
            return (data.lightBits & (uint)bit) != 0;
        }

        public bool IsLightOn( LightBits2 bit)
        {
            FlightData data = _reader.GetCurrentData();
            return (data.lightBits2 & (uint)bit) != 0;
        }

        public bool IsLightOn( LightBits3 bit)
        {
            FlightData data = _reader.GetCurrentData();
            return (data.lightBits3 & (uint)bit) != 0;
        }



    }
}
