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
            p.Profession = txtName.Text;
            p.Age = int.Parse(txtAge.Text);
            p.Gender = rbMale.Checked ? Gender.Male : Gender.Female;

            this.DialogResult = p;
            this.Close();
        }

      
    }
}
