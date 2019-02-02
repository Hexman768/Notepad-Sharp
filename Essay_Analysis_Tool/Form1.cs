using FastColoredTextBoxNS;
using ICSharpCode.TextEditor.Document;
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
        public bool SYNTAX_HIGHLIGHTING = false;
        public String EMPTY_STRING = "";

        private String FILE_NAME;
        private RichTextBoxStreamType FILE_TYPE;
        private findFunctionForm findFunctionForm;
        private Encoding currentFileEncoding;

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
            file_open.Filter = "All Files (*.*)|*.*|Rich Text File (*.rtf)|*.rtf|Text Documents (*.txt)|*.txt";
            file_open.FilterIndex = 1;
            file_open.Title = "Open text or RTF file";

            FILE_TYPE = RichTextBoxStreamType.RichText;
            if (DialogResult.OK == file_open.ShowDialog())
            {
                if (string.IsNullOrEmpty(file_open.FileName))
                    return;
                FILE_NAME = file_open.FileName;
                currentFileEncoding = EncodingDetector.DetectTextFileEncoding(FILE_NAME);
                mainEditor.OpenFile(FILE_NAME);
            }
        }

        /*
         * Creates a new blank file file while effectively erradicating any unsaved
         * changes to the previously open document.
         */
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mainEditor.Text.Equals(EMPTY_STRING))
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
                doSaveAs();
                return;
            }

            if (doSave())
            {
                MessageBox.Show("File Saved");
            }
        }

        /*
         * Calls the doSaveAs() function.
         */
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (doSaveAs())
            {
                MessageBox.Show("File Saved");
            }
        }

        /*
         * Opens a save as file dialogue upon clicking the "Save As" Option
         * available in the "Edit" drop-down menu.
         * 
         * @returns
         *    1.) True if the save operation completes successfully.
         *    2.) False if the save operation fails in which case a messageBox
         *        containing the error is displayed.
         */
        public bool doSaveAs()
        {
            SaveFileDialog saveDlg = new SaveFileDialog();

            // To filter files from SaveFileDialog
            saveDlg.Filter = "All Files (*.*)|*.*|Rich Text File (*.rtf)|*.rtf|Text Documents (*.txt)|*.txt";
            saveDlg.DefaultExt = "*.rtf";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Save the contents";

            DialogResult retval = saveDlg.ShowDialog();
            FILE_NAME = saveDlg.FileName;
            currentFileEncoding = EncodingDetector.DetectTextFileEncoding(FILE_NAME); // FILE_NAME is null

            if (retval != DialogResult.OK)
            {
                return false;
            }
            
            try
            {
                mainEditor.SaveToFile(FILE_NAME, currentFileEncoding);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().Name);
                return false;
            }
        }

        /*
         * Attempts to save the current file upon clicking the "Save" Option
         * available in the "Edit" drop-down menu.
         * 
         * @returns
         *    1.) True if the save operation completes successfully.
         *    2.) False if the save operation fails in which case a messageBox
         *        containing the error is displayed.
         */
        private bool doSave()
        {
            try
            {
                mainEditor.SaveToFile(FILE_NAME, currentFileEncoding);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().Name);
                return false;
            }
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
            mainEditor.Undo();
            redoToolStripMenuItem.Enabled = true;
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
        
        /*
         * This method is used to determine if the currently open file is dirty.
         * If the file is dirty then the {#textEditorControl1.Undo()} function is performed.
         */
        private void mainEditor_TextChanging(object sender, TextChangingEventArgs e)
        {
            IS_FILE_DIRTY = true;
            undoToolStripMenuItem.Enabled = true;
            findToolStripMenuItem.Enabled = !mainEditor.Text.Equals(EMPTY_STRING) ? true : false;
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.CSharp;
        }

        private void sQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.SQL;
        }

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.HTML;
        }

        private void cToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            mainEditor.Language = Language.CSharp;
        }

        private void sQLToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            mainEditor.Language = Language.SQL;
        }

        private void hTMLToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            mainEditor.Language = Language.HTML;
        }

        private void javaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.JS;
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.XML;
        }

        private void pHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.PHP;
        }

        private void luaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.Lua;
        }

        private void visualBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.VB;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.Custom;
        }
    }
}
