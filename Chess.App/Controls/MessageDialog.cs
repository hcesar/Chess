using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chess.App
{
    public class MessageDialog : Control
    {
        private Label lbMessage;
        private Button btOk;
        private Timer timer;
        private Action closeAction;
        private Func<bool> closePredicate;

        public override string Text { get { return this.lbMessage.Text; } set { this.lbMessage.Text = value; } }
        public event EventHandler Closing;


        public MessageDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Visible = false;
            this.BackColor = Color.Silver;
            this.BringToFront();

            this.timer = new Timer();
            this.timer.Interval = 10;
            this.timer.Tick += timer_Tick;

            this.lbMessage = new Label();
            this.lbMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbMessage.BackColor = Color.Transparent;
            this.lbMessage.Font = new System.Drawing.Font("Trebuchet MS", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMessage.ForeColor = Color.Brown;
            this.lbMessage.Size = new Size(800, 200);
            this.lbMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbMessage.BorderStyle = BorderStyle.FixedSingle;

            this.btOk = new Button();
            this.btOk.Text = "OK";
            this.btOk.Location = new Point(710, 150);
            this.btOk.Size = new Size(80, 40);
            this.btOk.Click += btOk_Click;
            this.btOk.NotifyDefault(false);

            this.Controls.Add(this.btOk);
            this.Controls.Add(this.lbMessage);
            this.btOk.BringToFront();

            this.Anchor = System.Windows.Forms.AnchorStyles.Left | AnchorStyles.Top;
            this.BackColor = System.Drawing.Color.Wheat;
            this.Location = new System.Drawing.Point(0, 300);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "dlgMessage";
            this.Size = new System.Drawing.Size(800, 200);
            this.TabIndex = 1;
            this.TabStop = false;
            this.Visible = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!this.Visible || this.closePredicate == null || this.closePredicate())
                this.Close();
        }

        public void ShowMessage(string message, Func<bool> closePredicate = null, Action closeAction = null)
        {
            if (this.Visible)
                throw new InvalidOperationException("Dialog already open.");

            this.Text = message;
            this.closePredicate = closePredicate;
            this.closeAction = closeAction;

            this.Visible = true;
            this.btOk.Visible = closePredicate == null;

            if (this.closePredicate != null)
                this.timer.Enabled = true;
            
            this.btOk.Focus();
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Close()
        {
            this.timer.Enabled = false;
            this.closePredicate = null;

            this.Visible = false;

            if (this.closeAction != null)
                this.closeAction();

            this.closeAction = null;

            if (this.Closing != null)
                this.Closing(this, EventArgs.Empty);
        }
    }
}