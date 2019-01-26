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
    public partial class mainForm : Form
    {
        // Declaring and Initializing useful variables
        OpenFileDialog file_open = new OpenFileDialog();

        public bool UNDO_AVAIALBE = false;
        public bool FIND_FORM_CLOSED = true;
        public bool IS_FILE_DIRTY = false;
        public bool NEW_FILE = true;
        public String EMPTY_STRING = "";

        private String FILE_NAME;
        private RichTextBoxStreamType FILE_TYPE;
        private findFunctionForm findFunctionForm;

        /*
         * Initialzes the Form.
         */
        public mainForm()
        {
            InitializeComponent();
        }

        /*
         * Opens an open file dialogue upon clicking the "Open" Option
         * available in the "Edit" drop-down menu.
         */
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NEW_FILE = false;

            file_open.Title = "Open File";
            file_open.Filter = "Rich Text File (*.rtf)|*.rtf| Plain Text File (*.txt)|*.txt";
            file_open.FilterIndex = 1;
            file_open.Title = "Open text or RTF file";

            FILE_TYPE = RichTextBoxStreamType.RichText;
            if (DialogResult.OK == file_open.ShowDialog())
            {
                if (string.IsNullOrEmpty(file_open.FileName))
                    return;
                if (file_open.FilterIndex == 2)
                {
                    FILE_TYPE = RichTextBoxStreamType.PlainText;
                }
                FILE_NAME = file_open.FileName;
                richTextBox1.LoadFile(file_open.FileName, FILE_TYPE);
            }
        }

        /*
         * Creates a new blank file file while effectively erradicating any unsaved
         * changes to the previously open document.
         */
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!richTextBox1.Text.Equals(EMPTY_STRING))
            {
                MessageBox.Show("These changes will be overridden!");
            }
        }

        /*
         * Opens a save file dialogue upon clicking the "Save" Option
         * available in the "Edit" drop-down menu.
         */
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NEW_FILE)
            {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }
            
            richTextBox1.SaveFile(FILE_NAME, FILE_TYPE);
            MessageBox.Show("File Saved");
        }

        /*
         * Opens a save as file dialogue upon clicking the "Save As" Option
         * available in the "Edit" drop-down menu.
         */
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            FILE_NAME = saveDlg.FileName;

            // To filter files from SaveFileDialog
            saveDlg.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            saveDlg.DefaultExt = "*.rtf";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Save the contents";

            DialogResult retval = saveDlg.ShowDialog();

            if (retval != DialogResult.OK)
            {
                return;
            }

            if (saveDlg.FilterIndex == 2)
            {
                FILE_TYPE = RichTextBoxStreamType.PlainText;
            }
            else
            {
                FILE_TYPE = RichTextBoxStreamType.RichText;
            }

            richTextBox1.SaveFile(FILE_NAME, FILE_TYPE);
            MessageBox.Show("File Saved");
        }

        /*
         * Closes the Application
         */
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /*
         * Undoes the last change to the currently open file.
         */
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
            redoToolStripMenuItem.Enabled = true;
        }

        /*
         * This method is used to determine if the currently open file is dirty.
         * If the file is dirty then the {#RichTextBox.Undo()} function is performed.
         */
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            IS_FILE_DIRTY = true;
            undoToolStripMenuItem.Enabled = true;
            if (!richTextBox1.Text.Equals(EMPTY_STRING))
            {
                findToolStripMenuItem.Enabled = true;
            }
            else
            {
                findToolStripMenuItem.Enabled = false;
            }
        }

        /*
         * Creates a new instance of {findFunctionForm} if {FIND_FORM_CLOSED}
         * or the local variable {findFunctionForm} is null. Thus allowing the user
         * to search for certain words within the file that is currently open.
         */
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FIND_FORM_CLOSED || findFunctionForm == null)
            {
                findFunctionForm = new findFunctionForm(this);
                findFunctionForm.Visible = true;
            }
        }
    }
}
