using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Windows.Forms;



namespace FalconBMSArduinoConnector
{
    public partial class Form1 : Form
    {
        FalconConnector falcon = new FalconConnector();
        //private Reader bmsReader = new Reader();
        //private Writer bmsWrite = new Writer();
        private Timer falconCheckTimer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //falcon.isFalconRunning();
            //CheckFalconStatus(); // first check

            falconCheckTimer = new Timer();
            falconCheckTimer.Interval = 1000;
            falconCheckTimer.Tick += (s, args) => CheckFalconStatus();
            falconCheckTimer.Start();
        }

        private void CheckFalconStatus()
        {
            
            if (falcon.isFalconRunning())
            {
               masterCaution_check.Checked = falcon.IsLightOn(LightBits.MasterCaution);

            }
        }


    }
}
