using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DMExport
{
    public partial class FormPwaSelect : Form
    {
        public FormPwaSelect()
        {
            InitializeComponent();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            Visible = false;
            new FormMain(txtServerURL.Text).Show();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
