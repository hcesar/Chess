namespace Chess.App
{
    partial class NewParticipantForm
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.rbFemale = new System.Windows.Forms.RadioButton();
            this.rbMale = new System.Windows.Forms.RadioButton();
            this.btnRegister = new System.Windows.Forms.Button();
            this.rbLeft = new System.Windows.Forms.RadioButton();
            this.rbRight = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.cbHighestEd = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.rbAdvanced = new System.Windows.Forms.RadioButton();
            this.rbBasic = new System.Windows.Forms.RadioButton();
            this.rbIntermediate = new System.Windows.Forms.RadioButton();
            this.cbArea = new System.Windows.Forms.ComboBox();
            this.lblOther = new System.Windows.Forms.Label();
            this.txtOther = new System.Windows.Forms.TextBox();
            this.lbELO = new System.Windows.Forms.Label();
            this.txtELO = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(24, 43);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(232, 20);
            this.txtName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Area";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(268, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Age";
            // 
            // txtAge
            // 
            this.txtAge.Location = new System.Drawing.Point(269, 43);
            this.txtAge.Name = "txtAge";
            this.txtAge.Size = new System.Drawing.Size(70, 20);
            this.txtAge.TabIndex = 1;
            // 
            // rbFemale
            // 
            this.rbFemale.AutoSize = true;
            this.rbFemale.Location = new System.Drawing.Point(88, 18);
            this.rbFemale.Name = "rbFemale";
            this.rbFemale.Size = new System.Drawing.Size(59, 17);
            this.rbFemale.TabIndex = 1;
            this.rbFemale.TabStop = true;
            this.rbFemale.Text = "Female";
            this.rbFemale.UseVisualStyleBackColor = true;
            // 
            // rbMale
            // 
            this.rbMale.AutoSize = true;
            this.rbMale.Location = new System.Drawing.Point(5, 17);
            this.rbMale.Name = "rbMale";
            this.rbMale.Size = new System.Drawing.Size(48, 17);
            this.rbMale.TabIndex = 0;
            this.rbMale.TabStop = true;
            this.rbMale.Text = "Male";
            this.rbMale.UseVisualStyleBackColor = true;
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(264, 342);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(75, 23);
            this.btnRegister.TabIndex = 8;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // rbLeft
            // 
            this.rbLeft.AutoSize = true;
            this.rbLeft.Location = new System.Drawing.Point(5, 18);
            this.rbLeft.Name = "rbLeft";
            this.rbLeft.Size = new System.Drawing.Size(43, 17);
            this.rbLeft.TabIndex = 0;
            this.rbLeft.TabStop = true;
            this.rbLeft.Text = "Left";
            this.rbLeft.UseVisualStyleBackColor = true;
            // 
            // rbRight
            // 
            this.rbRight.AutoSize = true;
            this.rbRight.Location = new System.Drawing.Point(88, 18);
            this.rbRight.Name = "rbRight";
            this.rbRight.Size = new System.Drawing.Size(50, 17);
            this.rbRight.TabIndex = 1;
            this.rbRight.TabStop = true;
            this.rbRight.Text = "Right";
            this.rbRight.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Highest Education";
            // 
            // cbHighestEd
            // 
            this.cbHighestEd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHighestEd.FormattingEnabled = true;
            this.cbHighestEd.Items.AddRange(new object[] {
            "Ensino Fundamental Incompleto",
            "Ensino Fundamental Completo",
            "Ensino Medio Incompleto",
            "Ensino Medio Completo",
            "Ensino Medio Tecnico",
            "Ensino Superior Incompleto",
            "Ensino Superior Completo",
            "Pos-Graduacao",
            "Mestrado",
            "Doutorado",
            "Pos-Doutorado"});
            this.cbHighestEd.Location = new System.Drawing.Point(24, 85);
            this.cbHighestEd.Margin = new System.Windows.Forms.Padding(2);
            this.cbHighestEd.Name = "cbHighestEd";
            this.cbHighestEd.Size = new System.Drawing.Size(232, 21);
            this.cbHighestEd.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbMale);
            this.groupBox1.Controls.Add(this.rbFemale);
            this.groupBox1.Location = new System.Drawing.Point(24, 201);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(150, 37);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Gender";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbLeft);
            this.groupBox2.Controls.Add(this.rbRight);
            this.groupBox2.Location = new System.Drawing.Point(189, 201);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(150, 37);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Laterality";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblDescription);
            this.groupBox3.Controls.Add(this.rbAdvanced);
            this.groupBox3.Controls.Add(this.rbBasic);
            this.groupBox3.Controls.Add(this.rbIntermediate);
            this.groupBox3.Location = new System.Drawing.Point(24, 251);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(315, 86);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Chess Level";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 45);
            this.lblDescription.MaximumSize = new System.Drawing.Size(304, 32);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(194, 13);
            this.lblDescription.TabIndex = 9;
            this.lblDescription.Text = "Please select one of the options above.";
            // 
            // rbAdvanced
            // 
            this.rbAdvanced.AutoSize = true;
            this.rbAdvanced.Location = new System.Drawing.Point(228, 18);
            this.rbAdvanced.Name = "rbAdvanced";
            this.rbAdvanced.Size = new System.Drawing.Size(82, 17);
            this.rbAdvanced.TabIndex = 2;
            this.rbAdvanced.TabStop = true;
            this.rbAdvanced.Text = "Professional";
            this.rbAdvanced.UseVisualStyleBackColor = true;
            this.rbAdvanced.CheckedChanged += new System.EventHandler(this.rbChessLevel_CheckedChanged);
            // 
            // rbBasic
            // 
            this.rbBasic.AutoSize = true;
            this.rbBasic.Location = new System.Drawing.Point(5, 18);
            this.rbBasic.Name = "rbBasic";
            this.rbBasic.Size = new System.Drawing.Size(67, 17);
            this.rbBasic.TabIndex = 0;
            this.rbBasic.TabStop = true;
            this.rbBasic.Text = "Beginner";
            this.rbBasic.UseVisualStyleBackColor = true;
            this.rbBasic.CheckedChanged += new System.EventHandler(this.rbChessLevel_CheckedChanged);
            // 
            // rbIntermediate
            // 
            this.rbIntermediate.AutoSize = true;
            this.rbIntermediate.Location = new System.Drawing.Point(108, 18);
            this.rbIntermediate.Name = "rbIntermediate";
            this.rbIntermediate.Size = new System.Drawing.Size(84, 17);
            this.rbIntermediate.TabIndex = 1;
            this.rbIntermediate.TabStop = true;
            this.rbIntermediate.Text = "Experienced";
            this.rbIntermediate.UseVisualStyleBackColor = true;
            this.rbIntermediate.CheckedChanged += new System.EventHandler(this.rbChessLevel_CheckedChanged);
            // 
            // cbArea
            // 
            this.cbArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArea.FormattingEnabled = true;
            this.cbArea.Items.AddRange(new object[] {
            "Administração",
            "Ciência da Computação",
            "Engenharia de Automação e Controle",
            "Engenharia Civil",
            "Engenharia Elétrica",
            "Engenharia de Materiais",
            "Engenharia Mecânica",
            "Engenharia de Produção",
            "Engenharia Química",
            "Engenharia Têxtil",
            "Outro"});
            this.cbArea.Location = new System.Drawing.Point(24, 126);
            this.cbArea.Name = "cbArea";
            this.cbArea.Size = new System.Drawing.Size(315, 21);
            this.cbArea.TabIndex = 3;
            this.cbArea.SelectedValueChanged += new System.EventHandler(this.cbArea_SelectedValueChanged);
            // 
            // lblOther
            // 
            this.lblOther.AutoSize = true;
            this.lblOther.Location = new System.Drawing.Point(21, 152);
            this.lblOther.Name = "lblOther";
            this.lblOther.Size = new System.Drawing.Size(33, 13);
            this.lblOther.TabIndex = 21;
            this.lblOther.Text = "Other";
            this.lblOther.Visible = false;
            // 
            // txtOther
            // 
            this.txtOther.Location = new System.Drawing.Point(24, 168);
            this.txtOther.Name = "txtOther";
            this.txtOther.Size = new System.Drawing.Size(315, 20);
            this.txtOther.TabIndex = 4;
            this.txtOther.Visible = false;
            // 
            // lbELO
            // 
            this.lbELO.AutoSize = true;
            this.lbELO.Location = new System.Drawing.Point(270, 70);
            this.lbELO.Name = "lbELO";
            this.lbELO.Size = new System.Drawing.Size(28, 13);
            this.lbELO.TabIndex = 23;
            this.lbELO.Text = "ELO";
            // 
            // txtELO
            // 
            this.txtELO.Location = new System.Drawing.Point(271, 85);
            this.txtELO.Name = "txtELO";
            this.txtELO.Size = new System.Drawing.Size(70, 20);
            this.txtELO.TabIndex = 22;
            // 
            // NewParticipantForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 382);
            this.Controls.Add(this.lbELO);
            this.Controls.Add(this.txtELO);
            this.Controls.Add(this.lblOther);
            this.Controls.Add(this.txtOther);
            this.Controls.Add(this.cbArea);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cbHighestEd);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAge);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewParticipantForm";
            this.Text = "New Participant";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAge;
        private System.Windows.Forms.RadioButton rbFemale;
        private System.Windows.Forms.RadioButton rbMale;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.RadioButton rbLeft;
        private System.Windows.Forms.RadioButton rbRight;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbHighestEd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbAdvanced;
        private System.Windows.Forms.RadioButton rbBasic;
        private System.Windows.Forms.RadioButton rbIntermediate;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ComboBox cbArea;
        private System.Windows.Forms.Label lblOther;
        private System.Windows.Forms.TextBox txtOther;
        private System.Windows.Forms.Label lbELO;
        private System.Windows.Forms.TextBox txtELO;
    }
}