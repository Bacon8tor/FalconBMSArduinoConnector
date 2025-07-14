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
            this.sendlightbits_check = new System.Windows.Forms.CheckBox();
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
            this.falconRunning.Location = new System.Drawing.Point(6, 27);
            this.falconRunning.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.falconRunning.Name = "falconRunning";
            this.falconRunning.Size = new System.Drawing.Size(256, 29);
            this.falconRunning.TabIndex = 0;
            this.falconRunning.Text = "Falcon is Not Running";
            this.falconRunning.UseVisualStyleBackColor = true;
            // 
            // masterCaution_check
            // 
            this.masterCaution_check.AutoSize = true;
            this.masterCaution_check.Location = new System.Drawing.Point(6, 41);
            this.masterCaution_check.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.masterCaution_check.Name = "masterCaution_check";
            this.masterCaution_check.Size = new System.Drawing.Size(184, 29);
            this.masterCaution_check.TabIndex = 1;
            this.masterCaution_check.Text = "MasterCaution";
            this.masterCaution_check.UseVisualStyleBackColor = true;
            // 
            // seatArmed_check
            // 
            this.seatArmed_check.AutoSize = true;
            this.seatArmed_check.Location = new System.Drawing.Point(6, 41);
            this.seatArmed_check.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.seatArmed_check.Name = "seatArmed_check";
            this.seatArmed_check.Size = new System.Drawing.Size(156, 29);
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
            this.panel1.Location = new System.Drawing.Point(56, 41);
            this.panel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(182, 133);
            this.panel1.TabIndex = 3;
            // 
            // gearLightRight_check
            // 
            this.gearLightRight_check.AutoSize = true;
            this.gearLightRight_check.Location = new System.Drawing.Point(118, 84);
            this.gearLightRight_check.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gearLightRight_check.Name = "gearLightRight_check";
            this.gearLightRight_check.Size = new System.Drawing.Size(28, 27);
            this.gearLightRight_check.TabIndex = 3;
            this.gearLightRight_check.UseVisualStyleBackColor = true;
            // 
            // gearLightLeft_check
            // 
            this.gearLightLeft_check.AutoSize = true;
            this.gearLightLeft_check.Location = new System.Drawing.Point(38, 84);
            this.gearLightLeft_check.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gearLightLeft_check.Name = "gearLightLeft_check";
            this.gearLightLeft_check.Size = new System.Drawing.Size(28, 27);
            this.gearLightLeft_check.TabIndex = 2;
            this.gearLightLeft_check.UseVisualStyleBackColor = true;
            // 
            // gearLightFront_check
            // 
            this.gearLightFront_check.AutoSize = true;
            this.gearLightFront_check.Location = new System.Drawing.Point(80, 39);
            this.gearLightFront_check.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gearLightFront_check.Name = "gearLightFront_check";
            this.gearLightFront_check.Size = new System.Drawing.Size(28, 27);
            this.gearLightFront_check.TabIndex = 1;
            this.gearLightFront_check.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 25);
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
            this.panel2.Location = new System.Drawing.Point(862, 50);
            this.panel2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(400, 798);
            this.panel2.TabIndex = 4;
            // 
            // sendlightbits_check
            // 
            this.sendlightbits_check.AutoSize = true;
            this.sendlightbits_check.Enabled = false;
            this.sendlightbits_check.Location = new System.Drawing.Point(31, 327);
            this.sendlightbits_check.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sendlightbits_check.Name = "sendlightbits_check";
            this.sendlightbits_check.Size = new System.Drawing.Size(168, 29);
            this.sendlightbits_check.TabIndex = 9;
            this.sendlightbits_check.Text = "Sendlightbits";
            this.sendlightbits_check.UseVisualStyleBackColor = true;
            this.sendlightbits_check.CheckedChanged += new System.EventHandler(this.sendLight_CheckedChange);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(84, 497);
            this.button2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 44);
            this.button2.TabIndex = 8;
            this.button2.Text = "SendPacket LightBits";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(84, 425);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 44);
            this.button1.TabIndex = 7;
            this.button1.Text = "Send DED Line 1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // sendTest_button1
            // 
            this.sendTest_button1.Enabled = false;
            this.sendTest_button1.Location = new System.Drawing.Point(58, 367);
            this.sendTest_button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.sendTest_button1.Name = "sendTest_button1";
            this.sendTest_button1.Size = new System.Drawing.Size(150, 44);
            this.sendTest_button1.TabIndex = 6;
            this.sendTest_button1.Text = "Send Test";
            this.sendTest_button1.UseVisualStyleBackColor = true;
            this.sendTest_button1.Visible = false;
            this.sendTest_button1.Click += new System.EventHandler(this.sendTest1);
            // 
            // flyState_label
            // 
            this.flyState_label.AutoSize = true;
            this.flyState_label.Location = new System.Drawing.Point(52, 111);
            this.flyState_label.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.flyState_label.Name = "flyState_label";
            this.flyState_label.Size = new System.Drawing.Size(0, 25);
            this.flyState_label.TabIndex = 5;
            // 
            // serialConnect_button
            // 
            this.serialConnect_button.Location = new System.Drawing.Point(26, 250);
            this.serialConnect_button.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.serialConnect_button.Name = "serialConnect_button";
            this.serialConnect_button.Size = new System.Drawing.Size(150, 44);
            this.serialConnect_button.TabIndex = 4;
            this.serialConnect_button.Text = "connect";
            this.serialConnect_button.UseVisualStyleBackColor = true;
            this.serialConnect_button.Click += new System.EventHandler(this.connectToSerial);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 159);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(201, 25);
            this.label5.TabIndex = 3;
            this.label5.Text = "Arduino Connection";
            // 
            // serialPort_combo
            // 
            this.serialPort_combo.FormattingEnabled = true;
            this.serialPort_combo.Location = new System.Drawing.Point(26, 197);
            this.serialPort_combo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.serialPort_combo.Name = "serialPort_combo";
            this.serialPort_combo.Size = new System.Drawing.Size(238, 33);
            this.serialPort_combo.TabIndex = 2;
            this.serialPort_combo.DropDown += new System.EventHandler(this.update_comports);
            // 
            // falconBuild_text
            // 
            this.falconBuild_text.AutoSize = true;
            this.falconBuild_text.Location = new System.Drawing.Point(46, 66);
            this.falconBuild_text.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.falconBuild_text.Name = "falconBuild_text";
            this.falconBuild_text.Size = new System.Drawing.Size(29, 25);
            this.falconBuild_text.TabIndex = 1;
            this.falconBuild_text.Text = "v.";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.tf_check);
            this.panel3.Controls.Add(this.masterCaution_check);
            this.panel3.Location = new System.Drawing.Point(24, 50);
            this.panel3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(240, 491);
            this.panel3.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "LightBits";
            // 
            // tf_check
            // 
            this.tf_check.AutoSize = true;
            this.tf_check.Location = new System.Drawing.Point(6, 84);
            this.tf_check.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tf_check.Name = "tf_check";
            this.tf_check.Size = new System.Drawing.Size(70, 29);
            this.tf_check.TabIndex = 2;
            this.tf_check.Text = "TF";
            this.tf_check.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.seatArmed_check);
            this.panel4.Location = new System.Drawing.Point(276, 50);
            this.panel4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(268, 491);
            this.panel4.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(74, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "LightBits2";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.panel1);
            this.panel5.Location = new System.Drawing.Point(556, 50);
            this.panel5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(294, 491);
            this.panel5.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(104, 8);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 25);
            this.label4.TabIndex = 0;
            this.label4.Text = "LightBits3";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1278, 866);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
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

