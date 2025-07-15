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
        private IntellivibeData _intellivibeData = new IntellivibeData();
        public FlyStates falconState;
        private bool _isFalconRunning;
        public bool isFalconRunning()
        {
            FlightData fData =  _reader.GetCurrentData();

            
            bool isFalconDetected =  _reader.IsFalconRunning;
            bool isProcessRunning = Process.GetProcessesByName("Falcon BMS").Length > 0;
            bool isBMSRecorderRunning = Process.GetProcessesByName("F4SharedMemoryRecorder").Length > 0; //You can use F4SharedMemoryRecorder to test lights 

            if (isFalconDetected && isProcessRunning || isBMSRecorderRunning)
            {
                //check if in 3d _intellivibeData.In3D.ToString();
                ;

                _isFalconRunning = true;
                return true;
                
            }
                
                    return false;
        }

        public bool IsLightOn(LightBits bit)
        {
            if(!_isFalconRunning) return false; // If Falcon is not running
            FlightData data = _reader.GetCurrentData();
            if (data == null) return false;
            return (data.lightBits & (uint)bit) != 0;
        }

        public bool IsLightOn( LightBits2 bit)
        {
            if (!_isFalconRunning) return false; // If Falcon is not running
            FlightData data = _reader.GetCurrentData();
            if (data == null) return false;
            return (data.lightBits2 & (uint)bit) != 0;
        }

        public bool IsLightOn( LightBits3 bit)
        {
            if (!_isFalconRunning) return false; // If Falcon is not running
            FlightData data = _reader.GetCurrentData();
            if (data == null) return false;
            return (data.lightBits3 & (uint)bit) != 0;
            
        }

        public string GetFalconVersion()
        {
            if (!_isFalconRunning) return "N/A"; // If Falcon is not running
            try
            {

                FlightData data = _reader.GetCurrentData();
                if(data == null) { return ""; }
                return data.BMSVersionMajor + "." + data.BMSVersionMinor + "." + data.BMSBuildNumber;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting Falcon version: " + ex.Message);
                return "N/A";
            }
            
            
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
