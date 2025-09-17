using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;

namespace FalconBMSArduinoConnector
{
    internal class DCSConnector
    {
        private TcpListener _listener;
        private TcpClient _client;
        private StreamReader _reader;
        private bool _isDCSConnected = false;
        private bool _isListening = false;
        private Thread _listenerThread;

        // DCS flight data properties - starting with basic test variables
        public double Altitude { get; private set; } = 0.0;
        public double Speed { get; private set; } = 0.0;
        public double Heading { get; private set; } = 0.0;
        public double FuelLevel { get; private set; } = 0.0;
        public bool MasterCaution { get; private set; } = false;
        public bool GearDown { get; private set; } = false;
        public string AircraftType { get; private set; } = "Unknown";

        public bool IsDCSConnected()
        {
            return _isDCSConnected && _client != null && _client.Connected;
        }

        public void StartDCSListener()
        {
            if (_isListening) return;

            _isListening = true;
            _listenerThread = new Thread(ListenForDCS);
            _listenerThread.Start();
        }

        public void StopDCSListener()
        {
            _isListening = false;
            _isDCSConnected = false;

            try
            {
                _reader?.Close();
                _client?.Close();
                _listener?.Stop();
                _listenerThread?.Join(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error stopping DCS listener: " + ex.Message);
            }
        }

        private void ListenForDCS()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 31090);
                _listener.Start();
                Console.WriteLine("DCS TCP Server started on port 31090");

                while (_isListening)
                {
                    try
                    {
                        Console.WriteLine("Waiting for DCS connection...");
                        _client = _listener.AcceptTcpClient();
                        _isDCSConnected = true;
                        Console.WriteLine("DCS connected successfully");

                        _reader = new StreamReader(_client.GetStream());
                        ProcessDCSData();
                    }
                    catch (Exception ex)
                    {
                        if (_isListening)
                        {
                            Console.WriteLine("DCS connection error: " + ex.Message);
                            _isDCSConnected = false;
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DCS listener error: " + ex.Message);
            }
        }

        private void ProcessDCSData()
        {
            try
            {
                string dataLine;
                while (_isListening && _client.Connected && (dataLine = _reader.ReadLine()) != null)
                {
                    if (dataLine == "exit") break;

                    ParseDCSData(dataLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing DCS data: " + ex.Message);
            }
            finally
            {
                _isDCSConnected = false;
                _reader?.Close();
                _client?.Close();
            }
        }

        private void ParseDCSData(string dataLine)
        {
            try
            {
                // Handle simple key=value format
                var parts = dataLine.Split('=');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().ToLower();
                    var value = parts[1].Trim();

                    switch (key)
                    {
                        case "altitude":
                        case "alt":
                            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double alt))
                                Altitude = alt;
                            break;
                        case "speed":
                        case "ias":
                        case "groundspeed":
                            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double spd))
                                Speed = spd;
                            break;
                        case "heading":
                        case "hdg":
                            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double hdg))
                                Heading = hdg;
                            break;
                        case "fuel":
                        case "fuel_level":
                            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double fuel))
                                FuelLevel = fuel;
                            break;
                        case "master_caution":
                        case "caution":
                            if (bool.TryParse(value, out bool caution))
                                MasterCaution = caution;
                            else if (value == "1" || value.ToLower() == "on")
                                MasterCaution = true;
                            else if (value == "0" || value.ToLower() == "off")
                                MasterCaution = false;
                            break;
                        case "gear_down":
                        case "gear":
                        case "landing_gear":
                            if (bool.TryParse(value, out bool gear))
                                GearDown = gear;
                            else if (value == "1" || value.ToLower() == "down")
                                GearDown = true;
                            else if (value == "0" || value.ToLower() == "up")
                                GearDown = false;
                            break;
                        case "aircraft_type":
                        case "aircraft":
                        case "actype":
                            AircraftType = value;
                            break;
                    }
                }
                else if (dataLine.Contains(":"))
                {
                    // Handle colon-separated format (key:value)
                    var colonParts = dataLine.Split(':');
                    if (colonParts.Length == 2)
                    {
                        var key = colonParts[0].Trim().ToLower();
                        var value = colonParts[1].Trim();

                        switch (key)
                        {
                            case "altitude":
                            case "alt":
                                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double alt))
                                    Altitude = alt;
                                break;
                            case "speed":
                            case "ias":
                                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double spd))
                                    Speed = spd;
                                break;
                            case "heading":
                            case "hdg":
                                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double hdg))
                                    Heading = hdg;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing DCS data: " + ex.Message);
            }
        }

        public string GetConnectionStatus()
        {
            if (_isDCSConnected)
                return "Connected to DCS";
            else if (_isListening)
                return "Listening for DCS";
            else
                return "Not connected";
        }

        // Method to get formatted flight data similar to Falcon
        public Dictionary<string, object> GetFlightData()
        {
            return new Dictionary<string, object>
            {
                ["altitude"] = Altitude,
                ["speed"] = Speed,
                ["heading"] = Heading,
                ["fuel"] = FuelLevel,
                ["master_caution"] = MasterCaution,
                ["gear_down"] = GearDown,
                ["aircraft_type"] = AircraftType,
                ["connected"] = _isDCSConnected
            };
        }
    }
}
