using Chess.App.Tests;
using Chess.IO;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Chess.App
{
    internal partial class ParticipantsForm : Form, IDialogForm<Participant>
    {
        public new Participant DialogResult { get; set; }

        public ParticipantsForm()
        {
            InitializeComponent();
        }


        private void ParticipantsForm_Load(object sender, EventArgs e)
        {
            this.LoadParticipants();
        }

        private void btnNewParticipant_Click(object sender, EventArgs e)
        {
            var participant = this.OpenDialog<NewParticipantForm, Participant>();
            if (participant != null)
                Participant.Insert(participant);
            this.LoadParticipants();
        }

        private void LoadParticipants()
        {
            this.lstParticipants.DataSource = Participant.Load();
            this.lstParticipants.DisplayMember = "Name";
            this.lstParticipants.SelectedIndex = -1;
        }

        private void btnSelectParticipant_Click(object sender, EventArgs e)
        {
            this.DialogResult = (Participant)this.lstParticipants.SelectedItem;
            this.Close();
        }

        private void lstParticipants_DoubleClick(object sender, EventArgs e)
        {
            if (this.lstParticipants.SelectedItem == null)
                return;

            this.DialogResult = (Participant)this.lstParticipants.SelectedItem;
            this.Close();
        }
    }
}