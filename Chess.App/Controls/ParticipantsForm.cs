using Chess.IO;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Chess.App
{
    internal partial class ParticipantsForm : Form
    {
        public ParticipantsForm()
        {
            InitializeComponent();
            this.LoadParticipants();
        }


        private void ParticipantsForm_Load(object sender, EventArgs e)
        {

        }

        private void btnNewParticipant_Click(object sender, EventArgs e)
        {
            this.OpenDialog<NewParticipantForm>();
            this.LoadParticipants();
        }

        private void LoadParticipants()
        {
        }
    }
}