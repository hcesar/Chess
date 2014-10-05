namespace Chess.App
{
    partial class ParticipantsForm
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
            this.lstParticipants = new System.Windows.Forms.ListBox();
            this.btnSelectParticipant = new System.Windows.Forms.Button();
            this.btnNewParticipant = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstParticipants
            // 
            this.lstParticipants.FormattingEnabled = true;
            this.lstParticipants.Location = new System.Drawing.Point(32, 52);
            this.lstParticipants.Name = "lstParticipants";
            this.lstParticipants.Size = new System.Drawing.Size(573, 251);
            this.lstParticipants.TabIndex = 0;
            // 
            // btnSelectParticipant
            // 
            this.btnSelectParticipant.Location = new System.Drawing.Point(529, 337);
            this.btnSelectParticipant.Name = "btnSelectParticipant";
            this.btnSelectParticipant.Size = new System.Drawing.Size(75, 23);
            this.btnSelectParticipant.TabIndex = 1;
            this.btnSelectParticipant.Text = "Start Test";
            this.btnSelectParticipant.UseVisualStyleBackColor = true;
            // 
            // btnNewParticipant
            // 
            this.btnNewParticipant.Location = new System.Drawing.Point(429, 337);
            this.btnNewParticipant.Name = "btnNewParticipant";
            this.btnNewParticipant.Size = new System.Drawing.Size(75, 23);
            this.btnNewParticipant.TabIndex = 2;
            this.btnNewParticipant.Text = "New";
            this.btnNewParticipant.UseVisualStyleBackColor = true;
            this.btnNewParticipant.Click += new System.EventHandler(this.btnNewParticipant_Click);
            // 
            // ParticipantsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 416);
            this.Controls.Add(this.btnNewParticipant);
            this.Controls.Add(this.btnSelectParticipant);
            this.Controls.Add(this.lstParticipants);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ParticipantsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Participants";
            this.Load += new System.EventHandler(this.ParticipantsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstParticipants;
        private System.Windows.Forms.Button btnSelectParticipant;
        private System.Windows.Forms.Button btnNewParticipant;

    }
}