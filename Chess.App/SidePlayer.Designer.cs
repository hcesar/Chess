namespace Chess.App
{
    partial class SidePlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SidePlayer));
            this.btnNewTest = new System.Windows.Forms.Button();
            this.btnCalibration = new System.Windows.Forms.Button();
            this.boardControl = new Chess.App.BoardControl();
            ((System.ComponentModel.ISupportInitialize)(this.boardControl)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNewTest
            // 
            this.btnNewTest.Location = new System.Drawing.Point(20, 21);
            this.btnNewTest.Name = "btnNewTest";
            this.btnNewTest.Size = new System.Drawing.Size(298, 30);
            this.btnNewTest.TabIndex = 2;
            this.btnNewTest.Text = "New Test";
            this.btnNewTest.UseVisualStyleBackColor = true;
            this.btnNewTest.Click += new System.EventHandler(this.btnNewTest_Click);
            // 
            // btnCalibration
            // 
            this.btnCalibration.Location = new System.Drawing.Point(522, 21);
            this.btnCalibration.Name = "btnCalibration";
            this.btnCalibration.Size = new System.Drawing.Size(298, 30);
            this.btnCalibration.TabIndex = 3;
            this.btnCalibration.Text = "EyeTracking Calibration";
            this.btnCalibration.UseVisualStyleBackColor = true;
            this.btnCalibration.Click += new System.EventHandler(this.btnCalibration_Click);
            // 
            // boardControl
            // 
            this.boardControl.Image = ((System.Drawing.Image)(resources.GetObject("boardControl.Image")));
            this.boardControl.Location = new System.Drawing.Point(20, 76);
            this.boardControl.Margin = new System.Windows.Forms.Padding(0);
            this.boardControl.Name = "boardControl";
            this.boardControl.Size = new System.Drawing.Size(800, 800);
            this.boardControl.TabIndex = 1;
            this.boardControl.TabStop = false;
            this.boardControl.Visible = false;
            // 
            // SidePlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 885);
            this.Controls.Add(this.btnCalibration);
            this.Controls.Add(this.btnNewTest);
            this.Controls.Add(this.boardControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SidePlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.boardControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BoardControl boardControl;
        private System.Windows.Forms.Button btnNewTest;
        private System.Windows.Forms.Button btnCalibration;
    }
}