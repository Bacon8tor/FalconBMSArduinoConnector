using F4SharedMem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace FalconBMSArduinoConnector
{
    /// <summary>
    /// Serial command IDs shared with the Arduino library (FBAC_Commands.h). Keep both lists in sync.
    /// Request:  Arduino -> PC  [cmd]
    /// Response: PC -> Arduino  [0xAA][type][len][data...][sum8(type + len + data)]   (type == cmd)
    /// </summary>
    internal enum FbacCommand : byte
    {
        Unknown = 0x00,
        LightBits = 0x01,
        LightBits2 = 0x02,
        LightBits3 = 0x03,
        BlinkBits = 0x04,
        DED = 0x05,
        FuelFlow = 0x06,
        InstrLight = 0x07,
        PFL = 0x08,
        ChaffCount = 0x09,
        Ping = 0x0F,
        FlareCount = 0x10,
        FloodConsole = 0x11,
        Rpm = 0x12,
        EcmBits = 0x13,
        OilPress = 0x14,
        OilPress2 = 0x15,
        NozzlePos = 0x16,
        NozzlePos2 = 0x17,
        Ftit = 0x18,
        Ftit2 = 0x19,
        CabinAlt = 0x20,
        Kias = 0x21,
        InternalFuel = 0x22,
        ExternalFuel = 0x23,
        EpuFuel = 0x24,
        HydPressA = 0x25,
        HydPressB = 0x26,
        CmdsMode = 0x27,
        UhfPreset = 0x28,
        UhfFreq = 0x29,
        SpeedBrake = 0x30,
        IffMode1Digit1 = 0x31,
        IffMode1Digit2 = 0x32,
        IffMode3Digit1 = 0x33,
        IffMode3Digit2 = 0x34,
        FuelFwd = 0x35,
        FuelAft = 0x36,
        FuelTotal = 0x37,
        DesiredCourse = 0x38,
        CourseDeviation = 0x39,
        DistanceToBeacon = 0x40,
        BearingToBeacon = 0x41,
        SimStatus = 0x42,
        // protocol v2
        ProtocolVersion = 0x43,
        AllLights = 0x44,
    }

    internal class ArduinoConnector
    {
        public const byte ProtocolVersion = 2;

        private const byte PacketStart = 0xAA;
        private const byte HandshakeRequest = 0xA5;
        private const byte HandshakeAck = 0x5A;
        private const int EcmCount = 5;           // BMS MAX_ECM_PROGRAMS
        private const int DisplayLineLength = 24;
        private const int HandshakeTimeoutMs = 4000;
        private const int HandshakeRetryMs = 250;

        // ISO-8859-1 maps chars 0-255 to the same byte, which is what the Arduino DED font expects
        private static readonly Encoding Latin1 = Encoding.GetEncoding(28591);

        // Payload sizes used to send a well-formed zero reply when BMS data is not available
        private static readonly Dictionary<FbacCommand, int> PayloadSizes = new Dictionary<FbacCommand, int>
        {
            { FbacCommand.LightBits, 4 }, { FbacCommand.LightBits2, 4 }, { FbacCommand.LightBits3, 4 },
            { FbacCommand.BlinkBits, 4 }, { FbacCommand.DED, 5 * DisplayLineLength }, { FbacCommand.FuelFlow, 4 },
            { FbacCommand.InstrLight, 1 }, { FbacCommand.PFL, 5 * DisplayLineLength }, { FbacCommand.ChaffCount, 4 },
            { FbacCommand.FlareCount, 4 }, { FbacCommand.FloodConsole, 1 }, { FbacCommand.Rpm, 4 },
            { FbacCommand.EcmBits, EcmCount * 4 }, { FbacCommand.OilPress, 4 }, { FbacCommand.OilPress2, 4 },
            { FbacCommand.NozzlePos, 4 }, { FbacCommand.NozzlePos2, 4 }, { FbacCommand.Ftit, 4 },
            { FbacCommand.Ftit2, 4 }, { FbacCommand.CabinAlt, 4 }, { FbacCommand.Kias, 4 },
            { FbacCommand.InternalFuel, 4 }, { FbacCommand.ExternalFuel, 4 }, { FbacCommand.EpuFuel, 4 },
            { FbacCommand.HydPressA, 4 }, { FbacCommand.HydPressB, 4 }, { FbacCommand.CmdsMode, 4 },
            { FbacCommand.UhfPreset, 4 }, { FbacCommand.UhfFreq, 4 }, { FbacCommand.SpeedBrake, 4 },
            { FbacCommand.IffMode1Digit1, 1 }, { FbacCommand.IffMode1Digit2, 1 },
            { FbacCommand.IffMode3Digit1, 1 }, { FbacCommand.IffMode3Digit2, 1 },
            { FbacCommand.FuelFwd, 4 }, { FbacCommand.FuelAft, 4 }, { FbacCommand.FuelTotal, 4 },
            { FbacCommand.DesiredCourse, 4 }, { FbacCommand.CourseDeviation, 4 },
            { FbacCommand.DistanceToBeacon, 4 }, { FbacCommand.BearingToBeacon, 4 },
            { FbacCommand.SimStatus, 4 }, { FbacCommand.AllLights, 16 + EcmCount * 4 },
        };

        // Get all available COM ports
        public List<string> GetAvailablePorts()
        {
            return new List<string>(SerialPort.GetPortNames());
        }

        // Optional: expose selected port
        public string SelectedPort { get; set; }

        /// <summary>Raised with "Disconnected" from the reader thread when the link drops.</summary>
        public event EventHandler<string> OnDataReceived;

        private readonly Reader bmsReader = new Reader();
        private SerialPort _serialPort;
        private volatile bool _continue;
        private Thread readThread;
        private volatile bool _isConnected;

        public bool IsConnected => _isConnected;

        public bool ConnectSerial(string name, bool DTR, int baudRate = 115200)
        {
            // Legacy behaviour: calling connect while connected toggles to disconnected
            if (_isConnected)
            {
                Disconnect();
                return false;
            }

            var port = new SerialPort(name, baudRate, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                DtrEnable = DTR,
                ReadTimeout = 200,
                WriteTimeout = 1000,
            };

            try
            {
                port.Open();
                Debug.WriteLine("Opened port " + name);

                if (!PerformHandshake(port))
                {
                    Debug.WriteLine("Handshake failed or timed out on " + name);
                    port.Close();
                    port.Dispose();
                    return false;
                }

                _serialPort = port;
                _continue = true;
                _isConnected = true;
                readThread = new Thread(ReadLoop) { IsBackground = true, Name = "FBAC serial " + name };
                readThread.Start();
                Debug.WriteLine("Handshake successful on " + name);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Connect error on {name}: {ex.Message}");
                try { port.Close(); port.Dispose(); } catch { }
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Sends 0xA5 every 250 ms until the Arduino answers 0x5A. Retrying matters for boards
        /// that reset when the port opens (Uno/Nano): the bootloader swallows the first bytes.
        /// </summary>
        private static bool PerformHandshake(SerialPort port)
        {
            port.DiscardInBuffer();
            var sw = Stopwatch.StartNew();
            long nextSend = 0;

            while (sw.ElapsedMilliseconds < HandshakeTimeoutMs)
            {
                if (sw.ElapsedMilliseconds >= nextSend)
                {
                    port.Write(new[] { HandshakeRequest }, 0, 1);
                    nextSend = sw.ElapsedMilliseconds + HandshakeRetryMs;
                }

                while (port.BytesToRead > 0)
                {
                    if (port.ReadByte() == HandshakeAck)
                        return true;
                }
                Thread.Sleep(10);
            }
            return false;
        }

        private void ReadLoop()
        {
            var port = _serialPort;
            while (_continue)
            {
                int incoming;
                try
                {
                    incoming = port.ReadByte(); // blocks up to ReadTimeout
                }
                catch (TimeoutException)
                {
                    continue;
                }
                catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is UnauthorizedAccessException)
                {
                    if (_continue)
                    {
                        Debug.WriteLine("Serial read error: " + ex.Message);
                        HandleDisconnection();
                    }
                    return;
                }

                if (incoming < 0)
                    continue;

                try
                {
                    HandleCommand(port, (byte)incoming);
                }
                catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is UnauthorizedAccessException || ex is TimeoutException)
                {
                    if (_continue)
                    {
                        Debug.WriteLine("Serial write error: " + ex.Message);
                        HandleDisconnection();
                    }
                    return;
                }
                catch (Exception ex)
                {
                    // Bad data from shared memory must not take the link down
                    Debug.WriteLine($"Error handling command 0x{incoming:X2}: {ex.Message}");
                }
            }
        }

        private void HandleCommand(SerialPort port, byte raw)
        {
            // A connected Arduino running an older library sends 0x5A when it thinks the link
            // timed out, and waits for 0xA5 to re-handshake. Answer with the bare byte.
            if (raw == HandshakeAck)
            {
                port.Write(new[] { HandshakeRequest }, 0, 1);
                return;
            }

            var command = (FbacCommand)raw;

            switch (command)
            {
                case FbacCommand.Ping:
                    SendResponse(port, raw, new byte[] { 0xAB });
                    return;
                case FbacCommand.ProtocolVersion:
                    SendResponse(port, raw, new byte[] { ProtocolVersion, 0x00 });
                    return;
            }

            if (!PayloadSizes.TryGetValue(command, out int size))
            {
                // Includes 0x99 (old library's "checksum failed")
                Debug.WriteLine($"Unknown command: 0x{raw:X2}");
                SendResponse(port, (byte)FbacCommand.Unknown, new byte[] { 0x00 });
                return;
            }

            FlightData fData = bmsReader.GetCurrentData();
            byte[] payload = fData == null ? new byte[size] : BuildPayload(command, fData);
            SendResponse(port, raw, payload);
        }

        private static byte[] BuildPayload(FbacCommand command, FlightData f)
        {
            switch (command)
            {
                case FbacCommand.LightBits: return BitConverter.GetBytes(f.lightBits);
                case FbacCommand.LightBits2: return BitConverter.GetBytes(f.lightBits2);
                case FbacCommand.LightBits3: return BitConverter.GetBytes(f.lightBits3);
                case FbacCommand.BlinkBits: return BitConverter.GetBytes(f.blinkBits);
                case FbacCommand.DED: return BuildDisplay(f.DEDLines, f.Invert);
                case FbacCommand.PFL: return BuildDisplay(f.PFLLines, f.PFLInvert);
                case FbacCommand.FuelFlow: return BitConverter.GetBytes(f.fuelFlow);
                case FbacCommand.InstrLight: return new[] { f.instrLight };
                case FbacCommand.ChaffCount: return BitConverter.GetBytes(f.ChaffCount);
                case FbacCommand.FlareCount: return BitConverter.GetBytes(f.FlareCount);
                case FbacCommand.FloodConsole: return new[] { (byte)f.floodConsole };
                case FbacCommand.Rpm: return BitConverter.GetBytes(f.rpm);
                case FbacCommand.EcmBits: return EcmBytes(f.ecmBits);
                case FbacCommand.OilPress: return BitConverter.GetBytes(f.oilPressure);
                case FbacCommand.OilPress2: return BitConverter.GetBytes(f.oilPressure2);
                case FbacCommand.NozzlePos: return BitConverter.GetBytes(f.nozzlePos);
                case FbacCommand.NozzlePos2: return BitConverter.GetBytes(f.nozzlePos2);
                case FbacCommand.Ftit: return BitConverter.GetBytes(f.ftit);
                case FbacCommand.Ftit2: return BitConverter.GetBytes(f.ftit2);
                case FbacCommand.CabinAlt: return BitConverter.GetBytes(f.cabinAlt);
                case FbacCommand.Kias: return BitConverter.GetBytes(f.kias);
                case FbacCommand.InternalFuel: return BitConverter.GetBytes(f.internalFuel);
                case FbacCommand.ExternalFuel: return BitConverter.GetBytes(f.externalFuel);
                case FbacCommand.EpuFuel: return BitConverter.GetBytes(f.epuFuel);
                case FbacCommand.HydPressA: return BitConverter.GetBytes(f.hydPressureA);
                case FbacCommand.HydPressB: return BitConverter.GetBytes(f.hydPressureB);
                case FbacCommand.CmdsMode: return BitConverter.GetBytes(f.cmdsMode);
                case FbacCommand.UhfPreset: return BitConverter.GetBytes(f.BupUhfPreset);
                case FbacCommand.UhfFreq: return BitConverter.GetBytes(f.BupUhfFreq);
                case FbacCommand.SpeedBrake: return BitConverter.GetBytes(f.speedBrake);
                case FbacCommand.IffMode1Digit1: return new[] { f.iffBackupMode1Digit1 };
                case FbacCommand.IffMode1Digit2: return new[] { f.iffBackupMode1Digit2 };
                case FbacCommand.IffMode3Digit1: return new[] { f.iffBackupMode3ADigit1 };
                case FbacCommand.IffMode3Digit2: return new[] { f.iffBackupMode3ADigit2 };
                case FbacCommand.FuelFwd: return BitConverter.GetBytes(f.fwd);
                case FbacCommand.FuelAft: return BitConverter.GetBytes(f.aft);
                case FbacCommand.FuelTotal: return BitConverter.GetBytes(f.total);
                case FbacCommand.DesiredCourse: return BitConverter.GetBytes(f.desiredCourse);
                case FbacCommand.CourseDeviation: return BitConverter.GetBytes(f.courseDeviation);
                case FbacCommand.DistanceToBeacon: return BitConverter.GetBytes(f.distanceToBeacon);
                case FbacCommand.BearingToBeacon: return BitConverter.GetBytes(f.bearingToBeacon);
                case FbacCommand.SimStatus:
                    // TODO: add more bits, just In3D for now
                    uint statusBits = f.IntellivibeData.In3D ? 1u : 0u;
                    return BitConverter.GetBytes(statusBits);
                case FbacCommand.AllLights:
                    var all = new byte[16 + EcmCount * 4];
                    Array.Copy(BitConverter.GetBytes(f.lightBits), 0, all, 0, 4);
                    Array.Copy(BitConverter.GetBytes(f.lightBits2), 0, all, 4, 4);
                    Array.Copy(BitConverter.GetBytes(f.lightBits3), 0, all, 8, 4);
                    Array.Copy(BitConverter.GetBytes(f.blinkBits), 0, all, 12, 4);
                    Array.Copy(EcmBytes(f.ecmBits), 0, all, 16, EcmCount * 4);
                    return all;
                default:
                    return new byte[PayloadSizes[command]];
            }
        }

        private static byte[] EcmBytes(uint[] ecmBits)
        {
            var bytes = new byte[EcmCount * 4];
            if (ecmBits == null) return bytes;
            for (int i = 0; i < Math.Min(EcmCount, ecmBits.Length); i++)
                Array.Copy(BitConverter.GetBytes(ecmBits[i]), 0, bytes, i * 4, 4);
            return bytes;
        }

        private static byte[] BuildDisplay(string[] lines, string[] invert)
        {
            var merged = new byte[5 * DisplayLineLength];
            for (int i = 0; i < 5; i++)
            {
                string line = lines != null && i < lines.Length ? lines[i] : null;
                string inv = invert != null && i < invert.Length ? invert[i] : null;
                Array.Copy(NormalizeLine(line, inv), 0, merged, i * DisplayLineLength, DisplayLineLength);
            }
            return merged;
        }

        // Adapted from the DEDuino project: https://github.com/uriba107/deduino_connector
        // Merges a DED/PFL line and its inversion mask into the Arduino font encoding
        // (inverted letters are lowercase, other inverted chars are +128).
        private static byte[] NormalizeLine(string disp, string inv)
        {
            var normLine = new char[DisplayLineLength];
            for (int j = 0; j < DisplayLineLength; j++)
                normLine[j] = ' ';

            if (disp == null)
                return Latin1.GetBytes(normLine);

            int count = Math.Min(DisplayLineLength, disp.Length);
            for (int j = 0; j < count; j++)
            {
                char c = disp[j];
                bool inverted = inv != null && j < inv.Length && inv[j] == 2;

                if (inverted)
                {
                    if (char.IsLetter(c)) normLine[j] = char.ToLower(c);  // inverted glyphs live at lowercase
                    else if (c == 1) normLine[j] = (char)192;            // selection arrows
                    else if (c == 2) normLine[j] = (char)170;            // DED "*"
                    else if (c == 3) normLine[j] = (char)223;            // DED "_"
                    else if (c == '~') normLine[j] = (char)252;          // arrow down (PFD)
                    else if (c == '^') normLine[j] = (char)222;          // degree symbol
                    else normLine[j] = (char)Math.Min(255, c + 128);
                }
                else
                {
                    if (c == 1) normLine[j] = '@';        // selector double arrow
                    else if (c == 2) normLine[j] = '*';
                    else if (c == 3) normLine[j] = '_';
                    else if (c == '~') normLine[j] = '|'; // arrow down (PFD)
                    else normLine[j] = c > 255 ? '?' : c;
                }
            }
            return Latin1.GetBytes(normLine);
        }

        private static void SendResponse(SerialPort port, byte type, byte[] data)
        {
            if (data.Length > 255)
                throw new ArgumentException("Payload too large", nameof(data));

            var packet = new byte[data.Length + 4];
            packet[0] = PacketStart;
            packet[1] = type;
            packet[2] = (byte)data.Length;
            byte checksum = (byte)(type + data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                packet[3 + i] = data[i];
                checksum += data[i];
            }
            packet[packet.Length - 1] = checksum;

            port.Write(packet, 0, packet.Length);
        }

        private void HandleDisconnection()
        {
            _continue = false;
            _isConnected = false;
            try { _serialPort?.Close(); } catch { }
            OnDataReceived?.Invoke(this, "Disconnected"); // UI listens and resets the button
        }

        public void Disconnect()
        {
            _continue = false;
            var thread = readThread;
            if (thread != null && thread != Thread.CurrentThread)
                thread.Join(1000); // ReadTimeout is 200 ms, so the loop exits quickly

            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    Debug.WriteLine("Disconnected from port " + _serialPort.PortName);
                }
                _serialPort?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during disconnect: " + ex.Message);
            }
            _serialPort = null;
            readThread = null;
            _isConnected = false;
        }
    }
}
