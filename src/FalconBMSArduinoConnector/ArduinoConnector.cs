using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        // Optional: connect logic (if needed later)
        private SerialPort _serialPort;

        public bool Connect(string portName, int baudRate = 115200, int handshakeTimeoutMs = 10000)
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate)
                {
                    ReadTimeout = 1000,   // Timeout in milliseconds
                    WriteTimeout = 1000,
                    NewLine = "\n"
                };

                //_serialPort.Parity = Parity.None;
                //_serialPort.StopBits = StopBits.One;
                //_serialPort.DataBits = 8;
                //_serialPort.Handshake = Handshake.None;
                //_serialPort.RtsEnable = true;

                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                _serialPort.Open();

                Console.WriteLine("Connected to Arduino on " + portName);
                // After opening serial port
                _serialPort.WriteLine("PING");

                var start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds < handshakeTimeoutMs)
                {
                    try
                    {
                        if (_serialPort.BytesToRead > 0)
                        {
                            string line = _serialPort.ReadLine()?.Trim();
                            Console.WriteLine("Received from Arduino: " + line);
                            if (line == "READY")
                            {
                                Console.WriteLine("Handshake received from Arduino on " + portName);
                                return true;
                            }
                        }
                    }
                    catch (TimeoutException) { }
                }

                Console.WriteLine("No handshake from Arduino on " + portName);
                _serialPort.Close();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect error: " + ex.Message);
                _serialPort?.Close();
                return false;
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            try
            {
                string indata = sp.ReadExisting();
                OnDataReceived?.Invoke(this, indata); // Fix: Use 'this' to reference the instance of ArduinoConnector
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Read timeout occurred.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading data: " + ex.Message);
                return;
            }
           
            //Console.Write(indata);
           
            
        }


        public void Disconnect()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort = null;
            }
        }

        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public void Send(string data)
        {
            if (IsConnected)
            {
                _serialPort.WriteLine(data);
                
            }
        }
        public void SendPacket(byte type, byte[] data)
        {
            List<byte> packet = new List<byte>();
            packet.Add(0xAA); // Start byte
            packet.Add(type);
            packet.Add((byte)data.Length);
            packet.AddRange(data);

            // Checksum: sum of type + length + data
            byte checksum = (byte)(type + data.Length + data.Sum(b => b));
            packet.Add(checksum);
            if (_serialPort != null && _serialPort.IsOpen)
            {
                // _ = WaitForReadyAsync();
               

                try
                {
                    
                    _serialPort.Write(packet.ToArray(), 0, packet.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending packet: " + ex.Message);

                }
                Console.WriteLine("Packet Sent ");
            }
       
        }

        public void SendDEDLines(string[] lines)
        {
            Console.WriteLine("Sending DED lines to Arduino22...");
            if (lines.Length != 5)
            {
                Console.WriteLine("DED must contain exactly 5 lines.");
                throw new ArgumentException("DED must contain exactly 5 lines.");
                
            }

            byte[] buffer = new byte[130]; // 5 × 26

            for (int i = 0; i < 5; i++)
            {
                string padded = (lines[i] ?? "").PadRight(26).Substring(0, 26);
                byte[] lineBytes = Encoding.ASCII.GetBytes(padded);
                Array.Copy(lineBytes, 0, buffer, i * 26, 26);
            }
            Console.WriteLine($"Sending packet type: {0x10}, length: {buffer.Length}");

            SendPacket(0x10, buffer);
        }

        private async Task WaitForReadyAsync()
        {
            while (true)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    int b = _serialPort.ReadByte();
                    if (b == 0x55) break;
                }
                await Task.Delay(5);
            }
        }

    }
}
