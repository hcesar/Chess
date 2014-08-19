namespace Chess.App
{
    partial class NewGameForm
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
            this.btnNewGame = new System.Windows.Forms.Button();
            this.comboWhite = new System.Windows.Forms.ComboBox();
            this.comboBlack = new System.Windows.Forms.ComboBox();
            this.lbWhite = new System.Windows.Forms.Label();
            this.lbBlack = new System.Windows.Forms.Label();
            this.comboConfigBlack = new System.Windows.Forms.ComboBox();
            this.comboConfigWhite = new System.Windows.Forms.ComboBox();
            this.lbConfigBlack = new System.Windows.Forms.Label();
            this.lbConfigWhite = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnNewGame
            // 
            this.btnNewGame.Location = new System.Drawing.Point(302, 174);
            this.btnNewGame.Name = "btnNewGame";
            this.btnNewGame.Size = new System.Drawing.Size(75, 23);
            this.btnNewGame.TabIndex = 0;
            this.btnNewGame.Text = "New Game";
            this.btnNewGame.UseVisualStyleBackColor = true;
            this.btnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
            // 
            // comboWhite
            // 
            this.comboWhite.FormattingEnabled = true;
            this.comboWhite.Items.AddRange(new object[] {
            "Human",
            "Computer",
            "Connect to Server"});
            this.comboWhite.Location = new System.Drawing.Point(23, 58);
            this.comboWhite.Name = "comboWhite";
            this.comboWhite.Size = new System.Drawing.Size(121, 21);
            this.comboWhite.TabIndex = 1;
            this.comboWhite.SelectedIndexChanged += new System.EventHandler(this.comboWhite_SelectedIndexChanged);
            // 
            // comboBlack
            // 
            this.comboBlack.FormattingEnabled = true;
            this.comboBlack.Items.AddRange(new object[] {
            "Human",
            "Computer",
            "Wait for connection"});
            this.comboBlack.Location = new System.Drawing.Point(256, 58);
            this.comboBlack.Name = "comboBlack";
            this.comboBlack.Size = new System.Drawing.Size(121, 21);
            this.comboBlack.TabIndex = 2;
            this.comboBlack.SelectedIndexChanged += new System.EventHandler(this.comboBlack_SelectedIndexChanged);
            // 
            // lbWhite
            // 
            this.lbWhite.AutoSize = true;
            this.lbWhite.Location = new System.Drawing.Point(20, 42);
            this.lbWhite.Name = "lbWhite";
            this.lbWhite.Size = new System.Drawing.Size(67, 13);
            this.lbWhite.TabIndex = 3;
            this.lbWhite.Text = "White Player";
            // 
            // lbBlack
            // 
            this.lbBlack.AutoSize = true;
            this.lbBlack.Location = new System.Drawing.Point(253, 42);
            this.lbBlack.Name = "lbBlack";
            this.lbBlack.Size = new System.Drawing.Size(66, 13);
            this.lbBlack.TabIndex = 4;
            this.lbBlack.Text = "Black Player";
            // 
            // comboConfigBlack
            // 
            this.comboConfigBlack.FormattingEnabled = true;
            this.comboConfigBlack.Location = new System.Drawing.Point(256, 110);
            this.comboConfigBlack.Name = "comboConfigBlack";
            this.comboConfigBlack.Size = new System.Drawing.Size(121, 21);
            this.comboConfigBlack.TabIndex = 5;
            this.comboConfigBlack.Visible = false;
            // 
            // comboConfigWhite
            // 
            this.comboConfigWhite.FormattingEnabled = true;
            this.comboConfigWhite.Location = new System.Drawing.Point(23, 110);
            this.comboConfigWhite.Name = "comboConfigWhite";
            this.comboConfigWhite.Size = new System.Drawing.Size(121, 21);
            this.comboConfigWhite.TabIndex = 7;
            this.comboConfigWhite.Visible = false;
            // 
            // lbConfigBlack
            // 
            this.lbConfigBlack.AutoSize = true;
            this.lbConfigBlack.Location = new System.Drawing.Point(253, 94);
            this.lbConfigBlack.Name = "lbConfigBlack";
            this.lbConfigBlack.Size = new System.Drawing.Size(72, 13);
            this.lbConfigBlack.TabIndex = 9;
            this.lbConfigBlack.Text = "lbConfigBlack";
            this.lbConfigBlack.Visible = false;
            // 
            // lbConfigWhite
            // 
            this.lbConfigWhite.AutoSize = true;
            this.lbConfigWhite.Location = new System.Drawing.Point(20, 94);
            this.lbConfigWhite.Name = "lbConfigWhite";
            this.lbConfigWhite.Size = new System.Drawing.Size(73, 13);
            this.lbConfigWhite.TabIndex = 8;
            this.lbConfigWhite.Text = "lbConfigWhite";
            this.lbConfigWhite.Visible = false;
            // 
            // NewGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 239);
            this.Controls.Add(this.lbConfigBlack);
            this.Controls.Add(this.lbConfigWhite);
            this.Controls.Add(this.comboConfigWhite);
            this.Controls.Add(this.comboConfigBlack);
            this.Controls.Add(this.lbBlack);
            this.Controls.Add(this.lbWhite);
            this.Controls.Add(this.comboBlack);
            this.Controls.Add(this.comboWhite);
            this.Controls.Add(this.btnNewGame);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewGameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Game";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNewGame;
        private System.Windows.Forms.ComboBox comboWhite;
        private System.Windows.Forms.ComboBox comboBlack;
        private System.Windows.Forms.Label lbWhite;
        private System.Windows.Forms.Label lbBlack;
        private System.Windows.Forms.ComboBox comboConfigBlack;
        private System.Windows.Forms.ComboBox comboConfigWhite;
        private System.Windows.Forms.Label lbConfigBlack;
        private System.Windows.Forms.Label lbConfigWhite;
    }
}