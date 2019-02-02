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
    public partial class findFunctionForm : Form
    {
        public mainForm callingForm;

        private char[] charArray;

        private string RESULTS_NOT_FOUND_ERROR = "No results found...";

        /*
         * Initialzes the Form.
         */
        public findFunctionForm(Form callingForm)
        {
            InitializeComponent();
            this.callingForm = callingForm as mainForm;
        }

        /*
         * Executes initial startup commands.
         */
        private void findFunctionForm_Load(object sender, EventArgs e)
        {
            findFormRadioButtonUp.Checked = true;
        }

        /*
         * Determines whether or not a piece of source text contains
         * the text given in the second argument.
         */
        private bool containsText(string source, string text)
        {
            return (source.Contains(findFormTextBox.Text)) ? true : false;
        }

        /*
         * Determines if the form contains the text specified in the {findFunctionForm}
         * and based on the results of that query, selects the discovered text.
         */
        private void findFormFindButton_Click(object sender, EventArgs e)
        {
            if (!containsText(callingForm.mainEditor.Text, findFormTextBox.Text))
            {
                MessageBox.Show(RESULTS_NOT_FOUND_ERROR);
                return;
            }
            charArray = findFormTextBox.Text.ToCharArray();
            callingForm.mainEditor.SelectionStart = callingForm.mainEditor.Text.IndexOf(charArray[0]);
            callingForm.mainEditor.SelectionLength = charArray.Length;
            callingForm.Focus();
        }
    }
}
