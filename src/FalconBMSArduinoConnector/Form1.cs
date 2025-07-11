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
        private Writer bmsWrite = new Writer();
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

                    checkLightBits(data);
                    

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

        private void checkLightBits(FlightData data)
        {
            // Check if the light bits are set and update the UI accordingly
            // This is a placeholder for actual light bit checking logic
            // You can add more checks for other light bits as needed
            Console.WriteLine("Checking light bits...");
            // Example: check if Master Caution light is on
            bool masterCaution = (data.lightBits & (uint)LightBits.MasterCaution) != 0;
            masterCaution_check.Checked = masterCaution;
            Console.WriteLine("Master Caution: " + (masterCaution ? "ON" : "OFF"));

            bool tf = (data.lightBits & (uint)LightBits.TF) != 0;
            tf_check.Checked = tf;
            Console.WriteLine("TF Light: " + (tf ? "ON" : "OFF"));

            bool oxyBrow = (data.lightBits & (uint)LightBits.OXY_BROW) != 0;
            //oxyBrow_check.Checked = oxyBrow;
            Console.WriteLine("Oxygen Brow Light: " + (oxyBrow ? "ON" : "OFF"));

            bool equipHot = (data.lightBits & (uint)LightBits.EQUIP_HOT) != 0;
            //equipHot_check.Checked = equipHot;
            Console.WriteLine("Equipment Hot Light: " + (equipHot ? "ON" : "OFF"));

            bool onground = (data.lightBits & (uint)LightBits.ONGROUND) != 0;
            //onground_check.Checked = onground;
            Console.WriteLine("On Ground Light: " + (onground ? "ON" : "OFF"));

            bool engFire = (data.lightBits & (uint)LightBits.ENG_FIRE) != 0;
            //engFire_check.Checked = engFire;
            Console.WriteLine("Engine Fire Light: " + (engFire ? "ON" : "OFF"));

            bool config = (data.lightBits & (uint)LightBits.CONFIG) != 0;
            //config_check.Checked = config;
            Console.WriteLine("Config Light: " + (config ? "ON" : "OFF"));

            bool hyd = (data.lightBits & (uint)LightBits.HYD) != 0;
            //hyd_check.Checked = hyd;
            Console.WriteLine("Hydraulic Light: " + (hyd ? "ON" : "OFF"));

            bool flcsABCD = (data.lightBits & (uint)LightBits.Flcs_ABCD) != 0;
            //flcsABCD_check.Checked = flcsABCD;
            Console.WriteLine("FLCS ABCD Light: " + (flcsABCD ? "ON" : "OFF"));

            bool can = (data.lightBits & (uint)LightBits.CAN) != 0;
            //can_check.Checked = can;
            Console.WriteLine("CAN Light: " + (can ? "ON" : "OFF"));

            bool tlCFG = (data.lightBits & (uint)LightBits.T_L_CFG) != 0;
            //tlCFG_check.Checked = tlCFG;
            Console.WriteLine("T/L Config Light: " + (tlCFG ? "ON" : "OFF"));

            bool aoaAbove = (data.lightBits & (uint)LightBits.AOAAbove) != 0;
            //aoaAbove_check.Checked = aoaAbove;
            Console.WriteLine("AOA Above Light: " + (aoaAbove ? "ON" : "OFF"));

            bool aoaBelow = (data.lightBits & (uint)LightBits.AOABelow) != 0;
            //aoaBelow_check.Checked = aoaBelow;
            Console.WriteLine("AOA Below Light: " + (aoaBelow ? "ON" : "OFF"));
             
            bool aoaON = (data.lightBits & (uint)LightBits.AOAOn) != 0;
            //aoaON_check.Checked = aoaON;
            Console.WriteLine("AOA On Light: " + (aoaON ? "ON" : "OFF"));

            bool rflReady = (data.lightBits & (uint)LightBits.RefuelRDY) != 0;
            //rflReady_check.Checked = rflReady;
            Console.WriteLine("Refuel Ready Light: " + (rflReady ? "ON" : "OFF"));

            bool rflAR = (data.lightBits & (uint)LightBits.RefuelAR) != 0;
            //rflAR_check.Checked = rflAR;
            Console.WriteLine("Refuel AR Light: " + (rflAR ? "ON" : "OFF"));

            bool rflDisc = (data.lightBits & (uint)LightBits.RefuelDSC) != 0;
            //rflDisc_check.Checked = rflDisc;
            Console.WriteLine("Refuel Disconnect Light: " + (rflDisc ? "ON" : "OFF"));


        }


    }
}
