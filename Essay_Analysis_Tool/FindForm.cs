using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Essay_Analysis_Tool
{
    public partial class FindDialog : Form
    {
        private readonly mainForm callingForm;

        public FindDialog(mainForm pMain)
        {
            InitializeComponent();
            callingForm = pMain;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void FindDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void controlTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateFindNextButton();
        }

        private void UpdateFindNextButton()
        {
            buttonFindNext.Enabled = callingForm.mainEditor.Text.Length > 0;
        }

        private void FindDialog_Load(object sender, EventArgs e)
        {
            UpdateFindNextButton();
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            var SearchText = findFormTextBox.Text;
            var MatchCase = checkBox1.Checked;
            var bSearchDown = findFormRadioButtonDown.Checked;

            if (!callingForm.FindAndSelect(SearchText, MatchCase, bSearchDown))
            {
                MessageBox.Show(this, "No Results Found...", "Warning");
            }
            callingForm.mainEditor.Focus();
        }

        public void Triggered()
        {
            callingForm.Focus();
        }

        private void controlTextBox_Enter(object sender, EventArgs e)
        {
            var Sender = (TextBox)sender;
            Sender.SelectAll();
        }

        public new void Show(IWin32Window window = null)
        {
            callingForm.Focus();
            callingForm.mainEditor.SelectAll();

            if (window == null)
            {
                base.Show();
            }
            else
            {
                base.Show(window);
            }
        }
    }
}
