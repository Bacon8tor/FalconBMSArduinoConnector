using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.Diagnostics;

namespace FalconBMSArduinoConnector
{
    internal class FalconConnector
    {
        private const int ProcessCheckIntervalMs = 1000;

        private readonly Reader _reader = new Reader();
        private readonly Stopwatch _processCheckTimer = new Stopwatch();
        private string _processName = "N/A";
        private bool _isFalconRunning;

        /// <summary>
        /// True when BMS (or a shared-memory test tool) is running. The process list is
        /// expensive to query, so it is refreshed at most once per second.
        /// </summary>
        public bool isFalconRunning()
        {
            if (!_processCheckTimer.IsRunning || _processCheckTimer.ElapsedMilliseconds >= ProcessCheckIntervalMs)
            {
                _processName = DetectProcess();
                _processCheckTimer.Restart();
            }

            bool isFalconDetected = _reader.IsFalconRunning;
            //You can use F4SharedMemoryRecorder to test lights
            _isFalconRunning = (isFalconDetected && _processName == "Falcon BMS") || _processName == "F4SharedMemoryRecorder";
            return _isFalconRunning;
        }

        private static string DetectProcess()
        {
            if (IsProcessRunning("Falcon BMS")) return "Falcon BMS";
            if (IsProcessRunning("F4SharedMemoryRecorder")) return "F4SharedMemoryRecorder";
            if (IsProcessRunning("RTTClient64")) return "RTTClient64";
            return "N/A";
        }

        private static bool IsProcessRunning(string name)
        {
            var processes = Process.GetProcessesByName(name);
            foreach (var p in processes) p.Dispose();
            return processes.Length > 0;
        }

        public bool IsLightOn(LightBits bit) => IsLightOn(GetFlightDataIfRunning(), bit);
        public bool IsLightOn(LightBits2 bit) => IsLightOn(GetFlightDataIfRunning(), bit);
        public bool IsLightOn(LightBits3 bit) => IsLightOn(GetFlightDataIfRunning(), bit);

        // Overloads that take an existing snapshot, so callers can read shared memory once per update
        public static bool IsLightOn(FlightData data, LightBits bit) => data != null && (data.lightBits & (uint)bit) != 0;
        public static bool IsLightOn(FlightData data, LightBits2 bit) => data != null && (data.lightBits2 & (uint)bit) != 0;
        public static bool IsLightOn(FlightData data, LightBits3 bit) => data != null && (data.lightBits3 & (uint)bit) != 0;

        private FlightData GetFlightDataIfRunning() => _isFalconRunning ? _reader.GetCurrentData() : null;

        public string GetFalconVersion() => GetFalconVersion(GetFlightDataIfRunning());

        public string GetFalconVersion(FlightData data)
        {
            if (!_isFalconRunning) return "N/A";
            if (data == null) return "";
            return data.BMSVersionMajor + "." + data.BMSVersionMinor + "." + data.BMSVersionMicro + "." + data.BMSBuildNumber;
        }

        public string GetFalconProcessName() => _processName;

        public string GetFlyingState() => GetFlyingState(_reader.GetCurrentData());

        public string GetFlyingState(FlightData data)
        {
            try
            {
                if (data == null)
                    return "No data available";

                var iv = data.IntellivibeData;

                if (iv.IsExitGame) return "Exited Game";
                if (iv.IsEndFlight) return "End of Flight";
                if (iv.IsEjecting) return "Ejecting";
                if (iv.IsPaused) return "Paused";
                if (iv.IsFrozen) return "Frozen";
                if (!iv.In3D) return "Not in 3D";
                if (iv.IsOverG) return "Over-G";
                if (iv.IsOnGround) return "On Ground";
                return "Flying";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getting flying state: " + ex.Message);
                return "Error";
            }
        }

        public FlyStates? GetFalconState()
        {
            FlightData data = _reader.GetCurrentData();
            if (data?.pilotsStatus == null || data.pilotsStatus.Length == 0)
                return null;
            return (FlyStates)data.pilotsStatus[0];
        }

        public FlightData GetFlightData()
        {
            return _reader.GetCurrentData();
        }
    }
}
