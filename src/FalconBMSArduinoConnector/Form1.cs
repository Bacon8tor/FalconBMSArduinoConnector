
using F4SharedMem.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Net.Http.Headers;
using System.Windows.Forms;



namespace FalconBMSArduinoConnector
{
    public partial class Form1 : Form
    {
        FalconConnector falcon = new FalconConnector();
        ArduinoConnector arduino = new ArduinoConnector();
        //private Reader bmsReader = new Reader();
        //private Writer bmsWrite = new Writer();
        private Timer falconCheckTimer;
        private Timer packetTimer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //falcon.isFalconRunning();
            CheckFalconStatus();

            serialPort_combo.DataSource = SerialPort.GetPortNames();


            falconCheckTimer = new Timer();
            falconCheckTimer.Interval = 500;
            falconCheckTimer.Tick += (s, args) => CheckFalconStatus();
            falconCheckTimer.Start();

            packetTimer = new System.Windows.Forms.Timer();
            packetTimer.Interval = 10; // Send every 10ms (adjust as needed)
            packetTimer.Tick += SendLightBitsTimer_Tick;

        }

        private void CheckFalconStatus()
        {

            if (falcon.isFalconRunning())
            {
                falconRunning.Checked = falcon.isFalconRunning();
                falconRunning.Text = "Falcon is Running";
                falconBuild_text.Text = "v." + falcon.GetFalconVersion();

                // Console.WriteLine(falcon.falconState.ToString());
                masterCaution_check.Checked = falcon.IsLightOn(LightBits.MasterCaution);
                tf_check.Checked = falcon.IsLightOn(LightBits.TF);
                gearLightFront_check.Checked = falcon.IsLightOn(LightBits3.NoseGearDown);
                gearLightFront_check.Checked = falcon.IsLightOn(LightBits3.LeftGearDown);
                seatArmed_check.Checked = falcon.IsLightOn(LightBits2.SEAT_ARM);


                // Console.WriteLine(falcon.GetFalconState());



            }
            else
            {
                falconRunning.Checked = false;
                falconRunning.Text = "Falcon is Not Running";
                falconBuild_text.Text = "v.";
                masterCaution_check.Checked = false;
                tf_check.Checked = false;
                gearLightFront_check.Checked = false;
                gearLightLeft_check.Checked = false;
                gearLightRight_check.Checked = false;
            }
        }

        private void update_comports(object sender, EventArgs e)
        {
            serialPort_combo.DataSource = SerialPort.GetPortNames();
        }

        private void connectToSerial(object sender, EventArgs e)
        {
            var portName = serialPort_combo.Text;
            if (!arduino.IsConnected)
            {
                if (arduino.Connect(portName))
                {
                    Console.WriteLine("Connected to " + portName);
                    serialConnect_button.Text = "Disconnect";
                    sendTest_button1.Enabled = true;

                }
                else { Console.WriteLine("Failed to connect to " + portName); }


            }
            else
            {
                arduino.Disconnect();
                if (arduino.IsConnected)
                {
                    Console.WriteLine("Failed to disconnect from " + portName);

                    return;
                }
                else
                {
                    Console.WriteLine("Disconnected from " + portName);
                    serialConnect_button.Text = "Connect";
                }
            }





        }

        private void sendTest1(object sender, EventArgs e)
        {
            if (arduino.IsConnected)
            {
                arduino.Send("FBAC");
                Console.WriteLine("Sent test message to Arduino.");
            }
            else
            {
                Console.WriteLine("Arduino is not connected. Cannot send message.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            arduino.Send(falcon.GetFlightData().DEDLines[0]);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            arduino.SendPacket(0x01, BitConverter.GetBytes(falcon.GetFlightData().lightBits));
        }

        private void sendLight_CheckedChange(object sender, EventArgs e)
        {
            if (sendlightbits_check.Checked)
            {
                packetTimer.Start();
            }
            else
            {
                packetTimer.Stop();
            }

        }
        private void SendLightBitsTimer_Tick(object sender, EventArgs e)
        {
            if (arduino.IsConnected)
            {
                // Send light bits to Arduino
                arduino.SendPacket(0x01, BitConverter.GetBytes(falcon.GetFlightData().lightBits));
                //arduino.SendPacket(0x02, BitConverter.GetBytes(falcon.GetFlightData().lightBits2));
                // arduino.SendPacket(0x03, BitConverter.GetBytes(falcon.GetFlightData().lightBits3));
                //Console.WriteLine("Sent light bits to Arduino.");
            }
            else
            {
                Console.WriteLine("Arduino is not connected. Cannot send light bits.");
            }
        }
    }
}
