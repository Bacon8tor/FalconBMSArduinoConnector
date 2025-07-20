using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using F4SharedMem;
using F4SharedMem.Headers;

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
        // Optional: connect logic (if needed later)
        private SerialPort _serialPort;
        static bool _continue;
        private Thread readThread = null;
        private bool _isConnected = false;
        public bool ConnectSerial(String name)
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

            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;

            try
            {
                _serialPort.Open();
                Console.WriteLine("Opened port " + _serialPort.PortName);

                // Send handshake byte to Arduino
                _serialPort.Write(new byte[] { 0xA5 }, 0, 1);
                Console.WriteLine("Sent handshake byte (0xA5)");

                // Wait for response (0x5A) with manual timeout
                int response = -1;
                int timeoutMs = 1000;
                int waitInterval = 10;
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
                else
                {
                    Console.WriteLine("Handshake failed or timed out.");
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
                    if (_serialPort.BytesToRead > 0 && _serialPort.BytesToRead < 2)
                    {
                        int incoming = _serialPort.ReadByte(); // Read one byte
                        byte command = (byte)incoming;

                        Console.WriteLine($"Received command: 0x{command:X2}");

                        if (fData == null)
                        {   
                            SendResponse(command, new byte[] { 0x00 });
                            Console.WriteLine("Flight data is null, skipping command processing.");
                            continue;
                        }
                        else
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
                                    Console.WriteLine($"Fuel flow: {fData.fuelFlow}");
                                    SendResponse(0x06, fuelFLow);
                                    break;
                                case 0x0F:
                                    SendResponse(0x0F, new byte[] { 0xAB });
                                    break;
                                case 0xA5:
                                    SendResponse(0xA5, new byte[] { 0x5A });
                                    break;
                                    
                                // Add more cases as needed
                                default:
                                    SendResponse(0x00, new byte[] { 0x00 });
                                    Console.WriteLine($"Unknown command: 0x{command:X2}");
                                    break;


                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(10); // reduce CPU usage
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Read error: " + ex.Message);
                    break;
                }
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
