using Chess.App.Tests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.App
{
    public partial class NewParticipantForm : Form, IDialogForm<Participant>
    {
        public new Participant DialogResult { get; set; }

        public NewParticipantForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtAge.Text) || !(rbMale.Checked | rbFemale.Checked))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            var p = new Participant();
            p.Name = txtName.Text;
            p.HighestEducation = (String)cbHighestEd.SelectedItem;
            p.Area = txtOther.Visible ? txtOther.Text : (String)cbArea.SelectedItem;
            p.Age = int.Parse(txtAge.Text);
            p.Gender = rbMale.Checked ? Gender.Male : Gender.Female;
            p.Laterality = rbLeft.Checked ? Laterality.Left : Laterality.Right;
            p.ELO = txtELO.Text;

            if (rbAdvanced.Checked)
                p.ChessLevel = ChessLevel.Professional;
            else if (rbIntermediate.Checked)
                p.ChessLevel = ChessLevel.Experienced;
            else
                p.ChessLevel = ChessLevel.Beginner;

            this.DialogResult = p;
            this.Close();
        }

        private void cbArea_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((String)cbArea.SelectedItem).Equals("Outro"))
            {
                lblOther.Visible = true;
                txtOther.Visible = true;
            }
            else
            {
                lblOther.Visible = false;
                txtOther.Visible = false;
            }
        }

        private void rbChessLevel_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbChecked = sender as RadioButton;

            if (rbChecked.Checked)
            {
                if (rbChecked.Text.Equals("Beginner"))
                    lblDescription.Text = "I know the rules and have played few times.";
                else if (rbChecked.Text.Equals("Experienced"))
                    lblDescription.Text = "I have played several times.";
                else
                    lblDescription.Text = "I play / have played professionally.";
            }
        }
      
    }
}
