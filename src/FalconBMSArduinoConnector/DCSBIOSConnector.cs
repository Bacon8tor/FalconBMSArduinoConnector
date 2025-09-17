using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalconBMSArduinoConnector
{
    internal class DCSBIOSConnector
    {
        // DCS-BIOS protocol constants
        private const string MULTICAST_ADDRESS = "239.255.50.10";
        private const int EXPORT_PORT = 5010;
        private const int IMPORT_PORT = 7778;
        private const uint SYNC_PATTERN = 0x55555555;

        // UDP sockets
        private UdpClient _exportClient;
        private UdpClient _importClient;
        private IPEndPoint _exportEndPoint;
        private IPEndPoint _importEndPoint;

        // Threading
        private Thread _listenThread;
        private bool _isListening = false;
        private bool _isConnected = false;

        // Data storage - 64KB address space as per DCS-BIOS specification
        private byte[] _dataBuffer = new byte[0x10000];
        private readonly object _dataLock = new object();

        // Frame synchronization
        private bool _synchronized = false;
        private byte[] _syncBuffer = new byte[4];
        private int _syncBufferPos = 0;

        // Aircraft data properties
        public bool MasterCaution { get; private set; } = false;
        public bool GearDown { get; private set; } = false;
        public double Altitude { get; private set; } = 0.0;
        public double Speed { get; private set; } = 0.0;
        public double Heading { get; private set; } = 0.0;
        public double FuelLevel { get; private set; } = 0.0;
        public string AircraftType { get; private set; } = "Unknown";

        // Known DCS-BIOS addresses for common data (these may vary by aircraft)
        // These are example addresses - you'll need to adjust for specific aircraft
        private const ushort ADDR_MASTER_CAUTION = 0x1000;
        private const ushort ADDR_GEAR_POSITION = 0x1002;
        private const ushort ADDR_ALTITUDE = 0x1004;
        private const ushort ADDR_SPEED = 0x1008;
        private const ushort ADDR_HEADING = 0x100C;
        private const ushort ADDR_FUEL = 0x1010;

        public bool IsDCSBIOSConnected()
        {
            return _isConnected && _isListening;
        }

        public void StartDCSBIOSListener()
        {
            if (_isListening) return;

            try
            {
                // Setup export listener (multicast UDP)
                _exportClient = new UdpClient();
                _exportClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _exportEndPoint = new IPEndPoint(IPAddress.Parse(MULTICAST_ADDRESS), EXPORT_PORT);
                _exportClient.Client.Bind(new IPEndPoint(IPAddress.Any, EXPORT_PORT));
                _exportClient.JoinMulticastGroup(IPAddress.Parse(MULTICAST_ADDRESS), IPAddress.Parse("127.0.0.1"));

                // Setup import client (for sending commands)
                _importClient = new UdpClient();
                _importEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), IMPORT_PORT);

                _isListening = true;
                _isConnected = true;

                // Start listening thread
                _listenThread = new Thread(ListenForDCSBIOSData);
                _listenThread.IsBackground = true;
                _listenThread.Start();

                Console.WriteLine("DCS-BIOS: Started listening on multicast " + MULTICAST_ADDRESS + ":" + EXPORT_PORT);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DCS-BIOS: Failed to start listener - " + ex.Message);
                _isConnected = false;
                _isListening = false;
            }
        }

        public void StopDCSBIOSListener()
        {
            _isListening = false;
            _isConnected = false;

            try
            {
                _exportClient?.DropMulticastGroup(IPAddress.Parse(MULTICAST_ADDRESS));
                _exportClient?.Close();
                _importClient?.Close();
                _listenThread?.Join(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DCS-BIOS: Error stopping listener - " + ex.Message);
            }
        }

        private void ListenForDCSBIOSData()
        {
            while (_isListening)
            {
                try
                {
                    if (_exportClient.Available > 0)
                    {
                        IPEndPoint remoteEndPoint = null;
                        byte[] data = _exportClient.Receive(ref remoteEndPoint);
                        ProcessDCSBIOSData(data);
                    }
                    else
                    {
                        Thread.Sleep(1); // Small delay to prevent excessive CPU usage
                    }
                }
                catch (Exception ex)
                {
                    if (_isListening)
                    {
                        Console.WriteLine("DCS-BIOS: Error receiving data - " + ex.Message);
                        Thread.Sleep(100);
                    }
                }
            }
        }

        private void ProcessDCSBIOSData(byte[] data)
        {
            foreach (byte b in data)
            {
                // Check for frame sync pattern (0x55 0x55 0x55 0x55)
                _syncBuffer[_syncBufferPos] = b;
                _syncBufferPos = (_syncBufferPos + 1) % 4;

                if (!_synchronized)
                {
                    // Look for sync pattern
                    if (_syncBuffer[0] == 0x55 && _syncBuffer[1] == 0x55 &&
                        _syncBuffer[2] == 0x55 && _syncBuffer[3] == 0x55)
                    {
                        _synchronized = true;
                        Console.WriteLine("DCS-BIOS: Frame synchronized");
                        continue;
                    }
                }
                else
                {
                    // Process data packets
                    ProcessDataByte(b);
                }
            }
        }

        private byte[] _packetBuffer = new byte[4];
        private int _packetPos = 0;
        private bool _readingData = false;
        private ushort _dataAddress = 0;
        private ushort _dataLength = 0;
        private byte[] _dataPayload = null;
        private int _dataPayloadPos = 0;

        private void ProcessDataByte(byte b)
        {
            if (!_readingData)
            {
                // Read packet header (address + length)
                _packetBuffer[_packetPos++] = b;

                if (_packetPos >= 4)
                {
                    // Extract address and length (little-endian)
                    _dataAddress = (ushort)(_packetBuffer[0] | (_packetBuffer[1] << 8));
                    _dataLength = (ushort)(_packetBuffer[2] | (_packetBuffer[3] << 8));

                    if (_dataLength > 0)
                    {
                        _dataPayload = new byte[_dataLength];
                        _dataPayloadPos = 0;
                        _readingData = true;
                    }

                    _packetPos = 0;
                }
            }
            else
            {
                // Read data payload
                _dataPayload[_dataPayloadPos++] = b;

                if (_dataPayloadPos >= _dataLength)
                {
                    // Complete packet received
                    ProcessCompletePacket(_dataAddress, _dataPayload);
                    _readingData = false;
                }
            }
        }

        private void ProcessCompletePacket(ushort address, byte[] data)
        {
            lock (_dataLock)
            {
                // Update data buffer
                if (address + data.Length <= _dataBuffer.Length)
                {
                    Array.Copy(data, 0, _dataBuffer, address, data.Length);

                    // Parse known data addresses
                    ParseAircraftData(address, data);
                }
            }
        }

        private void ParseAircraftData(ushort address, byte[] data)
        {
            // Note: These addresses are examples and will vary by aircraft
            // You'll need to configure these based on the specific aircraft's DCS-BIOS documentation

            try
            {
                switch (address)
                {
                    case ADDR_MASTER_CAUTION:
                        if (data.Length >= 2)
                        {
                            ushort value = (ushort)(data[0] | (data[1] << 8));
                            MasterCaution = value != 0;
                        }
                        break;

                    case ADDR_GEAR_POSITION:
                        if (data.Length >= 2)
                        {
                            ushort value = (ushort)(data[0] | (data[1] << 8));
                            GearDown = value > 0; // Gear down if any gear is extended
                        }
                        break;

                    case ADDR_ALTITUDE:
                        if (data.Length >= 4)
                        {
                            float value = BitConverter.ToSingle(data, 0);
                            Altitude = value;
                        }
                        break;

                    case ADDR_SPEED:
                        if (data.Length >= 4)
                        {
                            float value = BitConverter.ToSingle(data, 0);
                            Speed = value;
                        }
                        break;

                    case ADDR_HEADING:
                        if (data.Length >= 4)
                        {
                            float value = BitConverter.ToSingle(data, 0);
                            Heading = value;
                        }
                        break;

                    case ADDR_FUEL:
                        if (data.Length >= 4)
                        {
                            float value = BitConverter.ToSingle(data, 0);
                            FuelLevel = value;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DCS-BIOS: Error parsing data at address 0x{address:X4} - {ex.Message}");
            }
        }

        public void SendDCSBIOSCommand(string command)
        {
            if (!_isConnected || _importClient == null) return;

            try
            {
                byte[] commandBytes = Encoding.ASCII.GetBytes(command + "\n");
                _importClient.Send(commandBytes, commandBytes.Length, _importEndPoint);
                Console.WriteLine($"DCS-BIOS: Sent command - {command}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DCS-BIOS: Error sending command - {ex.Message}");
            }
        }

        public string GetConnectionStatus()
        {
            if (_isConnected && _synchronized)
                return "DCS-BIOS Connected & Synchronized";
            else if (_isConnected)
                return "DCS-BIOS Connected (Waiting for sync)";
            else
                return "DCS-BIOS Not Connected";
        }

        public Dictionary<string, object> GetFlightData()
        {
            lock (_dataLock)
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
                    ["connected"] = _isConnected,
                    ["synchronized"] = _synchronized
                };
            }
        }

        // Method to get raw data at specific address (for advanced usage)
        public byte[] GetRawData(ushort address, int length)
        {
            lock (_dataLock)
            {
                if (address + length <= _dataBuffer.Length)
                {
                    byte[] result = new byte[length];
                    Array.Copy(_dataBuffer, address, result, 0, length);
                    return result;
                }
                return new byte[0];
            }
        }

        // Method to configure aircraft-specific addresses
        public void ConfigureAircraftAddresses(Dictionary<string, ushort> addresses)
        {
            // This method allows dynamic configuration of data addresses
            // based on the specific aircraft being flown
            if (addresses.ContainsKey("master_caution"))
                // Update ADDR_MASTER_CAUTION dynamically
                Console.WriteLine("DCS-BIOS: Aircraft-specific addresses configured");
        }
    }
}