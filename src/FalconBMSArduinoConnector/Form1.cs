using F4SharedMem;
using F4SharedMem.Headers;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

namespace FalconBMSArduinoConnector
{
    public partial class FalconBMSArduinoConnector : MetroForm
    {
        FalconConnector falcon = new FalconConnector();
        List<ArduinoConnector> arduinoConnections = new List<ArduinoConnector>();
        private Timer falconCheckTimer;
        private MetroStyleManager metroStyleManager;

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
            LoadSettings();

            CheckFalconStatus();
            LoadArduinoTabs();

            falconCheckTimer = new Timer();
            falconCheckTimer.Interval = 50;
            falconCheckTimer.Tick += (s, args) => CheckFalconStatus();
            falconCheckTimer.Start();
             metroDataPanel.Dock = DockStyle.Fill;

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


        private void CheckFalconStatus()
        {
            if (falcon.isFalconRunning())
            {
                // Implementation of light bit checks and UI updates omitted for brevity
                metroProcessLabel.Text = falcon.GetFalconProcessName();
                metroVersionLabel.Text = falcon.GetFalconVersion();
            }
            else
            {
                // Handle Falcon not running state
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


            var comboBox = new MetroComboBox() { Left = 5, Top = 15, Width = 121 ,Theme = this.Theme };
            var button = new MetroButton() { Text = "Connect", Left = 150, Top = 20, Theme = this.Theme };
            var removeButton = new MetroButton() { Text = "Remove", Left = 5, Top = 200, Theme = this.Theme  };
            var DTRcheckbox = new MetroCheckBox() { Width = 220, Left = 130, Top = 55, Text = "Micro/ProMicro/Leonardo_Device" , Theme = this.Theme };

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

            var renameButton = new MetroButton() { Text = "Rename", Left = 100, Top = 200, Theme = this.Theme };
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
            SaveSettings(); // Save theme/style on exit
        }
        
        private void ShowPanel(MetroPanel panelToShow)
        {
            metroHomePanel.Visible = false;
            metroSettingsPanel.Visible = false;
            metroDataPanel.Visible = false;

            panelToShow.Visible = true;
        }
        
        private void metroSettingsButton_Click(object sender, EventArgs e)
        {
           ShowPanel(metroSettingsPanel);
            LoadSettingsPanel();
        }

        private void metroHomeButton_Click(object sender, EventArgs e)
        {
            ShowPanel(metroHomePanel);
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

        private void metroDataButton_Click(object sender, EventArgs e)
        {
            ShowPanel(metroDataPanel);
        }
    }
}
