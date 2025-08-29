using F4SharedMem;
using F4SharedMem.Headers;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Forms;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FalconBMSArduinoConnector
{
    public partial class FalconBMSArduinoConnector : MetroForm
    {
        FalconConnector falcon = new FalconConnector();
        DCSConnector dcs = new DCSConnector();
        List<ArduinoConnector> arduinoConnections = new List<ArduinoConnector>();
        private Timer falconCheckTimer;
        private MetroStyleManager metroStyleManager;
        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private Font DEDFont = null;
        private MetroFramework.MetroThemeStyle currentTheme;
        private MetroFramework.MetroColorStyle currentStyle;
        private const string SettingsFile = "user_settings.xml";
        private const string SaveFile = "arduino_tabs.xml";

        public class ArduinoTabInfo
        {
            public string PortName { get; set; }
            public string TabName { get; set; }
        }
        public class UserSettings
        {
            public MetroThemeStyle Theme { get; set; }
            public MetroColorStyle Style { get; set; }
        }
        
        public FalconBMSArduinoConnector()
        {
            InitializeComponent();

            metroStyleManager = new MetroStyleManager();
            metroStyleManager.Owner = this;
            metroStyleManager.Theme = MetroThemeStyle.Dark;
            metroStyleManager.Style = MetroColorStyle.Blue;
            this.StyleManager = metroStyleManager;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (var res in resourceNames)
            {
                Console.WriteLine("Embedded resource: " + res);
            }

            LoadSettings();

            LoadCheckBoxes();
            CheckFalconStatus();
            LoadArduinoTabs();
            LoadCustomFont();
            
            
           

            falconCheckTimer = new Timer();
            falconCheckTimer.Interval = 50;
            falconCheckTimer.Tick += (s, args) => CheckFalconStatus();
            falconCheckTimer.Start();
             metroDataPanel.Dock = DockStyle.Fill;
            
        }

        //FORM LOADING & SAVING
        public void LoadCustomFont()
        {

            try
            {
                var fontStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("FalconBMSArduinoConnector.Resources.falconded.ttf");

                if (fontStream != null)
                {
                    byte[] fontData = new byte[fontStream.Length];
                    fontStream.Read(fontData, 0, (int)fontStream.Length);
                    fontStream.Close();

                    IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                    System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                    privateFonts.AddMemoryFont(fontPtr, fontData.Length);
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

                    Font customFont = new Font(privateFonts.Families[0], 12f);
                    DEDFont = customFont;
                    metroDEDLabel_1.Font = DEDFont;
                    metroDEDLabel_2.Font = DEDFont;
                    metroDEDLabel_3.Font = DEDFont;
                    metroDEDLabel_4.Font = DEDFont;
                    metroDEDLabel_5.Font = DEDFont;
                }
                else
                {
                    Console.WriteLine("Failed to load font stream: resource not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading custom font: {ex.Message}");
            }

        }
        private void SaveSettings()
        {
            try
            {
                var settings = new UserSettings
                {
                    Theme = metroStyleManager.Theme,
                    Style = metroStyleManager.Style
                };

                var serializer = new XmlSerializer(typeof(UserSettings));
                using (var stream = File.Create(SettingsFile))
                {
                    serializer.Serialize(stream, settings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}");
            }
        }
        private void LoadSettings()
        {
            if (!File.Exists(SettingsFile))
                return;

            try
            {
                var serializer = new XmlSerializer(typeof(UserSettings));
                using (var stream = File.OpenRead(SettingsFile))
                {
                    var settings = (UserSettings)serializer.Deserialize(stream);
                    metroStyleManager.Theme = settings.Theme;
                    metroStyleManager.Style = settings.Style;

                    // Apply loaded settings to form and controls
                    this.Theme = metroStyleManager.Theme;
                    this.Style = metroStyleManager.Style;
                    this.StyleManager = metroStyleManager;
                    ApplyThemeToControls(this.Controls);
                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load settings: {ex.Message}");
            }
        }
        private void LoadSettingsPanel()
        {
            metroSettingsPanel.Controls.Clear(); // Clear old controls if needed

            // Label for Theme
            var themeLabel = new MetroFramework.Controls.MetroLabel()
            {
                Text = "Theme:",
                Location = new Point(20, 20)
            };
            metroSettingsPanel.Controls.Add(themeLabel);

            // ComboBox for Theme Selection
            var themeComboBox = new MetroFramework.Controls.MetroComboBox()
            {
                Location = new Point(150, 20),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            themeComboBox.Items.AddRange(new string[] { "Light", "Dark" });
            themeComboBox.SelectedIndexChanged += ThemeComboBox_SelectedIndexChanged;
            metroSettingsPanel.Controls.Add(themeComboBox);

            // Label for Style
            var styleLabel = new MetroFramework.Controls.MetroLabel()
            {
                Text = "Style:",
                Location = new Point(20, 60)
            };
            metroSettingsPanel.Controls.Add(styleLabel);

            // ComboBox for Style Selection
            var styleComboBox = new MetroFramework.Controls.MetroComboBox()
            {
                Location = new Point(150, 60),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Add MetroColorStyle enum names
            foreach (var style in Enum.GetNames(typeof(MetroFramework.MetroColorStyle)))
                styleComboBox.Items.Add(style);

            styleComboBox.SelectedIndexChanged += StyleComboBox_SelectedIndexChanged;
            metroSettingsPanel.Controls.Add(styleComboBox);

            // Optionally, preload current settings
            themeComboBox.SelectedItem = this.Theme.ToString();
            styleComboBox.SelectedItem = this.Style.ToString();
        }
        private void ShowPanel(MetroPanel panelToShow)
        {
            metroHomePanel.Visible = false;
            metroSettingsPanel.Visible = false;
            metroDataPanel.Visible = false;

            panelToShow.Visible = true;
        }
        private void LoadCheckBoxes()
        {
            // metroCheckBox1 to metroCheckBox29
            metroCheckBox1.Text = "Master Caution";       // LightBits.MasterCaution
            metroCheckBox2.Text = "TF";                   // LightBits.TF
            metroCheckBox3.Text = "OXY BROW";             // LightBits.OXY_BROW
            metroCheckBox4.Text = "EQUIP HOT";            // LightBits.EQUIP_HOT
            metroCheckBox5.Text = "ENG FIRE";             // LightBits.ENG_FIRE
            metroCheckBox6.Text = "CONFIG";               // LightBits.CONFIG
            metroCheckBox7.Text = "HYD";                  // LightBits.HYD
            metroCheckBox8.Text = "FLCS ABCD";            // LightBits.Flcs_ABCD
            metroCheckBox9.Text = "FLCS";                 // LightBits.FLCS
            metroCheckBox10.Text = "CAN";                 // LightBits.CAN
            metroCheckBox11.Text = "T-L CFG";             // LightBits.T_L_CFG
            metroCheckBox12.Text = "AOA Above";           // LightBits.AOAAbove
            metroCheckBox13.Text = "AOA On";              // LightBits.AOAOn
            metroCheckBox14.Text = "AOA Below";           // LightBits.AOABelow
            metroCheckBox15.Text = "Refuel RDY";          // LightBits.RefuelRDY
            metroCheckBox16.Text = "Refuel AR";           // LightBits.RefuelAR
            metroCheckBox17.Text = "Refuel DSC";          // LightBits.RefuelDSC
            metroCheckBox18.Text = "Flight Control Sys";  // LightBits.FltControlSys
            metroCheckBox19.Text = "LE Flaps";            // LightBits.LEFlaps
            metroCheckBox20.Text = "Engine Fault";        // LightBits.EngineFault
            metroCheckBox21.Text = "Overheat";            // LightBits.Overheat
            metroCheckBox22.Text = "Fuel Low";            // LightBits.FuelLow
            metroCheckBox23.Text = "Avionics";            // LightBits.Avionics
            metroCheckBox24.Text = "Radar Alt";           // LightBits.RadarAlt
            metroCheckBox25.Text = "IFF";                 // LightBits.IFF
            metroCheckBox26.Text = "ECM";                 // LightBits.ECM
            metroCheckBox27.Text = "Hook";                // LightBits.Hook
            metroCheckBox28.Text = "NWS Fail";            // LightBits.NWSFail
            metroCheckBox29.Text = "Cabin Press";         // LightBits.CabinPress
            // From LightBits2 enum
            metroCheckBox30.Text = "HandOff";          // Threat Warning Prime
            metroCheckBox31.Text = "Launch";
            metroCheckBox32.Text = "PriMode";
            metroCheckBox33.Text = "Naval";
            metroCheckBox34.Text = "Unk";
            metroCheckBox35.Text = "TgtSep";

            metroCheckBox36.Text = "GO";               // EWS
            metroCheckBox37.Text = "NO GO";
            metroCheckBox38.Text = "DEGR";
            metroCheckBox39.Text = "RDY";
            metroCheckBox40.Text = "Chaff Low";
            metroCheckBox41.Text = "Flare Low";

            metroCheckBox42.Text = "Aux Search";       // Aux Threat Warning
            metroCheckBox43.Text = "Aux Active";
            metroCheckBox44.Text = "Aux Low";
            metroCheckBox45.Text = "Aux Power";

            metroCheckBox46.Text = "ECM Power";        // ECM and Caution
            metroCheckBox47.Text = "ECM Fail";
            metroCheckBox48.Text = "FWD Fuel Low";
            metroCheckBox49.Text = "AFT Fuel Low";
            metroCheckBox50.Text = "EPU On";
            metroCheckBox51.Text = "JFS On";
            metroCheckBox52.Text = "SEC";
            metroCheckBox53.Text = "OXY LOW";
            metroCheckBox54.Text = "PROBE HEAT";
            metroCheckBox55.Text = "SEAT ARM";
            metroCheckBox56.Text = "BUC";
            metroCheckBox57.Text = "FUEL/OIL HOT";
            metroCheckBox58.Text = "ANTI SKID";
            metroCheckBox59.Text = "TFR ENGAGED";
            metroCheckBox60.Text = "GEAR HANDLE";
            metroCheckBox61.Text = "ENGINE";

            //lightbits3
            metroCheckBox62.Text = "Flcs PMG";
            metroCheckBox63.Text = "Main Gen";
            metroCheckBox64.Text = "Stby Gen";
            metroCheckBox65.Text = "EPU Gen";
            metroCheckBox66.Text = "EPU PMG";
            metroCheckBox67.Text = "To FLCS";
            metroCheckBox68.Text = "FLCS Relay";
            metroCheckBox69.Text = "Battery Fail";

            metroCheckBox70.Text = "Hydrazine";
            metroCheckBox71.Text = "Air";

            metroCheckBox72.Text = "Elec Fault";
            metroCheckBox73.Text = "LEF Fault";

            metroCheckBox74.Text = "On Ground";
            metroCheckBox75.Text = "FLCS BIT Run";
            metroCheckBox76.Text = "FLCS BIT Fail";
            metroCheckBox77.Text = "DBU Warn";
            metroCheckBox78.Text = "Nose Gear Down";
            metroCheckBox79.Text = "Left Gear Down";
            metroCheckBox80.Text = "Right Gear Down";
            metroCheckBox81.Text = "Park Brake On";
            metroCheckBox82.Text = "Power Off";

            metroCheckBox83.Text = "CADC";
            metroCheckBox84.Text = "Speed Brake";

            metroCheckBox85.Text = "System Test";
            metroCheckBox86.Text = "MC Announced";

            metroCheckBox87.Text = "MLG WOW";
            metroCheckBox88.Text = "NLG WOW";

            metroCheckBox89.Text = "ATF Not Engaged";
            metroCheckBox90.Text = "Inlet Icing";




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
                            AddArduinoConnectionTab(tab.PortName, tab.TabName);
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
                AddArduinoConnectionTab();
            }
        }
        private void SaveArduinoTabs()
        {
            var data = new List<ArduinoTabInfo>();

            foreach (MetroTabPage page in metroTabControl1.TabPages)
            {
                var combo = page.Controls.OfType<MetroComboBox>().FirstOrDefault();
                if (combo != null)
                {
                    data.Add(new ArduinoTabInfo
                    {
                        PortName = combo.Text,
                        TabName = page.Text
                    });
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
        private void AddArduinoConnectionTab(string selectedPort = null, string tabName = null)
        {
            var connector = new ArduinoConnector();
            arduinoConnections.Add(connector);

            var tabPage = new MetroTabPage
            {
                Text = string.IsNullOrEmpty(tabName) ? $"Arduino {arduinoConnections.Count}" : tabName
            };


            var comboBox = new MetroComboBox() { Left = 5, Top = 15, Width = 121, Theme = this.Theme };
            var button = new MetroButton() { Text = "Connect", Left = 150, Top = 20, Theme = this.Theme };
            var removeButton = new MetroButton() { Text = "Remove", Left = 5, Top = 150, Theme = this.Theme };
            var DTRcheckbox = new MetroCheckBox() { Width = 220, Left = 150, Top = 55, Text = "Micro/ProMicro/Leonardo_Device", Theme = this.Theme };
            var renameButton = new MetroButton() { Text = "Rename", Left = 100, Top = 150, Theme = this.Theme };

            var ports = SerialPort.GetPortNames().Distinct().ToArray();
            comboBox.DataSource = ports;
            if (!string.IsNullOrEmpty(selectedPort) && comboBox.Items.Contains(selectedPort))
                comboBox.SelectedItem = selectedPort;

            comboBox.DropDown += (s, e) =>
            {
                string currentSelection = comboBox.Text;
                var refreshedPorts = SerialPort.GetPortNames().Distinct().ToArray();
                comboBox.DataSource = null;
                comboBox.DataSource = refreshedPorts;
                if (refreshedPorts.Contains(currentSelection))
                    comboBox.SelectedItem = currentSelection;
            };

            button.Click += (s, args) =>
            {
                if (!connector.IsConnected)
                {
                    if (connector.ConnectSerial(comboBox.Text, DTRcheckbox.Checked))
                    {
                        button.Text = "Disconnect";
                        Console.WriteLine($"Connected to {comboBox.Text}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect: " + comboBox.Text);
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

                int index = metroTabControl1.TabPages.IndexOf(tabPage);
                if (index >= 0 && index < arduinoConnections.Count)
                {
                    arduinoConnections.RemoveAt(index);
                }

                metroTabControl1.TabPages.Remove(tabPage);
                SaveArduinoTabs();
            };

            connector.OnDataReceived += (s, msg) =>
            {
                if (msg == "Disconnected")
                {
                    Invoke((Action)(() =>
                    {
                        button.Text = "Connect";
                        button.Enabled = true;
                    }));
                }
            };


            renameButton.Click += (s, e) =>
            {
                string input = Interaction.InputBox("Enter new tab name:", "Rename Tab", tabPage.Text);

                if (!string.IsNullOrEmpty(input))
                {
                    tabPage.Text = input;
                    SaveArduinoTabs(); // Save new name
                }
            };

            tabPage.Controls.Add(removeButton);
            tabPage.Controls.Add(DTRcheckbox);

            tabPage.Controls.Add(button);
            tabPage.Controls.Add(comboBox);
            tabPage.Controls.Add(renameButton);


            tabPage.Theme = this.Theme;
            metroTabControl1.TabPages.Add(tabPage);
        }
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            foreach (var connector in arduinoConnections)
            {
                connector.Disconnect();
            }
            Console.WriteLine("All Arduino connections closed.");
            SaveArduinoTabs();
            SaveSettings(); // Save theme/style on exit
        }


        //THEMING
        private void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                var type = control.GetType();

                // Check if the control has Theme property
                var themeProp = type.GetProperty("Theme");
                if (themeProp != null && themeProp.CanWrite)
                {
                    themeProp.SetValue(control, this.Theme);
                }

                // Check if the control has Style property
                var styleProp = type.GetProperty("Style");
                if (styleProp != null && styleProp.CanWrite)
                {
                    styleProp.SetValue(control, this.Style);
                }

                // Recurse for child controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }
        private void ThemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = sender as MetroFramework.Controls.MetroComboBox;
            var newTheme = combo.SelectedItem.ToString() == "Dark"
                ? MetroThemeStyle.Dark
                : MetroThemeStyle.Light;

            metroStyleManager.Theme = newTheme;
            this.Theme = newTheme;
            this.StyleManager = metroStyleManager;

            ApplyThemeToControls(this.Controls);
            this.Refresh();
        }
        private void StyleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = sender as MetroFramework.Controls.MetroComboBox;
            var newStyle = (MetroColorStyle)Enum.Parse(typeof(MetroColorStyle), combo.SelectedItem.ToString());

            metroStyleManager.Style = newStyle;
            this.Style = newStyle;
            this.StyleManager = metroStyleManager;

            ApplyThemeToControls(this.Controls);
            this.Refresh();
        }


        // CLICK EVENTS 
        private void metroSettingsButton_Click(object sender, EventArgs e)
        {
            ShowPanel(metroSettingsPanel);
            LoadSettingsPanel();
        }
        private void metroHomeButton_Click(object sender, EventArgs e)
        {
            ShowPanel(metroHomePanel);
        }
        private void metroDataButton_Click(object sender, EventArgs e)
        {
            ShowPanel(metroDataPanel);
        }
        private void addArduinoButton_Click(object sender, EventArgs e)
        {
            AddArduinoConnectionTab();
            SaveArduinoTabs();
        }

        
        //UPDATING ONSCREEN DATA
        private void CheckFalconStatus()
        {
            if (falcon.isFalconRunning())
            {
                if (falcon.GetFlightData() != null)
                {
                    // Implementation of light bit checks and UI updates omitted for brevity
                    metroProcessLabel.Text = falcon.GetFalconProcessName();
                    metroVersionLabel.Text = falcon.GetFalconVersion();
                    metroStatusLabel.Text = falcon.GetFlyingState();
                    UpdateCheckBoxes();
                    UpdateScreens();
                }

            }
            else
            {
                // Handle Falcon not running state
            }
        }
        private void UpdateScreens()
        {
            metroDEDLabel_1.Text = falcon.GetFlightData().DEDLines[0];
            metroDEDLabel_2.Text = falcon.GetFlightData().DEDLines[1];
            metroDEDLabel_3.Text = falcon.GetFlightData().DEDLines[2];
            metroDEDLabel_4.Text = falcon.GetFlightData().DEDLines[3];
            metroDEDLabel_5.Text = falcon.GetFlightData().DEDLines[4];
        }
        private void UpdateCheckBoxes()
        {

            metroCheckBox1.Checked = falcon.IsLightOn(LightBits.MasterCaution);
            metroCheckBox2.Checked = falcon.IsLightOn(LightBits.TF);
            metroCheckBox3.Checked = falcon.IsLightOn(LightBits.OXY_BROW);
            metroCheckBox4.Checked = falcon.IsLightOn(LightBits.EQUIP_HOT);
            metroCheckBox5.Checked = falcon.IsLightOn(LightBits.ENG_FIRE);
            metroCheckBox6.Checked = falcon.IsLightOn(LightBits.CONFIG);
            metroCheckBox7.Checked = falcon.IsLightOn(LightBits.HYD);
            metroCheckBox8.Checked = falcon.IsLightOn(LightBits.Flcs_ABCD);
            metroCheckBox9.Checked = falcon.IsLightOn(LightBits.FLCS);
            metroCheckBox10.Checked = falcon.IsLightOn(LightBits.CAN);
            metroCheckBox11.Checked = falcon.IsLightOn(LightBits.T_L_CFG);
            metroCheckBox12.Checked = falcon.IsLightOn(LightBits.AOAAbove);
            metroCheckBox13.Checked = falcon.IsLightOn(LightBits.AOAOn);
            metroCheckBox14.Checked = falcon.IsLightOn(LightBits.AOABelow);
            metroCheckBox15.Checked = falcon.IsLightOn(LightBits.RefuelRDY);
            metroCheckBox16.Checked = falcon.IsLightOn(LightBits.RefuelAR);
            metroCheckBox17.Checked = falcon.IsLightOn(LightBits.RefuelDSC);
            metroCheckBox18.Checked = falcon.IsLightOn(LightBits.FltControlSys);
            metroCheckBox19.Checked = falcon.IsLightOn(LightBits.LEFlaps);
            metroCheckBox20.Checked = falcon.IsLightOn(LightBits.EngineFault);
            metroCheckBox21.Checked = falcon.IsLightOn(LightBits.Overheat);
            metroCheckBox22.Checked = falcon.IsLightOn(LightBits.FuelLow);
            metroCheckBox23.Checked = falcon.IsLightOn(LightBits.Avionics);
            metroCheckBox24.Checked = falcon.IsLightOn(LightBits.RadarAlt);
            metroCheckBox25.Checked = falcon.IsLightOn(LightBits.IFF);
            metroCheckBox26.Checked = falcon.IsLightOn(LightBits.ECM);
            metroCheckBox27.Checked = falcon.IsLightOn(LightBits.Hook);
            metroCheckBox28.Checked = falcon.IsLightOn(LightBits.NWSFail);
            metroCheckBox29.Checked = falcon.IsLightOn(LightBits.CabinPress);

            metroCheckBox30.Checked = falcon.IsLightOn(LightBits2.HandOff);
            metroCheckBox31.Checked = falcon.IsLightOn(LightBits2.Launch);
            metroCheckBox32.Checked = falcon.IsLightOn(LightBits2.PriMode);
            metroCheckBox33.Checked = falcon.IsLightOn(LightBits2.Naval);
            metroCheckBox34.Checked = falcon.IsLightOn(LightBits2.Unk);
            metroCheckBox35.Checked = falcon.IsLightOn(LightBits2.TgtSep);
            metroCheckBox36.Checked = falcon.IsLightOn(LightBits2.Go);
            metroCheckBox37.Checked = falcon.IsLightOn(LightBits2.NoGo);
            metroCheckBox38.Checked = falcon.IsLightOn(LightBits2.Degr);
            metroCheckBox39.Checked = falcon.IsLightOn(LightBits2.Rdy);
            metroCheckBox40.Checked = falcon.IsLightOn(LightBits2.ChaffLo);
            metroCheckBox41.Checked = falcon.IsLightOn(LightBits2.FlareLo);
            metroCheckBox42.Checked = falcon.IsLightOn(LightBits2.AuxSrch);
            metroCheckBox43.Checked = falcon.IsLightOn(LightBits2.AuxAct);
            metroCheckBox44.Checked = falcon.IsLightOn(LightBits2.AuxLow);
            metroCheckBox45.Checked = falcon.IsLightOn(LightBits2.AuxPwr);
            metroCheckBox46.Checked = falcon.IsLightOn(LightBits2.EcmPwr);
            metroCheckBox47.Checked = falcon.IsLightOn(LightBits2.EcmFail);
            metroCheckBox48.Checked = falcon.IsLightOn(LightBits2.FwdFuelLow);
            metroCheckBox49.Checked = falcon.IsLightOn(LightBits2.AftFuelLow);
            metroCheckBox50.Checked = falcon.IsLightOn(LightBits2.EPUOn);
            metroCheckBox51.Checked = falcon.IsLightOn(LightBits2.JFSOn);
            metroCheckBox52.Checked = falcon.IsLightOn(LightBits2.SEC);
            metroCheckBox53.Checked = falcon.IsLightOn(LightBits2.OXY_LOW);
            metroCheckBox54.Checked = falcon.IsLightOn(LightBits2.PROBEHEAT);
            metroCheckBox55.Checked = falcon.IsLightOn(LightBits2.SEAT_ARM);
            metroCheckBox56.Checked = falcon.IsLightOn(LightBits2.BUC);
            metroCheckBox57.Checked = falcon.IsLightOn(LightBits2.FUEL_OIL_HOT);
            metroCheckBox58.Checked = falcon.IsLightOn(LightBits2.ANTI_SKID);
            metroCheckBox59.Checked = falcon.IsLightOn(LightBits2.TFR_ENGAGED);
            metroCheckBox60.Checked = falcon.IsLightOn(LightBits2.GEARHANDLE);
            metroCheckBox61.Checked = falcon.IsLightOn(LightBits2.ENGINE);

            //LightBits3
            metroCheckBox62.Checked = falcon.IsLightOn(LightBits3.FlcsPmg);
            metroCheckBox63.Checked = falcon.IsLightOn(LightBits3.MainGen);
            metroCheckBox64.Checked = falcon.IsLightOn(LightBits3.StbyGen);
            metroCheckBox65.Checked = falcon.IsLightOn(LightBits3.EpuGen);
            metroCheckBox66.Checked = falcon.IsLightOn(LightBits3.EpuPmg);
            metroCheckBox67.Checked = falcon.IsLightOn(LightBits3.ToFlcs);
            metroCheckBox68.Checked = falcon.IsLightOn(LightBits3.FlcsRly);
            metroCheckBox69.Checked = falcon.IsLightOn(LightBits3.BatFail);

            metroCheckBox70.Checked = falcon.IsLightOn(LightBits3.Hydrazine);
            metroCheckBox71.Checked = falcon.IsLightOn(LightBits3.Air);

            metroCheckBox72.Checked = falcon.IsLightOn(LightBits3.Elec_Fault);
            metroCheckBox73.Checked = falcon.IsLightOn(LightBits3.Lef_Fault);

            metroCheckBox74.Checked = falcon.IsLightOn(LightBits3.OnGround);
            metroCheckBox75.Checked = falcon.IsLightOn(LightBits3.FlcsBitRun);
            metroCheckBox76.Checked = falcon.IsLightOn(LightBits3.FlcsBitFail);
            metroCheckBox77.Checked = falcon.IsLightOn(LightBits3.DbuWarn);
            metroCheckBox78.Checked = falcon.IsLightOn(LightBits3.NoseGearDown);
            metroCheckBox79.Checked = falcon.IsLightOn(LightBits3.LeftGearDown);
            metroCheckBox80.Checked = falcon.IsLightOn(LightBits3.RightGearDown);
            metroCheckBox81.Checked = falcon.IsLightOn(LightBits3.ParkBrakeOn);
            metroCheckBox82.Checked = falcon.IsLightOn(LightBits3.Power_Off);

            metroCheckBox83.Checked = falcon.IsLightOn(LightBits3.cadc);
            metroCheckBox84.Checked = falcon.IsLightOn(LightBits3.SpeedBrake);

            metroCheckBox85.Checked = falcon.IsLightOn(LightBits3.SysTest);
            metroCheckBox86.Checked = falcon.IsLightOn(LightBits3.MCAnnounced);

            metroCheckBox87.Checked = falcon.IsLightOn(LightBits3.MLGWOW);
            metroCheckBox88.Checked = falcon.IsLightOn(LightBits3.NLGWOW);

            metroCheckBox89.Checked = falcon.IsLightOn(LightBits3.ATF_Not_Engaged);
            metroCheckBox90.Checked = falcon.IsLightOn(LightBits3.Inlet_Icing);


            metro_uhf_preset_label.Text = "UHF Preset: " + falcon.GetFlightData().BupUhfPreset.ToString();
            if (falcon.GetFlightData().BupUhfFreq.ToString().Length > 1)
            {
                metro_uhf_freq_label.Text = "UHF Freq: " + falcon.GetFlightData().BupUhfFreq.ToString().Substring(0, 3) + "." + falcon.GetFlightData().BupUhfFreq.ToString().Substring(3, 3);
            }
            iffmode_label.Text = "IFF Mode: " + falcon.GetFlightData().iffBackupMode1Digit1 + " " + falcon.GetFlightData().iffBackupMode1Digit2 + " " + falcon.GetFlightData().iffBackupMode3ADigit1 + " " + falcon.GetFlightData().iffBackupMode3ADigit2;
        }
    }
}
