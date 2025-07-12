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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
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
            this.falconRunning.Location = new System.Drawing.Point(3, 14);
            this.falconRunning.Name = "falconRunning";
            this.falconRunning.Size = new System.Drawing.Size(131, 17);
            this.falconRunning.TabIndex = 0;
            this.falconRunning.Text = "Falcon is Not Running";
            this.falconRunning.UseVisualStyleBackColor = true;
            // 
            // masterCaution_check
            // 
            this.masterCaution_check.AutoSize = true;
            this.masterCaution_check.Location = new System.Drawing.Point(3, 21);
            this.masterCaution_check.Name = "masterCaution_check";
            this.masterCaution_check.Size = new System.Drawing.Size(94, 17);
            this.masterCaution_check.TabIndex = 1;
            this.masterCaution_check.Text = "MasterCaution";
            this.masterCaution_check.UseVisualStyleBackColor = true;
            // 
            // seatArmed_check
            // 
            this.seatArmed_check.AutoSize = true;
            this.seatArmed_check.Location = new System.Drawing.Point(3, 21);
            this.seatArmed_check.Name = "seatArmed_check";
            this.seatArmed_check.Size = new System.Drawing.Size(81, 17);
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
            this.panel1.Location = new System.Drawing.Point(28, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(91, 69);
            this.panel1.TabIndex = 3;
            // 
            // gearLightRight_check
            // 
            this.gearLightRight_check.AutoSize = true;
            this.gearLightRight_check.Location = new System.Drawing.Point(59, 44);
            this.gearLightRight_check.Name = "gearLightRight_check";
            this.gearLightRight_check.Size = new System.Drawing.Size(15, 14);
            this.gearLightRight_check.TabIndex = 3;
            this.gearLightRight_check.UseVisualStyleBackColor = true;
            // 
            // gearLightLeft_check
            // 
            this.gearLightLeft_check.AutoSize = true;
            this.gearLightLeft_check.Location = new System.Drawing.Point(19, 44);
            this.gearLightLeft_check.Name = "gearLightLeft_check";
            this.gearLightLeft_check.Size = new System.Drawing.Size(15, 14);
            this.gearLightLeft_check.TabIndex = 2;
            this.gearLightLeft_check.UseVisualStyleBackColor = true;
            // 
            // gearLightFront_check
            // 
            this.gearLightFront_check.AutoSize = true;
            this.gearLightFront_check.Location = new System.Drawing.Point(40, 20);
            this.gearLightFront_check.Name = "gearLightFront_check";
            this.gearLightFront_check.Size = new System.Drawing.Size(15, 14);
            this.gearLightFront_check.TabIndex = 1;
            this.gearLightFront_check.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gear";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.sendTest_button1);
            this.panel2.Controls.Add(this.flyState_label);
            this.panel2.Controls.Add(this.serialConnect_button);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.serialPort_combo);
            this.panel2.Controls.Add(this.falconBuild_text);
            this.panel2.Controls.Add(this.falconRunning);
            this.panel2.Location = new System.Drawing.Point(588, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 415);
            this.panel2.TabIndex = 4;
            // 
            // sendTest_button1
            // 
            this.sendTest_button1.Enabled = false;
            this.sendTest_button1.Location = new System.Drawing.Point(29, 191);
            this.sendTest_button1.Name = "sendTest_button1";
            this.sendTest_button1.Size = new System.Drawing.Size(75, 23);
            this.sendTest_button1.TabIndex = 6;
            this.sendTest_button1.Text = "Send Test";
            this.sendTest_button1.UseVisualStyleBackColor = true;
            this.sendTest_button1.Click += new System.EventHandler(this.sendTest1);
            // 
            // flyState_label
            // 
            this.flyState_label.AutoSize = true;
            this.flyState_label.Location = new System.Drawing.Point(26, 58);
            this.flyState_label.Name = "flyState_label";
            this.flyState_label.Size = new System.Drawing.Size(0, 13);
            this.flyState_label.TabIndex = 5;
            // 
            // serialConnect_button
            // 
            this.serialConnect_button.Location = new System.Drawing.Point(13, 130);
            this.serialConnect_button.Name = "serialConnect_button";
            this.serialConnect_button.Size = new System.Drawing.Size(75, 23);
            this.serialConnect_button.TabIndex = 4;
            this.serialConnect_button.Text = "connect";
            this.serialConnect_button.UseVisualStyleBackColor = true;
            this.serialConnect_button.Click += new System.EventHandler(this.connectToSerial);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Arduino Connection";
            // 
            // serialPort_combo
            // 
            this.serialPort_combo.FormattingEnabled = true;
            this.serialPort_combo.Location = new System.Drawing.Point(13, 102);
            this.serialPort_combo.Name = "serialPort_combo";
            this.serialPort_combo.Size = new System.Drawing.Size(121, 21);
            this.serialPort_combo.TabIndex = 2;
            this.serialPort_combo.DropDown += new System.EventHandler(this.update_comports);
            // 
            // falconBuild_text
            // 
            this.falconBuild_text.AutoSize = true;
            this.falconBuild_text.Location = new System.Drawing.Point(23, 34);
            this.falconBuild_text.Name = "falconBuild_text";
            this.falconBuild_text.Size = new System.Drawing.Size(16, 13);
            this.falconBuild_text.TabIndex = 1;
            this.falconBuild_text.Text = "v.";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.tf_check);
            this.panel3.Controls.Add(this.masterCaution_check);
            this.panel3.Location = new System.Drawing.Point(12, 26);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(120, 255);
            this.panel3.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "LightBits";
            // 
            // tf_check
            // 
            this.tf_check.AutoSize = true;
            this.tf_check.Location = new System.Drawing.Point(3, 44);
            this.tf_check.Name = "tf_check";
            this.tf_check.Size = new System.Drawing.Size(39, 17);
            this.tf_check.TabIndex = 2;
            this.tf_check.Text = "TF";
            this.tf_check.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.seatArmed_check);
            this.panel4.Location = new System.Drawing.Point(138, 26);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(134, 255);
            this.panel4.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "LightBits2";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.panel1);
            this.panel5.Location = new System.Drawing.Point(278, 26);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(147, 255);
            this.panel5.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "LightBits3";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(42, 221);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Send DED Line 1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(42, 273);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "SendPacket LightBits";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
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
    }
}

