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

        public findFunctionForm(Form callingForm)
        {
            InitializeComponent();
            this.callingForm = callingForm as mainForm;
        }

        private void findFunctionForm_Load(object sender, EventArgs e)
        {
            findFormRadioButtonUp.Checked = true;
        }

        private void findFormFindButton_Click(object sender, EventArgs e)
        {
            callingForm.richTextBox1.Find(findFormTextBox.Text);
        }
    }
}
