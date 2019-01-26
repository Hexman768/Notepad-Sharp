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
    public partial class Form1 : Form
    {
        // Declaring and Initializing useful variables
        OpenFileDialog file_open = new OpenFileDialog();

        public bool UNDO_AVAIALBE = false;
        public bool IS_FILE_DIRTY = false;

        /*
         * Initialzes the Form.
         */
        public Form1()
        {
            InitializeComponent();
        }

        /*
         * Opens an open file dialogue upon clicking the "Open" Option
         * available in the "Edit" drop-down menu.
         */
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            file_open.Title = "Open File";
            file_open.Filter = "Rich Text File (*.rtf)|*.rtf| Plain Text File (*.txt)|*.txt";
            file_open.FilterIndex = 1;
            file_open.Title = "Open text or RTF file";

            RichTextBoxStreamType stream_type = RichTextBoxStreamType.RichText;
            if (DialogResult.OK == file_open.ShowDialog())
            {
                if (string.IsNullOrEmpty(file_open.FileName))
                    return;
                if (file_open.FilterIndex == 2)
                {
                    stream_type = RichTextBoxStreamType.PlainText;
                }
                richTextBox1.LoadFile(file_open.FileName, stream_type);
            }
        }

        /*
         * Creates a new blank file file while effectively erradicating any unsaved
         * changes to the previously open document.
         */
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
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
            SaveFileDialog saveDlg = new SaveFileDialog();
            string filename = "";

            // To filter files from SaveFileDialog
            saveDlg.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            saveDlg.DefaultExt = "*.rtf";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Save the contents";

            filename = file_open.FileName;

            RichTextBoxStreamType stream_type;
            // Checks the extension of the file to save
            if (filename.Contains(".txt"))
                stream_type = RichTextBoxStreamType.PlainText;
            else
                stream_type = RichTextBoxStreamType.RichText;

            richTextBox1.SaveFile(filename, stream_type);
        }

        /*
         * Opens a save as file dialogue upon clicking the "Save As" Option
         * available in the "Edit" drop-down menu.
         */
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            string filename = "";

            // To filter files from SaveFileDialog
            saveDlg.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            saveDlg.DefaultExt = "*.rtf";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Save the contents";

            DialogResult retval = saveDlg.ShowDialog();
            if (retval == DialogResult.OK)
                filename = saveDlg.FileName;
            else
                return;

            RichTextBoxStreamType stream_type;
            if (saveDlg.FilterIndex == 2)
                stream_type = RichTextBoxStreamType.PlainText;
            else
                stream_type = RichTextBoxStreamType.RichText;

            richTextBox1.SaveFile(filename, stream_type);
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
        }
    }
}
