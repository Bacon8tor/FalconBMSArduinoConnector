using System;
using System.Collections.Generic;
using System.IO.Ports;

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

        // Optional: connect logic (if needed later)
        private SerialPort _serialPort;

        public bool Connect(string portName, int baudRate = 115200, int handshakeTimeoutMs = 10000)
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate)
                {
                    ReadTimeout = 100,   // Timeout in milliseconds
                    WriteTimeout = 1000,
                    NewLine = "\n"
                };

                _serialPort.Open();
                Console.WriteLine("Connected to Arduino on " + portName);
                var start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds < handshakeTimeoutMs)
                {
                    try
                    {
                        string line = _serialPort.ReadLine()?.Trim();
                        Console.WriteLine($"Received from {portName}: {line}");
                        if (line == "READY")
                        {
                            Console.WriteLine("Handshake received from Arduino on " + portName);
                            SelectedPort = portName;
                            return true;
                        }
                    }
                    //catch (TimeoutException)
                    //{
                    //    // Just wait — don’t log every timeout

                    //}
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Serial error on {portName}: {ex.Message}");
                        break;
                    }
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

        

    }
}
