using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalconBMSArduinoConnector
{
    internal class ArduinoConnector
    {
        // Get all available COM ports
        public List<string> GetAvailablePorts()
        {
            return new List<string>(SerialPort.GetPortNames());
        }

        // Optional: expose selected port
        public string SelectedPort { get; set; }

        public event EventHandler<string> OnDataReceived;

        private FlightData flightData ;
        private Reader bmsReader = new Reader();
        private DCSConnector dcsConnector;
        private DCSBIOSConnector dcsBiosConnector;
        private FalconConnector falconConnector;

        public void SetSimulatorConnectors(FalconConnector falcon, DCSConnector dcs, DCSBIOSConnector dcsBios)
        {
            falconConnector = falcon;
            dcsConnector = dcs;
            dcsBiosConnector = dcsBios;
        }
        // Optional: connect logic (if needed later)
        private SerialPort _serialPort;
        private volatile bool _continue;
        private Thread readThread = null;
        private bool _isConnected = false;
        
        public bool ConnectSerial(String name,bool DTR)
        {
            // If already connected, disconnect
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _continue = false;  // stop read loop
                    readThread?.Join(500);  // wait for read thread to finish (optional timeout)
                    _serialPort.Close();
                    Console.WriteLine("Disconnected from port " + _serialPort.PortName);
                    _isConnected = false;


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during disconnect: " + ex.Message);
                }
                return false;
            }

            // Create a new SerialPort object
            _serialPort = new SerialPort();

            // Set serial parameters
            _serialPort.PortName = name;
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            if (DTR)
            {
                _serialPort.DtrEnable = true;
            }
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;

            try
            {
                _serialPort.Open();
                Console.WriteLine("Opened port " + _serialPort.PortName);

                //Thread.Sleep(1000); //Possibly needed when using arduin oleonardo not sure 

                // Send handshake byte to Arduino
                _serialPort.Write(new byte[] { 0xA5 }, 0, 1);
                Console.WriteLine("Sent handshake byte (0xA5)");

                // Wait for response (0x5A) with manual timeout
                int response = -1;
                int timeoutMs = 1000;
                int waitInterval = 250;
                int waited = 0;

                while (waited < timeoutMs)
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        response = _serialPort.ReadByte();
                        break;
                    }
                    Thread.Sleep(waitInterval);
                    waited += waitInterval;
                }

                if (response == 0x5A)
                {
                    Console.WriteLine("Handshake successful! Starting read thread...");

                    _continue = true;
                    readThread = new Thread(Read);
                    readThread.Start();
                    _isConnected = true;
                    return true; // Handshake successful
                }
                //if (response == -1)
                //{
                //    Console.WriteLine("Handshake successful! Starting read thread...");
                //    _serialPort.Write(new byte[] { 0xAA }, 0, 1);
                //    _continue = true;
                //    readThread = new Thread(Read);
                //    readThread.Start();
                //    _isConnected = true;
                //    return true; // Handshake successful
                //}
                else
                {

                    Console.WriteLine("Handshake failed or timed out.");
                    Console.WriteLine("Failed Data: " + response);
                    _serialPort.Close();
                    _isConnected = false;
                    return false; // Handshake failed

                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Handshake timed out — no response from Arduino.");
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _isConnected = false;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied to " + _serialPort.PortName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _isConnected = false;
            }
            return false; // Handshake failed
        }

        private void Read()
        {
            while (_continue)
            {
                try
                {
                    
                    var fData = bmsReader.GetCurrentData();
                    flightData = fData; // Store the flight data for later use
                    if (_serialPort.BytesToRead > 0 && _serialPort.BytesToRead < 2)
                    {
                        int incoming = _serialPort.ReadByte(); // Read one byte
                        byte command = (byte)incoming;

                        Console.WriteLine($"Received command: 0x{command:X2}");

                        // Check which simulator is running (priority: Falcon > DCS-BIOS > DCS)
                        bool isFalconRunning = falconConnector?.isFalconRunning() ?? false;
                        bool isDCSBIOSConnected = dcsBiosConnector?.IsDCSBIOSConnected() ?? false;
                        bool isDCSConnected = dcsConnector?.IsDCSConnected() ?? false;

                        if (fData == null && !isDCSBIOSConnected && !isDCSConnected)
                        {
                            SendResponse(command, new byte[] { 0x00 });
                            Console.WriteLine("No simulator data available, skipping command processing.");
                            continue;
                        }
                        else if (isFalconRunning)
                        {
                            // Handle Falcon BMS commands (highest priority)
                            HandleFalconCommand(command, fData);
                        }
                        else if (isDCSBIOSConnected)
                        {
                            // Handle DCS-BIOS commands (preferred over simple DCS)
                            HandleDCSBIOSCommand(command);
                        }
                        else if (isDCSConnected)
                        {
                            // Handle simple DCS commands
                            HandleDCSCommand(command);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10); // reduce CPU usage
                    }
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine("Serial IO error: " + ioEx.Message);
                    HandleDisconnection();
                }
                catch (InvalidOperationException opEx)
                {
                    Console.WriteLine("Serial operation error: " + opEx.Message);
                    HandleDisconnection();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected error: " + ex.Message);
                    HandleDisconnection();
                }

            }
        }

        private void HandleFalconCommand(byte command, FlightData fData)
        {
            switch (command)
                            {
                                //LightBits
                                case 0x01:
                                    byte[] lightBits = BitConverter.GetBytes(fData.lightBits);
                                    SendResponse(0x01, lightBits);
                                    break;
                                //LightBits2
                                case 0x02:
                                    byte[] lightBits2 = BitConverter.GetBytes(fData.lightBits2);
                                    SendResponse(0x02, lightBits2);
                                    break;
                                //LightBits2
                                case 0x03:
                                    byte[] lightBits3 = BitConverter.GetBytes(fData.lightBits3);
                                    SendResponse(0x03, lightBits3);
                                    break;
                                case 0x04:
                                    byte[] blinkBits = BitConverter.GetBytes(fData.blinkBits);
                                    SendResponse(0x04, blinkBits);
                                    break;
                                case 0x05:
                                    byte[] mergedDED = new byte[120];
                                    for (int i = 0; i < 5; i++)
                                    {
                                        byte[] norm = NormalizeLine(fData.DEDLines[i], fData.Invert[i]);
                                        Array.Copy(norm, 0, mergedDED, i * 24, 24);
                                    }
                                    SendResponse(0x05, mergedDED);
                                    break;
                                case 0x06:
                                    byte[] fuelFLow = BitConverter.GetBytes(fData.fuelFlow);
                                    //Console.WriteLine($"Fuel flow: {fData.fuelFlow}");
                                    SendResponse(0x06, fuelFLow);
                                    break;
                                case 0x07:
                                    byte[] instrLight = BitConverter.GetBytes(fData.instrLight);
                                    SendResponse(0x07, instrLight);
                                    break;
                                case 0x08:
                                    byte[] mergedPFL = new byte[120];
                                    for (int i = 0; i < 5; i++)
                                    {
                                        byte[] norm = NormalizeLine(fData.PFLLines[i], fData.PFLInvert[i]);
                                        Array.Copy(norm, 0, mergedPFL, i * 24, 24);
                                    }
                                    SendResponse(0x05, mergedPFL);
                                    break;
                                case 0x09:
                                    byte[] chaffCount = BitConverter.GetBytes(fData.ChaffCount);
                                    SendResponse(0x09, chaffCount);
                                    break;
                                case 0x10:
                                    byte[] flareCount = BitConverter.GetBytes(fData.FlareCount);
                                    SendResponse(0x10, flareCount);
                                    break;
                                case 0x11:
                                    byte[] floodConsole = BitConverter.GetBytes((byte)fData.floodConsole); //1-6
                                    SendResponse(0x11, new byte[] { 0x00 }); //floodConsole);
                                    break;
                                case 0x12:
                                    byte[] rpm = BitConverter.GetBytes(fData.rpm);
                                    SendResponse(0x12, rpm);
                                    break;
                                case 0x13:
                                    uint[] eBits = fData.ecmBits;

                                    // Convert the entire uint[] into a byte[] (4 bytes per uint)
                                    byte[] ecmBitsBytes = new byte[eBits.Length * sizeof(uint)];

                                    for (int i = 0; i < eBits.Length; i++)
                                    {
                                        byte[] bytes = BitConverter.GetBytes(eBits[i]);
                                        Array.Copy(bytes, 0, ecmBitsBytes, i * 4, 4);
                                    }
                                    // Send all the ecmBits at once
                                    SendResponse(0x13, ecmBitsBytes);
                                    break;
                                case 0x14:
                                    byte[] oilPress = BitConverter.GetBytes(fData.oilPressure);
                                    SendResponse(0x14, oilPress);
                                    break;
                                case 0x15:
                                    byte[] oilPress2 = BitConverter.GetBytes(fData.oilPressure2);
                                    SendResponse(0x15, oilPress2);
                                    break;
                                case 0x16:
                                    byte[] nozzlePos = BitConverter.GetBytes(fData.nozzlePos);
                                    SendResponse(0x16, nozzlePos);
                                    break;
                                case 0x17:
                                    byte[] nozzlePos2 = BitConverter.GetBytes(fData.nozzlePos2);
                                    SendResponse(0x17, nozzlePos2);
                                    break;
                                case 0x18:
                                    byte[] ftit = BitConverter.GetBytes(fData.ftit);
                                    SendResponse(0x18, ftit);
                                    break;
                                case 0x19:
                                    byte[] ftit2 = BitConverter.GetBytes(fData.ftit2);
                                    SendResponse(0x19, ftit2);
                                    break;
                                case 0x20:
                                    byte[] cabinAlt = BitConverter.GetBytes(fData.cabinAlt);
                                    SendResponse(0x20, cabinAlt);
                                    break;
                                case 0x21:
                                    byte[] kias = BitConverter.GetBytes(fData.kias);
                                    SendResponse(0x21, kias);
                                    break;
                                case 0x22:
                                    byte[] internalFuel = BitConverter.GetBytes(fData.internalFuel);
                                    SendResponse(0x22, internalFuel);
                                    break;
                                case 0x23:
                                    byte[] externalFuel = BitConverter.GetBytes(fData.externalFuel);
                                    SendResponse(0x23, externalFuel);
                                    break;
                                case 0x24:
                                    byte[] epuFuel = BitConverter.GetBytes(fData.epuFuel);
                                    SendResponse(0x24, epuFuel);
                                    break;
                                case 0x25:
                                    byte[] hydPressA = BitConverter.GetBytes(fData.hydPressureA);
                                    SendResponse(0x25, hydPressA);
                                    break;
                                case 0x26:
                                    byte[] hydPressB = BitConverter.GetBytes(fData.hydPressureB);
                                    SendResponse(0x26, hydPressB);
                                    break;
                                case 0x27:
                                    byte[] cmdsMode = BitConverter.GetBytes(fData.cmdsMode);
                                    SendResponse(0x27, cmdsMode);
                                    break;
                                case 0x28:
                                    byte[] uhfpreset = BitConverter.GetBytes(fData.BupUhfPreset);
                                    SendResponse(0x28, uhfpreset);
                                    break;
                                case 0x29:
                                    byte[] uhfradio = BitConverter.GetBytes(fData.BupUhfFreq);
                                    SendResponse(0x29, uhfradio);
                                    break;
                                case 0x30:
                                    byte[] speedBrake = BitConverter.GetBytes(fData.speedBrake);
                                    SendResponse(0x30, speedBrake);
                                    break;
                                case 0x31:
                                    byte[] iifBM1D1 = BitConverter.GetBytes(fData.iffBackupMode1Digit1);
                                    SendResponse(0x31, iifBM1D1);
                                    break;
                                case 0x32:
                                    byte[] iifBM1D2 = BitConverter.GetBytes(fData.iffBackupMode1Digit2);
                                    SendResponse(0x32, iifBM1D2);
                                    break;
                                case 0x33:
                                    byte[] iifBM3D1 = BitConverter.GetBytes(fData.iffBackupMode3ADigit1);
                                    SendResponse(0x33, iifBM3D1);
                                    break;
                                case 0x34:
                                    byte[] iifBM3D2 = BitConverter.GetBytes(fData.iffBackupMode3ADigit2);
                                    SendResponse(0x34, iifBM3D2);
                                    break;
                                case 0x99:
                                    Console.WriteLine(" Packet Failed CheckSum");
                                    Disconnect();
                                    break;
                                case 0x0F:
                                    SendResponse(0x0F, new byte[] { 0xAB });
                                    break;

                                case 0x5A:
                                    SendResponse(0xA5, new byte[] { 0x5A });
                                    break;
                                    
                                // Add more cases as needed
                                default:
                                    SendResponse(0x00, new byte[] { 0x00 });
                                    Console.WriteLine($"Falcon: Unknown command: 0x{command:X2}");
                                    break;
                            }
                        }

        private void HandleDCSBIOSCommand(byte command)
        {
            var dcsBiosData = dcsBiosConnector.GetFlightData();

            switch (command)
            {
                case 0x01: // LightBits - Map DCS-BIOS data to light bits
                    uint lightBits = 0;
                    if ((bool)dcsBiosData["master_caution"]) lightBits |= 0x01; // Master Caution
                    if ((bool)dcsBiosData["gear_down"]) lightBits |= 0x8000; // Landing gear
                    byte[] lightBitsBytes = BitConverter.GetBytes(lightBits);
                    SendResponse(0x01, lightBitsBytes);
                    break;

                case 0x05: // DED Lines - Send DCS-BIOS flight info
                    string[] dedLines = {
                        "DCS-BIOS CONNECTED",
                        $"AIRCRAFT: {dcsBiosData["aircraft_type"]}",
                        $"ALT: {dcsBiosData["altitude"]:F0} FT",
                        $"SPD: {dcsBiosData["speed"]:F1} KTS",
                        $"HDG: {dcsBiosData["heading"]:F0} DEG"
                    };

                    byte[] mergedDED = new byte[120];
                    for (int i = 0; i < 5; i++)
                    {
                        byte[] lineBytes = Encoding.ASCII.GetBytes(dedLines[i].PadRight(24).Substring(0, 24));
                        Array.Copy(lineBytes, 0, mergedDED, i * 24, 24);
                    }
                    SendResponse(0x05, mergedDED);
                    break;

                case 0x21: // Speed (kias)
                    float speed = Convert.ToSingle(dcsBiosData["speed"]);
                    byte[] speedBytes = BitConverter.GetBytes(speed);
                    SendResponse(0x21, speedBytes);
                    break;

                case 0x22: // Fuel (map to internal fuel)
                    float fuel = Convert.ToSingle(dcsBiosData["fuel"]);
                    byte[] fuelBytes = BitConverter.GetBytes(fuel);
                    SendResponse(0x22, fuelBytes);
                    break;

                case 0x38: // Heading (map to desired course)
                    float heading = Convert.ToSingle(dcsBiosData["heading"]);
                    byte[] headingBytes = BitConverter.GetBytes(heading);
                    SendResponse(0x38, headingBytes);
                    break;

                case 0x0F: // Handshake
                    SendResponse(0x0F, new byte[] { 0xAB });
                    break;

                case 0x5A: // Handshake response
                    SendResponse(0xA5, new byte[] { 0x5A });
                    break;

                default:
                    SendResponse(command, new byte[] { 0x00 });
                    Console.WriteLine($"DCS-BIOS: Unknown command: 0x{command:X2}");
                    break;
            }
        }

        private void HandleDisconnection()
        {
            _continue = false;
            _isConnected = false;

            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                    _serialPort.Close();
            }
            catch { }

            OnDataReceived?.Invoke(this, "Disconnected");  // UI can listen and toggle button
        }

        private void HandleDCSCommand(byte command)
        {
            var dcsData = dcsConnector.GetFlightData();

            switch (command)
            {
                case 0x01: // LightBits - Map DCS data to light bits
                    uint lightBits = 0;
                    if ((bool)dcsData["master_caution"]) lightBits |= 0x01; // Master Caution
                    byte[] lightBitsBytes = BitConverter.GetBytes(lightBits);
                    SendResponse(0x01, lightBitsBytes);
                    break;

                case 0x05: // DED Lines - Send DCS flight info
                    string[] dedLines = {
                        "DCS WORLD CONNECTED",
                        $"AIRCRAFT: {dcsData["aircraft_type"]}",
                        $"ALT: {dcsData["altitude"]:F0} FT",
                        $"SPD: {dcsData["speed"]:F1} KTS",
                        $"HDG: {dcsData["heading"]:F0} DEG"
                    };

                    byte[] mergedDED = new byte[120];
                    for (int i = 0; i < 5; i++)
                    {
                        byte[] lineBytes = Encoding.ASCII.GetBytes(dedLines[i].PadRight(24).Substring(0, 24));
                        Array.Copy(lineBytes, 0, mergedDED, i * 24, 24);
                    }
                    SendResponse(0x05, mergedDED);
                    break;

                case 0x21: // Speed (kias)
                    float speed = Convert.ToSingle(dcsData["speed"]);
                    byte[] speedBytes = BitConverter.GetBytes(speed);
                    SendResponse(0x21, speedBytes);
                    break;

                case 0x22: // Fuel (map to internal fuel)
                    float fuel = Convert.ToSingle(dcsData["fuel"]);
                    byte[] fuelBytes = BitConverter.GetBytes(fuel);
                    SendResponse(0x22, fuelBytes);
                    break;

                case 0x38: // Heading (map to desired course)
                    float heading = Convert.ToSingle(dcsData["heading"]);
                    byte[] headingBytes = BitConverter.GetBytes(heading);
                    SendResponse(0x38, headingBytes);
                    break;

                case 0x0F: // Handshake
                    SendResponse(0x0F, new byte[] { 0xAB });
                    break;

                case 0x5A: // Handshake response
                    SendResponse(0xA5, new byte[] { 0x5A });
                    break;

                default:
                    SendResponse(command, new byte[] { 0x00 });
                    Console.WriteLine($"DCS: Unknown command: 0x{command:X2}");
                    break;
            }
        }

        //Copied FROM DEDUINO Project , was good example of how to send DED accounting for Inverese.  https://github.com/uriba107/deduino_connector
        private byte[] NormalizeLine(string Disp, string Inv)
        /*
         * This function takes two strings LINE and INV and mashes them into a string that conforms with the font on the Arduino Display
         * This works for DED and PFL
         */
        {
            char[] NormLine = new char[26]; // Create the result buffer
            for (short j = 0; j < Disp.Length; j++) // run the length of the Display string
            {
                if (Inv[j] == 2) // check if the corresponding position in the INV line is "lit" - indicated by char(2)
                { // if inverted
                    if (char.IsLetter(Disp[j])) // if char is letter (always uppercase)
                    {
                        NormLine[j] = char.ToLower((Disp[j])); // lowercase it - which is the inverted in the custom font
                    }
                    else if (Disp[j] == 1) // if it's the selection arrows
                    {
                        NormLine[j] = (char)192; // that is the selection arrow stuff
                    }
                    else if (Disp[j] == 2) // if it's a DED "*"
                    {
                        NormLine[j] = (char)170;
                    }
                    else if (Disp[j] == 3) // // if it's a DED "_"
                    {
                        NormLine[j] = (char)223;
                    }
                    else if (Disp[j] == '~') // Arrow down (PFD)
                    {
                        NormLine[j] = (char)252;
                    }
                    else if (Disp[j] == '^') // degree simbol (doesn't work with +128 from some reason so manualy do it
                    {
                        NormLine[j] = (char)222;
                    }
                    else // for everything else - just add 128 to the ASCII value (i.e numbers and so on)
                    {
                        NormLine[j] = (char)(Disp[j] + 128);
                    }
                }
                else // if it's non inverted
                {
                    if (Disp[j] == 1) // Selector double arrow
                    {
                        NormLine[j] = '@';
                    }
                    else if (Disp[j] == 2) // if it's a DED "*"
                    {
                        NormLine[j] = '*';
                    }
                    else if (Disp[j] == 3) // if it's a DED "_"
                    {
                        NormLine[j] = '_';
                    }
                    else if (Disp[j] == '~') // Arrow down (PFD)
                    {
                        NormLine[j] = '|';
                    }
                    else
                    {
                        NormLine[j] = Disp[j];
                    }
                }

            }
            
                return Encoding.GetEncoding(1252).GetBytes(NormLine, 0, 24);
            
        }
        
        private void SendResponse(byte type, byte[] data)
        {
            List<byte> packet = new List<byte>();
            packet.Add(0xAA);
            packet.Add(type);
            packet.Add((byte)data.Length);
            packet.AddRange(data);

            byte checksum = (byte)(type + data.Length + data.Sum(b => b));
            packet.Add(checksum);

            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Write(packet.ToArray(), 0, packet.Count);
                Console.WriteLine("Sent response for type 0x" + type.ToString("X2"));
              //Console.WriteLine("Packet: " + BitConverter.ToString(packet.ToArray()).Replace("-", " "));
                //Console.WriteLine("Length: " + data.Length);
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }
        
        public void Disconnect()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _continue = false; // stop read loop
                    readThread?.Join(500); // wait for read thread to finish (optional timeout)
                    _serialPort.Close();
                    Console.WriteLine("Disconnected from port " + _serialPort.PortName);
                    _isConnected = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during disconnect: " + ex.Message);
                }
            }
        }

    }
}
