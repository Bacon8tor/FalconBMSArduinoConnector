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
            this.falconBuild_text = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tf_check = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // falconRunning
            // 
            this.falconRunning.AutoSize = true;
            this.falconRunning.Location = new System.Drawing.Point(3, 14);
            this.falconRunning.Name = "falconRunning";
            this.falconRunning.Size = new System.Drawing.Size(101, 17);
            this.falconRunning.TabIndex = 0;
            this.falconRunning.Text = "Falcon Running";
            this.falconRunning.UseVisualStyleBackColor = true;
            // 
            // masterCaution_check
            // 
            this.masterCaution_check.AutoSize = true;
            this.masterCaution_check.Location = new System.Drawing.Point(3, 3);
            this.masterCaution_check.Name = "masterCaution_check";
            this.masterCaution_check.Size = new System.Drawing.Size(94, 17);
            this.masterCaution_check.TabIndex = 1;
            this.masterCaution_check.Text = "MasterCaution";
            this.masterCaution_check.UseVisualStyleBackColor = true;
            // 
            // seatArmed_check
            // 
            this.seatArmed_check.AutoSize = true;
            this.seatArmed_check.Location = new System.Drawing.Point(278, 46);
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
            this.panel1.Location = new System.Drawing.Point(431, 26);
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
            this.label1.Location = new System.Drawing.Point(37, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gear";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.falconBuild_text);
            this.panel2.Controls.Add(this.falconRunning);
            this.panel2.Location = new System.Drawing.Point(588, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 94);
            this.panel2.TabIndex = 4;
            // 
            // falconBuild_text
            // 
            this.falconBuild_text.AutoSize = true;
            this.falconBuild_text.Location = new System.Drawing.Point(23, 34);
            this.falconBuild_text.Name = "falconBuild_text";
            this.falconBuild_text.Size = new System.Drawing.Size(33, 13);
            this.falconBuild_text.TabIndex = 1;
            this.falconBuild_text.Text = "Build:";
            this.falconBuild_text.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tf_check);
            this.panel3.Controls.Add(this.masterCaution_check);
            this.panel3.Location = new System.Drawing.Point(12, 26);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 100);
            this.panel3.TabIndex = 5;
            // 
            // tf_check
            // 
            this.tf_check.AutoSize = true;
            this.tf_check.Location = new System.Drawing.Point(3, 27);
            this.tf_check.Name = "tf_check";
            this.tf_check.Size = new System.Drawing.Size(39, 17);
            this.tf_check.TabIndex = 2;
            this.tf_check.Text = "TF";
            this.tf_check.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.seatArmed_check);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

