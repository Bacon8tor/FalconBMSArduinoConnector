using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;



namespace FalconBMSArduinoConnector
{
    public partial class Form1 : Form
    {
        private Reader bmsReader = new Reader();
        private Timer falconCheckTimer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            CheckFalconStatus(); // first check

            falconCheckTimer = new Timer();
            falconCheckTimer.Interval = 1000;
            falconCheckTimer.Tick += (s, args) => CheckFalconStatus();
            falconCheckTimer.Start();
        }

        private void CheckFalconStatus()
        {
            bool isRunning = Process.GetProcessesByName("Falcon BMS").Length > 0;

            falconRunning.Checked = isRunning;
            falconRunning.Text = isRunning ? "Falcon Running" : "Falcon Not Running";
            falconRunning.ForeColor = isRunning ? Color.Green : Color.Red;

            Console.WriteLine(isRunning ? "Falcon is running" : "Falcon is NOT running");

            if (isRunning)
            {
                var data = bmsReader.GetCurrentData();
                falconBuild_text.Text = "Build: " + data.BMSVersionMajor.ToString() + "." +
                                       data.BMSVersionMinor.ToString() + "." +                                       
                                       data.BMSBuildNumber.ToString();
                falconBuild_text.Visible = true;
                if (data != null)
                {
                    // Example: check if Master Caution light is on
                    bool masterCaution = (data.lightBits & (uint)LightBits.MasterCaution) != 0;
                    masterCaution_check.Checked = masterCaution;
                    Console.WriteLine("Master Caution: " + (masterCaution ? "ON" : "OFF"));

                    // You can check others similarly:
                    bool seatArm = (data.lightBits2 & (uint)LightBits2.SEAT_ARM) != 0;
                    seatArmed_check.Checked = seatArm;
                    Console.WriteLine((uint)LightBits2.SEAT_ARM);
                    Console.WriteLine("Seat Armed: " + (seatArm ? "YES" : "NO"));

                    bool gearDown_L = (data.lightBits3 & (uint)LightBits3.LeftGearDown) != 0;
                    //add checkbox for Left Gear Down in your form
                    gearLightLeft_check.Checked = gearDown_L;
                    gearLightLeft_check.BackColor = gearDown_L ? Color.Green : Color.Red;
                    Console.WriteLine("Left Gear Down: " + (gearDown_L ? "YES" : "NO"));

                    
                }
            }
        }


    }
}
