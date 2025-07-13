namespace FalconBMSArduinoConnector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.falconRunning = new System.Windows.Forms.CheckBox();
            this.masterCaution_check = new System.Windows.Forms.CheckBox();
            this.seatArmed_check = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gearLightRight_check = new System.Windows.Forms.CheckBox();
            this.gearLightLeft_check = new System.Windows.Forms.CheckBox();
            this.gearLightFront_check = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.sendTest_button1 = new System.Windows.Forms.Button();
            this.flyState_label = new System.Windows.Forms.Label();
            this.serialConnect_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.serialPort_combo = new System.Windows.Forms.ComboBox();
            this.falconBuild_text = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.tf_check = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.sendlightbits_check = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // falconRunning
            // 
            this.falconRunning.AutoSize = true;
            this.falconRunning.Location = new System.Drawing.Point(4, 17);
            this.falconRunning.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.falconRunning.Name = "falconRunning";
            this.falconRunning.Size = new System.Drawing.Size(159, 20);
            this.falconRunning.TabIndex = 0;
            this.falconRunning.Text = "Falcon is Not Running";
            this.falconRunning.UseVisualStyleBackColor = true;
            // 
            // masterCaution_check
            // 
            this.masterCaution_check.AutoSize = true;
            this.masterCaution_check.Location = new System.Drawing.Point(4, 26);
            this.masterCaution_check.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.masterCaution_check.Name = "masterCaution_check";
            this.masterCaution_check.Size = new System.Drawing.Size(115, 20);
            this.masterCaution_check.TabIndex = 1;
            this.masterCaution_check.Text = "MasterCaution";
            this.masterCaution_check.UseVisualStyleBackColor = true;
            // 
            // seatArmed_check
            // 
            this.seatArmed_check.AutoSize = true;
            this.seatArmed_check.Location = new System.Drawing.Point(4, 26);
            this.seatArmed_check.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.seatArmed_check.Name = "seatArmed_check";
            this.seatArmed_check.Size = new System.Drawing.Size(100, 20);
            this.seatArmed_check.TabIndex = 2;
            this.seatArmed_check.Text = "Seat Armed";
            this.seatArmed_check.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gearLightRight_check);
            this.panel1.Controls.Add(this.gearLightLeft_check);
            this.panel1.Controls.Add(this.gearLightFront_check);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(37, 26);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(121, 85);
            this.panel1.TabIndex = 3;
            // 
            // gearLightRight_check
            // 
            this.gearLightRight_check.AutoSize = true;
            this.gearLightRight_check.Location = new System.Drawing.Point(79, 54);
            this.gearLightRight_check.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gearLightRight_check.Name = "gearLightRight_check";
            this.gearLightRight_check.Size = new System.Drawing.Size(18, 17);
            this.gearLightRight_check.TabIndex = 3;
            this.gearLightRight_check.UseVisualStyleBackColor = true;
            // 
            // gearLightLeft_check
            // 
            this.gearLightLeft_check.AutoSize = true;
            this.gearLightLeft_check.Location = new System.Drawing.Point(25, 54);
            this.gearLightLeft_check.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gearLightLeft_check.Name = "gearLightLeft_check";
            this.gearLightLeft_check.Size = new System.Drawing.Size(18, 17);
            this.gearLightLeft_check.TabIndex = 2;
            this.gearLightLeft_check.UseVisualStyleBackColor = true;
            // 
            // gearLightFront_check
            // 
            this.gearLightFront_check.AutoSize = true;
            this.gearLightFront_check.Location = new System.Drawing.Point(53, 25);
            this.gearLightFront_check.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gearLightFront_check.Name = "gearLightFront_check";
            this.gearLightFront_check.Size = new System.Drawing.Size(18, 17);
            this.gearLightFront_check.TabIndex = 1;
            this.gearLightFront_check.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gear";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.sendlightbits_check);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.sendTest_button1);
            this.panel2.Controls.Add(this.flyState_label);
            this.panel2.Controls.Add(this.serialConnect_button);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.serialPort_combo);
            this.panel2.Controls.Add(this.falconBuild_text);
            this.panel2.Controls.Add(this.falconRunning);
            this.panel2.Location = new System.Drawing.Point(784, 15);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(267, 511);
            this.panel2.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(56, 318);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 28);
            this.button2.TabIndex = 8;
            this.button2.Text = "SendPacket LightBits";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(56, 272);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 7;
            this.button1.Text = "Send DED Line 1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // sendTest_button1
            // 
            this.sendTest_button1.Enabled = false;
            this.sendTest_button1.Location = new System.Drawing.Point(39, 235);
            this.sendTest_button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sendTest_button1.Name = "sendTest_button1";
            this.sendTest_button1.Size = new System.Drawing.Size(100, 28);
            this.sendTest_button1.TabIndex = 6;
            this.sendTest_button1.Text = "Send Test";
            this.sendTest_button1.UseVisualStyleBackColor = true;
            this.sendTest_button1.Click += new System.EventHandler(this.sendTest1);
            // 
            // flyState_label
            // 
            this.flyState_label.AutoSize = true;
            this.flyState_label.Location = new System.Drawing.Point(35, 71);
            this.flyState_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.flyState_label.Name = "flyState_label";
            this.flyState_label.Size = new System.Drawing.Size(0, 16);
            this.flyState_label.TabIndex = 5;
            // 
            // serialConnect_button
            // 
            this.serialConnect_button.Location = new System.Drawing.Point(17, 160);
            this.serialConnect_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.serialConnect_button.Name = "serialConnect_button";
            this.serialConnect_button.Size = new System.Drawing.Size(100, 28);
            this.serialConnect_button.TabIndex = 4;
            this.serialConnect_button.Text = "connect";
            this.serialConnect_button.UseVisualStyleBackColor = true;
            this.serialConnect_button.Click += new System.EventHandler(this.connectToSerial);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 102);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 16);
            this.label5.TabIndex = 3;
            this.label5.Text = "Arduino Connection";
            // 
            // serialPort_combo
            // 
            this.serialPort_combo.FormattingEnabled = true;
            this.serialPort_combo.Location = new System.Drawing.Point(17, 126);
            this.serialPort_combo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.serialPort_combo.Name = "serialPort_combo";
            this.serialPort_combo.Size = new System.Drawing.Size(160, 24);
            this.serialPort_combo.TabIndex = 2;
            this.serialPort_combo.DropDown += new System.EventHandler(this.update_comports);
            // 
            // falconBuild_text
            // 
            this.falconBuild_text.AutoSize = true;
            this.falconBuild_text.Location = new System.Drawing.Point(31, 42);
            this.falconBuild_text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.falconBuild_text.Name = "falconBuild_text";
            this.falconBuild_text.Size = new System.Drawing.Size(17, 16);
            this.falconBuild_text.TabIndex = 1;
            this.falconBuild_text.Text = "v.";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.tf_check);
            this.panel3.Controls.Add(this.masterCaution_check);
            this.panel3.Location = new System.Drawing.Point(16, 32);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(160, 314);
            this.panel3.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "LightBits";
            // 
            // tf_check
            // 
            this.tf_check.AutoSize = true;
            this.tf_check.Location = new System.Drawing.Point(4, 54);
            this.tf_check.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tf_check.Name = "tf_check";
            this.tf_check.Size = new System.Drawing.Size(46, 20);
            this.tf_check.TabIndex = 2;
            this.tf_check.Text = "TF";
            this.tf_check.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.seatArmed_check);
            this.panel4.Location = new System.Drawing.Point(184, 32);
            this.panel4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(179, 314);
            this.panel4.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "LightBits2";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.panel1);
            this.panel5.Location = new System.Drawing.Point(371, 32);
            this.panel5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(196, 314);
            this.panel5.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 5);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "LightBits3";
            // 
            // sendlightbits_check
            // 
            this.sendlightbits_check.AutoSize = true;
            this.sendlightbits_check.Location = new System.Drawing.Point(38, 365);
            this.sendlightbits_check.Name = "sendlightbits_check";
            this.sendlightbits_check.Size = new System.Drawing.Size(106, 20);
            this.sendlightbits_check.TabIndex = 9;
            this.sendlightbits_check.Text = "Sendlightbits";
            this.sendlightbits_check.UseVisualStyleBackColor = true;
            this.sendlightbits_check.CheckedChanged += new System.EventHandler(this.sendLight_CheckedChange);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox falconRunning;
        private System.Windows.Forms.CheckBox masterCaution_check;
        private System.Windows.Forms.CheckBox seatArmed_check;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox gearLightRight_check;
        private System.Windows.Forms.CheckBox gearLightLeft_check;
        private System.Windows.Forms.CheckBox gearLightFront_check;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label falconBuild_text;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox tf_check;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button serialConnect_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox serialPort_combo;
        private System.Windows.Forms.Label flyState_label;
        private System.Windows.Forms.Button sendTest_button1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox sendlightbits_check;
    }
}

