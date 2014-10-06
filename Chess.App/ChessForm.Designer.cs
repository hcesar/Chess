namespace Chess.App
{
    partial class ChessForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromNetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbSideInstructions = new System.Windows.Forms.Label();
            this.boardControl = new Chess.App.BoardControl();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boardControl)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1486, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.newGameToolStripMenuItem.Text = "New Game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromFileToolStripMenuItem,
            this.fromNetworkToolStripMenuItem});
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.loadToolStripMenuItem.Text = "Open";
            // 
            // fromFileToolStripMenuItem
            // 
            this.fromFileToolStripMenuItem.Name = "fromFileToolStripMenuItem";
            this.fromFileToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.fromFileToolStripMenuItem.Text = "From File";
            this.fromFileToolStripMenuItem.Click += new System.EventHandler(this.fromFileToolStripMenuItem_Click);
            // 
            // fromNetworkToolStripMenuItem
            // 
            this.fromNetworkToolStripMenuItem.Name = "fromNetworkToolStripMenuItem";
            this.fromNetworkToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.fromNetworkToolStripMenuItem.Text = "From Network";
            this.fromNetworkToolStripMenuItem.Click += new System.EventHandler(this.fromNetworkToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // testsToolStripMenuItem
            // 
            this.testsToolStripMenuItem.Name = "testsToolStripMenuItem";
            this.testsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.testsToolStripMenuItem.Text = "Tests";
            // 
            // lbSideInstructions
            // 
            this.lbSideInstructions.AutoSize = true;
            this.lbSideInstructions.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbSideInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSideInstructions.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbSideInstructions.Location = new System.Drawing.Point(1274, 52);
            this.lbSideInstructions.MaximumSize = new System.Drawing.Size(200, 0);
            this.lbSideInstructions.MinimumSize = new System.Drawing.Size(200, 400);
            this.lbSideInstructions.Name = "lbSideInstructions";
            this.lbSideInstructions.Size = new System.Drawing.Size(200, 400);
            this.lbSideInstructions.TabIndex = 2;
            // 
            // boardControl
            // 
            this.boardControl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.boardControl.Location = new System.Drawing.Point(331, 24);
            this.boardControl.Margin = new System.Windows.Forms.Padding(0);
            this.boardControl.Name = "boardControl";
            this.boardControl.Size = new System.Drawing.Size(800, 800);
            this.boardControl.TabIndex = 1;
            this.boardControl.TabStop = false;
            this.boardControl.Visible = false;
            // 
            // ChessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1486, 848);
            this.Controls.Add(this.lbSideInstructions);
            this.Controls.Add(this.boardControl);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ChessForm";
            this.Text = "Chess";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boardControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private BoardControl boardControl;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromNetworkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testsToolStripMenuItem;
        public System.Windows.Forms.Label lbSideInstructions;
    }
}

