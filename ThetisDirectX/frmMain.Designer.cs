namespace ThetisDirectX
{
    partial class frmMain
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
            this.picDisplay = new System.Windows.Forms.PictureBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.udLinesPerFrame = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.udFPS = new System.Windows.Forms.NumericUpDown();
            this.chkRun = new System.Windows.Forms.CheckBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.chkAA = new System.Windows.Forms.CheckBox();
            this.chkVSync = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.udLineWidth = new System.Windows.Forms.NumericUpDown();
            this.cmbAdaptors = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLinesPerFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLineWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // picDisplay
            // 
            this.picDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picDisplay.BackColor = System.Drawing.Color.Black;
            this.picDisplay.Cursor = System.Windows.Forms.Cursors.Cross;
            this.picDisplay.Location = new System.Drawing.Point(12, 12);
            this.picDisplay.Name = "picDisplay";
            this.picDisplay.Size = new System.Drawing.Size(800, 600);
            this.picDisplay.TabIndex = 0;
            this.picDisplay.TabStop = false;
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(829, 12);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.Size = new System.Drawing.Size(169, 323);
            this.txtResults.TabIndex = 3;
            // 
            // radioButton1
            // 
            this.radioButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(829, 524);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(88, 17);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "250,000 lines";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(829, 547);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(88, 17);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "500,000 lines";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(829, 570);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(97, 17);
            this.radioButton3.TabIndex = 6;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "1,000,000 lines";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(829, 593);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(97, 17);
            this.radioButton4.TabIndex = 7;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "2,000,000 lines";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // udLinesPerFrame
            // 
            this.udLinesPerFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udLinesPerFrame.Location = new System.Drawing.Point(830, 402);
            this.udLinesPerFrame.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.udLinesPerFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udLinesPerFrame.Name = "udLinesPerFrame";
            this.udLinesPerFrame.Size = new System.Drawing.Size(75, 20);
            this.udLinesPerFrame.TabIndex = 8;
            this.udLinesPerFrame.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udLinesPerFrame.ValueChanged += new System.EventHandler(this.udLinesPerFrame_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(911, 404);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "lines per frame";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(911, 430);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "frames per second";
            // 
            // udFPS
            // 
            this.udFPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udFPS.Location = new System.Drawing.Point(851, 428);
            this.udFPS.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udFPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udFPS.Name = "udFPS";
            this.udFPS.Size = new System.Drawing.Size(54, 20);
            this.udFPS.TabIndex = 10;
            this.udFPS.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.udFPS.ValueChanged += new System.EventHandler(this.udFPS_ValueChanged);
            // 
            // chkRun
            // 
            this.chkRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkRun.AutoSize = true;
            this.chkRun.Location = new System.Drawing.Point(952, 593);
            this.chkRun.Name = "chkRun";
            this.chkRun.Size = new System.Drawing.Size(46, 17);
            this.chkRun.TabIndex = 12;
            this.chkRun.Text = "Run";
            this.chkRun.UseVisualStyleBackColor = true;
            this.chkRun.CheckedChanged += new System.EventHandler(this.chkRun_CheckedChanged);
            // 
            // lblResolution
            // 
            this.lblResolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResolution.AutoSize = true;
            this.lblResolution.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResolution.Location = new System.Drawing.Point(827, 498);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(78, 16);
            this.lblResolution.TabIndex = 13;
            this.lblResolution.Text = "3000x4000";
            // 
            // chkAA
            // 
            this.chkAA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAA.AutoSize = true;
            this.chkAA.Location = new System.Drawing.Point(932, 520);
            this.chkAA.Name = "chkAA";
            this.chkAA.Size = new System.Drawing.Size(66, 17);
            this.chkAA.TabIndex = 15;
            this.chkAA.Text = "AntiAlias";
            this.chkAA.UseVisualStyleBackColor = true;
            this.chkAA.CheckedChanged += new System.EventHandler(this.chkAA_CheckedChanged);
            // 
            // chkVSync
            // 
            this.chkVSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkVSync.AutoSize = true;
            this.chkVSync.Location = new System.Drawing.Point(932, 497);
            this.chkVSync.Name = "chkVSync";
            this.chkVSync.Size = new System.Drawing.Size(69, 17);
            this.chkVSync.TabIndex = 16;
            this.chkVSync.Text = "VSync(1)";
            this.chkVSync.UseVisualStyleBackColor = true;
            this.chkVSync.CheckedChanged += new System.EventHandler(this.chkVSync_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(911, 456);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "line width";
            // 
            // udLineWidth
            // 
            this.udLineWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.udLineWidth.DecimalPlaces = 1;
            this.udLineWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udLineWidth.Location = new System.Drawing.Point(851, 454);
            this.udLineWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.udLineWidth.Name = "udLineWidth";
            this.udLineWidth.Size = new System.Drawing.Size(54, 20);
            this.udLineWidth.TabIndex = 17;
            this.udLineWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udLineWidth.ValueChanged += new System.EventHandler(this.udLineWidth_ValueChanged);
            // 
            // cmbAdaptors
            // 
            this.cmbAdaptors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAdaptors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdaptors.FormattingEnabled = true;
            this.cmbAdaptors.Location = new System.Drawing.Point(829, 364);
            this.cmbAdaptors.Name = "cmbAdaptors";
            this.cmbAdaptors.Size = new System.Drawing.Size(168, 21);
            this.cmbAdaptors.TabIndex = 19;
            this.cmbAdaptors.SelectedIndexChanged += new System.EventHandler(this.cmbAdaptors_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(826, 348);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Adapter:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 629);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbAdaptors);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.udLineWidth);
            this.Controls.Add(this.chkVSync);
            this.Controls.Add(this.chkAA);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.chkRun);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.udFPS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udLinesPerFrame);
            this.Controls.Add(this.radioButton4);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.picDisplay);
            this.MinimumSize = new System.Drawing.Size(1030, 668);
            this.Name = "frmMain";
            this.Text = "Thetis DirectX Profiler - MW0LGE 2022";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLinesPerFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLineWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picDisplay;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.NumericUpDown udLinesPerFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown udFPS;
        private System.Windows.Forms.CheckBox chkRun;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.CheckBox chkAA;
        private System.Windows.Forms.CheckBox chkVSync;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown udLineWidth;
        private System.Windows.Forms.ComboBox cmbAdaptors;
        private System.Windows.Forms.Label label4;
    }
}

