using F4SharedMem;
using F4SharedMem.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace FalconBMSArduinoConnector
{
    public partial class FalconBMSArduinoConnector : Form
    {
        FalconConnector falcon = new FalconConnector();
        ArduinoConnector arduino = new ArduinoConnector();
        private Timer falconCheckTimer;
        List<ArduinoConnector> arduinoConnections = new List<ArduinoConnector>();

        private const string SaveFile = "arduino_tabs.xml";

        public class ArduinoTabInfo
        {
            public string PortName { get; set; }
        }

        public FalconBMSArduinoConnector()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
//            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(30, 30, 30);  // Dark theme

            CheckFalconStatus();

            LoadArduinoTabs();

            falconCheckTimer = new Timer();
            falconCheckTimer.Interval = 50;
            falconCheckTimer.Tick += (s, args) => CheckFalconStatus();
            falconCheckTimer.Start();
        }

        private void LoadArduinoTabs()
        {
            if (File.Exists(SaveFile))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(List<ArduinoTabInfo>));
                    using (var stream = File.OpenRead(SaveFile))
                    {
                        var tabs = (List<ArduinoTabInfo>)serializer.Deserialize(stream);
                        foreach (var tab in tabs)
                        {
                            AddArduinoConnectionTab(tab.PortName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load saved tabs: {ex.Message}");
                }
            }
            else
            {
                AddArduinoConnectionTab(); // add at least one default tab
            }
        }

        private void CheckFalconStatus()
        {

            if (falcon.isFalconRunning())
            {

                falconRunning.Checked = falcon.isFalconRunning();
                falconRunning.Text = falcon.GetFalconProcessName();
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
                refuelRDY_check.Checked = falcon.IsLightOn(LightBits.RefuelRDY);
                refuelAR_check.Checked = falcon.IsLightOn(LightBits.RefuelAR);
                refuelDSC_check.Checked = falcon.IsLightOn(LightBits.RefuelDSC);
                FltControlSys_check.Checked = falcon.IsLightOn(LightBits.FltControlSys);
                LEFlaps_check.Checked = falcon.IsLightOn(LightBits.LEFlaps);
                EngineFault_check.Checked = falcon.IsLightOn(LightBits.EngineFault);
                Overheat_check.Checked = falcon.IsLightOn(LightBits.Overheat);
                FuelLow_check.Checked = falcon.IsLightOn(LightBits.FuelLow);
                Avionics_check.Checked = falcon.IsLightOn(LightBits.Avionics);
                RadarAlt_check.Checked = falcon.IsLightOn(LightBits.RadarAlt);
                IFF_check.Checked = falcon.IsLightOn(LightBits.IFF);
                ECM_check.Checked = falcon.IsLightOn(LightBits.ECM);
                Hook_check.Checked = falcon.IsLightOn(LightBits.Hook);
                NWSFail_check.Checked = falcon.IsLightOn(LightBits.NWSFail);
                CabinPress_check.Checked = falcon.IsLightOn(LightBits.CabinPress);
                AutoPilotOn_check.Checked = falcon.IsLightOn(LightBits.AutoPilotOn);
                TFR_STBY_check.Checked = falcon.IsLightOn(LightBits.TFR_STBY);


                //LightBits2
                HandOff_check.Checked = falcon.IsLightOn(LightBits2.HandOff);
                Launch_check.Checked = falcon.IsLightOn(LightBits2.Launch);
                PriMode_check.Checked = falcon.IsLightOn(LightBits2.PriMode);
                Naval_Check.Checked = falcon.IsLightOn(LightBits2.Naval);
                Unk_check.Checked = falcon.IsLightOn(LightBits2.Unk);
                TgtSep_check.Checked = falcon.IsLightOn(LightBits2.TgtSep);
                Go_check.Checked = falcon.IsLightOn(LightBits2.Go);
                NoGo_check.Checked = falcon.IsLightOn(LightBits2.NoGo);
                Degr_check.Checked = falcon.IsLightOn(LightBits2.Degr);
                Rdy_check.Checked = falcon.IsLightOn(LightBits2.Rdy);
                ChaffLo_check.Checked = falcon.IsLightOn(LightBits2.ChaffLo);
                FlareLo_check.Checked = falcon.IsLightOn(LightBits2.FlareLo);
                AuxSrch_check.Checked = falcon.IsLightOn(LightBits2.AuxSrch);
                AuxAct_check.Checked = falcon.IsLightOn(LightBits2.AuxAct);
                AuxLow_check.Checked = falcon.IsLightOn(LightBits2.AuxLow);
                AuxPwr_check.Checked = falcon.IsLightOn(LightBits2.AuxPwr);
                EcmPwr_check.Checked = falcon.IsLightOn(LightBits2.EcmPwr);
                EcmFail_check.Checked = falcon.IsLightOn(LightBits2.EcmFail);
                FwdFuelLow_check.Checked = falcon.IsLightOn(LightBits2.FwdFuelLow);
                AftFuelLow_check.Checked = falcon.IsLightOn(LightBits2.AftFuelLow);
                EPUOn_check.Checked = falcon.IsLightOn(LightBits2.EPUOn);
                JFSOn_check.Checked = falcon.IsLightOn(LightBits2.JFSOn);
                SEC_check.Checked = falcon.IsLightOn(LightBits2.SEC);
                OXY_LOW_check.Checked = falcon.IsLightOn(LightBits2.OXY_LOW);
                PROBEHEAT_check.Checked = falcon.IsLightOn(LightBits2.PROBEHEAT);
                SEAT_ARM_check.Checked = falcon.IsLightOn(LightBits2.SEAT_ARM);
                BUC_check.Checked = falcon.IsLightOn(LightBits2.BUC);
                FUEL_OIL_HOT_check.Checked = falcon.IsLightOn(LightBits2.FUEL_OIL_HOT);
                ANTI_SKID_check.Checked = falcon.IsLightOn(LightBits2.ANTI_SKID);
                TFR_ENGAGED_check.Checked = falcon.IsLightOn(LightBits2.TFR_ENGAGED);
                GEARHANDLE_check.Checked = falcon.IsLightOn(LightBits2.GEARHANDLE);
                ENGINE_check.Checked = falcon.IsLightOn(LightBits2.ENGINE);

                //Lightbits3
                gearLightFront_check.Checked = falcon.IsLightOn(LightBits3.NoseGearDown);
                gearLightFront_check.Checked = falcon.IsLightOn(LightBits3.LeftGearDown);


                // Show flight data

                //Show DED data
                try
                {
                    if (data != null)
                    {
                        DED_Line1_text.Text = data.DEDLines[0].ToUpper();
                        DED_Line2_text.Text = data.DEDLines[1].ToUpper();
                        DED_Line3_text.Text = data.DEDLines[2].ToUpper();
                        DED_Line4_text.Text = data.DEDLines[3].ToUpper();
                        DED_Line5_text.Text = data.DEDLines[4].ToUpper();

                        isntrLights_text.Text = "Instrument Lights: " + data.instrLight.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error displaying DED data: " + ex.Message);

                }
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
                isntrLights_text.Text = "Instrument Lights: ";
            }

        }
        private void SaveArduinoTabs()
        {
            var data = new List<ArduinoTabInfo>();

            foreach (TabPage page in tabControl_Arduinos.TabPages)
            {
                var combo = page.Controls.OfType<ComboBox>().FirstOrDefault();
                if (combo != null)
                {
                    data.Add(new ArduinoTabInfo { PortName = combo.Text });
                }
            }

            try
            {
                var serializer = new XmlSerializer(typeof(List<ArduinoTabInfo>));
                using (var stream = File.Create(SaveFile))
                {
                    serializer.Serialize(stream, data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save tabs: {ex.Message}");
            }
        }

        private void AddArduinoConnectionTab(string selectedPort = null)
        {
            var connector = new ArduinoConnector();
            arduinoConnections.Add(connector);

            var tabPage = new TabPage($"Arduino {arduinoConnections.Count}");

            var comboBox = new ComboBox() { Left = 10, Top = 10, Width = 100 };
            var button = new Button() { Text = "Connect", Left = 120, Top = 10 };
            var removeButton = new Button() { Text = "Remove", Left = 200, Top = 90 };
            button.FlatStyle = FlatStyle.Flat;
            removeButton.FlatStyle = FlatStyle.Flat;

            comboBox.DataSource = SerialPort.GetPortNames();
            if (!string.IsNullOrEmpty(selectedPort) && comboBox.Items.Contains(selectedPort))
                comboBox.SelectedItem = selectedPort;

            comboBox.DropDown += (s, e) =>
            {
                string currentSelection = comboBox.Text;
                var ports = SerialPort.GetPortNames();
                comboBox.DataSource = null;
                comboBox.DataSource = ports;

                if (ports.Contains(currentSelection))
                    comboBox.SelectedItem = currentSelection;
            };

            button.Click += (s, args) =>
            {
                if (!connector.IsConnected)
                {
                    if (connector.ConnectSerial(comboBox.Text))
                    {
                        button.Text = "Disconnect";
                        Console.WriteLine($"Connected to {comboBox.Text}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect: " + comboBox.Text );
                    }
                }
                else
                {
                    connector.Disconnect();
                    button.Text = "Connect";
                    Console.WriteLine($"Disconnected from {comboBox.Text}");
                }
            };

            removeButton.Click += (s, args) =>
            {
                if (connector.IsConnected)
                {
                    connector.Disconnect();
                    Console.WriteLine("Disconnected before removal.");
                }

                int index = tabControl_Arduinos.TabPages.IndexOf(tabPage);
                if (index >= 0 && index < arduinoConnections.Count)
                {
                    arduinoConnections.RemoveAt(index);
                }

                tabControl_Arduinos.TabPages.Remove(tabPage);
                SaveArduinoTabs();
            };
            connector.OnDataReceived += (s, msg) =>
            {
                if (msg == "Disconnected")
                {
                    Invoke((Action)(() => {
                        button.Text = "Connect";
                        button.Enabled = true;
                    }));
                }
            };


            tabPage.Controls.Add(comboBox);
            tabPage.Controls.Add(button);
            tabPage.Controls.Add(removeButton);

            tabControl_Arduinos.TabPages.Add(tabPage);
        }

        private void addArduinoButton_Click(object sender, EventArgs e)
        {
            AddArduinoConnectionTab();
            SaveArduinoTabs();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            foreach (var connector in arduinoConnections)
            {
                connector.Disconnect();
            }
            Console.WriteLine("All Arduino connections closed.");
            SaveArduinoTabs();
        }
    }
}
