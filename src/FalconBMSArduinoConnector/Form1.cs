
using F4SharedMem;
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
    public partial class FalconBMSArduinoConnector : Form
    {
        FalconConnector falcon = new FalconConnector();
        ArduinoConnector arduino = new ArduinoConnector();
        //private Reader bmsReader = new Reader();
        //private Writer bmsWrite = new Writer();
        private Timer falconCheckTimer;
        

        public FalconBMSArduinoConnector()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //falcon.isFalconRunning();
            CheckFalconStatus();

            serialPort_combo.DataSource = SerialPort.GetPortNames();


            falconCheckTimer = new Timer();
            falconCheckTimer.Interval = 50;
            falconCheckTimer.Tick += (s, args) => CheckFalconStatus();
            falconCheckTimer.Start();



        }

        private void CheckFalconStatus()
        {

            if (falcon.isFalconRunning())
            {
                falconRunning.Checked = falcon.isFalconRunning();
                falconRunning.Text = "Falcon is Running";
                falconBuild_text.Text = "v." + falcon.GetFalconVersion();
                
                var data = falcon.GetFlightData();
                //LightBits
                masterCaution_check.Checked = falcon.IsLightOn(LightBits.MasterCaution);
                tf_check.Checked = falcon.IsLightOn(LightBits.TF);
                oxyBrow_check.Checked = falcon.IsLightOn(LightBits.OXY_BROW);
                equipHot_check.Checked = falcon.IsLightOn(LightBits.EQUIP_HOT);
                onground_check.Checked = falcon.IsLightOn(LightBits.ONGROUND);
                engFire_check.Checked = falcon.IsLightOn(LightBits.ENG_FIRE);
                config_check.Checked = falcon.IsLightOn(LightBits.CONFIG);
                hyd_check.Checked = falcon.IsLightOn(LightBits.HYD);
                flcsABCD_check.Checked = falcon.IsLightOn(LightBits.Flcs_ABCD);
                flcs_check.Checked = falcon.IsLightOn(LightBits.FLCS);
                CAN_check.Checked = falcon.IsLightOn(LightBits.CAN);
                tlCFG_check.Checked = falcon.IsLightOn(LightBits.T_L_CFG);
                aoaAbove_check.Checked = falcon.IsLightOn(LightBits.AOAAbove);
                aoaOn_check.Checked = falcon.IsLightOn(LightBits.AOAOn);
                aoaBelow_check.Checked = falcon.IsLightOn(LightBits.AOABelow);

                //LightBits2
                gearLightFront_check.Checked = falcon.IsLightOn(LightBits3.NoseGearDown);
                gearLightFront_check.Checked = falcon.IsLightOn(LightBits3.LeftGearDown);
                seatArmed_check.Checked = falcon.IsLightOn(LightBits2.SEAT_ARM);


                //Show DED data
                DED_Line1_text.Text = data.DEDLines[0];
                DED_Line2_text.Text = data.DEDLines[1];
                DED_Line3_text.Text = data.DEDLines[2];
                DED_Line4_text.Text = data.DEDLines[3];
                DED_Line5_text.Text = data.DEDLines[4];

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
                    arduino.OnDataReceived += Send_DataToArduino;
                    Console.WriteLine("Connected to " + portName);
                    serialConnect_button.Text = "Disconnect";


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
            return;
        }

        private void Send_DataToArduino(object sender, string data)
        {
            // Handle incoming data from Arduino here
            Console.WriteLine("Data received from Arduino: " + data);
            if (falcon.GetFlightData() == null)
            {
                Console.WriteLine("Flight data is null. Cannot send light bits.");
                return;
            }
                   var flightData = falcon.GetFlightData();
            if (data.Trim() == "lb")
            {
                Console.WriteLine("Sending light bits to Arduino...");
                arduino.SendPacket(0x01, BitConverter.GetBytes(flightData.lightBits));
            }
            else if (data.Trim() == "lb2")
            {
                Console.WriteLine("Sending light bits 2 to Arduino...");
                arduino.SendPacket(0x02, BitConverter.GetBytes(flightData.lightBits2));
            }
            else if (data.Trim() == "lb3")
            {
                Console.WriteLine("Sending light bits 3 to Arduino...");
                arduino.SendPacket(0x03, BitConverter.GetBytes(flightData.lightBits3));
            }
            else if (data.Trim() == "DED0")
            {
                Console.WriteLine("Sending DED 0 data to Arduino...");
                //arduino.SendDEDLines(flightData.DEDLines);
                arduino.Send(flightData.DEDLines[0]);

            }
            else if (data.Trim() == "DED1")
            {
                Console.WriteLine("Sending DED 1 data to Arduino...");
                //arduino.SendDEDLines(flightData.DEDLines);
                arduino.Send(flightData.DEDLines[1]);

            }
            else if (data.Trim() == "DED2")
            {
                Console.WriteLine("Sending DED 2 data to Arduino...");
                //arduino.SendDEDLines(flightData.DEDLines);
                arduino.Send(flightData.DEDLines[2]);

            }
            else if (data.Trim() == "DED3")
            {
                Console.WriteLine("Sending DED 3 data to Arduino...");
                //arduino.SendDEDLines(flightData.DEDLines);
                arduino.Send(flightData.DEDLines[3]);

            }
            else if (data.Trim() == "DED4")
            {
                Console.WriteLine("Sending DED 4 data to Arduino...");
                //arduino.SendDEDLines(flightData.DEDLines);
                arduino.Send(flightData.DEDLines[4]);
            }

            }
    }
}
