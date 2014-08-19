using Chess.IO;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Chess.App
{
    internal partial class NewGameForm : Form
    {
        public NewGameForm()
        {
            InitializeComponent();
            this.comboWhite.SelectedIndex = 0;
            this.comboBlack.SelectedIndex = 0;
            this.comboConfigBlack.Items.AddRange(Enum.GetNames(typeof(AILevel)));
            this.comboConfigBlack.SelectedIndex = 1;
        }

        private void comboWhite_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboConfigWhite.Items.Clear();
            var playerType = GetPlayerType(comboWhite);

            switch (playerType)
            {
                case PlayerType.Human:
                    this.lbConfigWhite.Visible = this.comboConfigWhite.Visible = false;
                    break;

                case PlayerType.Computer:
                    this.lbConfigWhite.Text = "AI Level";
                    this.lbConfigWhite.Visible = this.comboConfigWhite.Visible = true;
                    this.comboConfigWhite.Items.AddRange(Enum.GetNames(typeof(AILevel)));
                    this.comboConfigWhite.SelectedIndex = 1;
                    break;

                case PlayerType.Network:
                    this.lbConfigWhite.Text = "Server";
                    this.lbConfigWhite.Visible = this.comboConfigWhite.Visible = true;
                    this.comboConfigWhite.Items.AddRange(ServerDiscovery.FindServers().Select(i => i.Address.ToString()).ToArray());
                    this.comboConfigWhite.SelectedIndex = -1;
                    this.comboConfigWhite.Text = null;
                    break;
            }
        }

        private void comboBlack_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboConfigBlack.Text = null;
            var playerType = GetPlayerType(comboBlack);
            if (this.lbConfigBlack.Visible = this.comboConfigBlack.Visible = playerType == PlayerType.Computer)
                this.comboConfigBlack.SelectedIndex = 1;

            this.lbConfigBlack.Text = "AI Level";
        }

        public static GameConfiguration Show(Form parent)
        {
            var dlg = new NewGameForm();
            if (dlg.ShowDialog(parent) == DialogResult.OK)
            {
                var ret = new GameConfiguration();

                ret.White = GetPlayer(GetPlayerType(dlg.comboWhite), dlg.comboConfigWhite.Text);
                ret.Black = GetPlayer(GetPlayerType(dlg.comboBlack), dlg.comboConfigBlack.Text);

                return ret;
            }

            return null;
        }

        private static Player GetPlayer(PlayerType playerType, string config)
        {
            switch (playerType)
            {
                case PlayerType.Human: return new HumanPlayer();
                case PlayerType.Computer: return new ComputerPlayer((AILevel)Enum.Parse(typeof(AILevel), config));
                case PlayerType.Network:
                    ChessConnection cnn;
                    if (string.IsNullOrEmpty(config))
                        cnn = new Server();
                    else
                        cnn = new Client(System.Net.IPAddress.Parse(config));

                    return new NetworkPlayer(cnn);
            }

            throw new NotSupportedException(playerType.ToString());
        }

        private static PlayerType GetPlayerType(ComboBox combo)
        {
            return (PlayerType)combo.SelectedIndex;
        }

        private enum PlayerType
        {
            Human = 0,
            Computer = 1,
            Network = 2
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}