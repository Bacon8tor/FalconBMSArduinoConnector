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

        public FlyStates falconState;

        public bool isFalconRunning()
        {
            FlightData fData =  _reader.GetCurrentData();
            
            bool isFalconDetected =  _reader.IsFalconRunning;
            bool isProcessRunning = Process.GetProcessesByName("Falcon BMS").Length > 0;
            bool isBMSRecorderRunning = Process.GetProcessesByName("F4SharedMemoryRecorder").Length > 0; //You can use F4SharedMemoryRecorder to test lights 

            if (isFalconDetected && isProcessRunning || isBMSRecorderRunning)
            {
               //falconState = (FlyStates)fData.pilotsStatus[0];

                return true;
                
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

        public string GetFalconVersion()
        {
            FlightData data = _reader.GetCurrentData();
            return  data.BMSVersionMajor + "." + data.BMSVersionMinor + "." + data.BMSVersionMinor;
        }

        public FlyStates GetFalconState()
        {
            FlightData data = _reader.GetCurrentData();
            FlyStates falconState = (FlyStates)data.pilotsStatus[0];
            return falconState;
        }

        public FlightData GetFlightData()
        {
            FlightData data = _reader.GetCurrentData();
            
            return data;
        }

       
    }
}
