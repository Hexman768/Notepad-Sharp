using FastColoredTextBoxNS;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Essay_Analysis_Tool
{
    public partial class mainForm : Form
    {
        // Declaring and Initializing useful variables
        OpenFileDialog file_open = new OpenFileDialog();
        FontDialog fontDialog = new FontDialog();

        public bool UNDO_AVAIALBE = false;
        public bool FIND_FORM_CLOSED = true;
        public bool IS_FILE_DIRTY = false;
        public bool NEW_FILE = true;
        public bool SYNTAX_HIGHLIGHTING = false;
        public String EMPTY_STRING = "";

        private String FILE_NAME;
        private RichTextBoxStreamType FILE_TYPE;
        private FindDialog findFunctionForm;
        private Encoding currentFileEncoding;

        private const string _HTML_EXT = "html";
        private const string _XML_EXT = "xml";
        private const string _JAVASCRIPT_EXT = "js";
        private const string _CSHARP_EXT = "cs";
        private const string _LUA_EXT = "lua";
        private const string _SQL_EXT = "sql";
        
        /// <summary>
        /// Initialzes the Form.
        /// </summary>
        public mainForm()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Opens an open file dialogue upon clicking the "Open" Option
        /// available in the "Edit" drop-down menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NEW_FILE = false;
            SelectionStart = 0;

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
                setCurrentEditorSyntaxHighlight(FILE_NAME);
                currentFileEncoding = EncodingDetector.DetectTextFileEncoding(FILE_NAME);
                mainEditor.OpenFile(FILE_NAME);
            }
        }
        
        /// <summary>
        /// Creates a new blank file file while effectively erradicating any unsaved
        /// changes to the previously open document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mainEditor.Text.Equals(EMPTY_STRING))
            {
                MessageBox.Show("Unsaved changes will be overridden!");
            }
            NEW_FILE = true;
        }

        /// <summary>
        /// Detects the current syntax via the string argument that gets passed in.
        /// </summary>
        /// <param name="fName">Name of the file currently open in the editor</param>
        private void setCurrentEditorSyntaxHighlight(string fName)
        {
            char[] name = fName.ToCharArray();
            string ext = "";
            int token = fName.Length-1;

            while (name[token] != '.')
            {
                ext += name[token].ToString();
                token -= 1;
            }
            name = ext.ToCharArray();
            Array.Reverse(name);
            token = 0;
            ext = "";
            while (token < name.Length)
            {
                ext += name[token].ToString();
                token += 1;
            }

            switch(ext)
            {
                case _HTML_EXT:
                    mainEditor.Language = Language.HTML;
                    syntaxLabel.Text = "HTML";
                    break;
                case _XML_EXT:
                    mainEditor.Language = Language.XML;
                    syntaxLabel.Text = "XML";
                    break;
                case _JAVASCRIPT_EXT:
                    mainEditor.Language = Language.JS;
                    syntaxLabel.Text = "JavaScript";
                    break;
                case _LUA_EXT:
                    mainEditor.Language = Language.Lua;
                    syntaxLabel.Text = "Lua";
                    break;
                case _CSHARP_EXT:
                    mainEditor.Language = Language.CSharp;
                    syntaxLabel.Text = "C#";
                    break;
                case _SQL_EXT:
                    mainEditor.Language = Language.SQL;
                    syntaxLabel.Text = "SQL";
                    break;
                default:
                    mainEditor.Language = Language.Custom;
                    syntaxLabel.Text = "None";
                    break;
            }
        }
        
        /// <summary>
        /// Opens a save file dialogue upon clicking the "Save" Option
        /// available in the "Edit" drop-down menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NEW_FILE)
            {
                doSaveAs();
            }
            else
            {
                doSave();
            }
        }
        
        /// <summary>
        /// Calls the doSaveAs() function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doSaveAs();
        }

        /// <summary>
        /// Opens a save as file dialogue upon clicking the "Save As" Option
        /// available in the "Edit" drop-down menu.
        /// </summary>
        public bool doSaveAs()
        {
            SaveFileDialog saveDlg = new SaveFileDialog();

            // To filter files from SaveFileDialog
            saveDlg.Filter = "All Files (*.*)|*.*|Rich Text File (*.rtf)|*.rtf|Text Documents (*.txt)|*.txt";
            saveDlg.DefaultExt = "*.rtf";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Save the contents";

            DialogResult retval = saveDlg.ShowDialog();

            try
            {
                StreamWriter streamWriter = new StreamWriter(saveDlg.FileName);
                streamWriter.Write("");
                currentFileEncoding = streamWriter.Encoding;
                streamWriter.Close();
            } catch (Exception e) {
                MessageBox.Show(e.Message, e.GetType().Name);
            }
            
            if (retval == DialogResult.OK)
            {
                try {
                    mainEditor.SaveToFile(saveDlg.FileName, currentFileEncoding);
                    MessageBox.Show("File Saved");
                    return true;
                } catch (Exception e) {
                    MessageBox.Show(e.Message, e.GetType().Name);
                    return false;
                }
            }
            return false;
        }

        private string searchTextCache;
        private bool lastMatchCaseCache;
        private bool lastSearchDownCache;

        public bool FindAndSelect(string searchText, bool matchCase, bool searchDown)
        {
            int Index;

            var eStringComparison = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

            if (searchDown)
            {
                Index = mainEditor.Text.IndexOf(searchText, SelectionEnd, eStringComparison);
            }
            else
            {
                Index = mainEditor.Text.LastIndexOf(searchText, SelectionStart, SelectionStart, eStringComparison);
            }

            if (Index == -1) return false;

            searchTextCache = searchText;
            lastMatchCaseCache = matchCase;
            lastSearchDownCache = searchDown;

            SelectionStart = Index;
            SelectionLength = searchText.Length;

            return true;
        }

        private FindDialog _FindDialog;

        private void menuitemEditFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void Find()
        {
            if (mainEditor.Text.Length == 0) return;

            if (_FindDialog == null)
            {
                _FindDialog = new FindDialog(this);
            }

            _FindDialog.Left = this.Left + 56;
            _FindDialog.Top = this.Top + 160;

            if (!_FindDialog.Visible)
            {
                _FindDialog.Show(this);
            }
            else
            {
                _FindDialog.Show();
            }

            _FindDialog.Triggered();
        }

        public int SelectionEnd
        {
            get { return SelectionStart + SelectionLength; }
        }


        public int SelectionStart
        {
            get { return mainEditor.SelectionStart; }
            set
            {
                mainEditor.SelectionStart = value;
            }
        }

        public int SelectionLength
        {
            get { return mainEditor.SelectionLength; }
            set { mainEditor.SelectionLength = value; }
        }


        /// <summary>
        /// Attempts to save the current file upon clicking the "Save" Option
        /// available in the "Edit" drop-down menu.
        /// </summary>
        private bool doSave()
        {
            try
            {
                mainEditor.SaveToFile(FILE_NAME, currentFileEncoding);
                MessageBox.Show("File Saved");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().Name);
                return false;
            }
        }
        
        /// <summary>
        /// Closes the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Undoes the last change to the currently open file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Undo();
            redoToolStripMenuItem.Enabled = true;
        }
        
        /// <summary>
        /// Creates a new instance of {findFunctionForm} if {FIND_FORM_CLOSED}
        /// or the local variable { findFunctionForm } is null. Thus allowing the user
        /// to search for certain words within the file that is currently open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FIND_FORM_CLOSED || findFunctionForm == null)
            {
                findFunctionForm = new FindDialog(this);
                findFunctionForm.Visible = true;
            }
        }

        /// <summary>
        /// This method is used to determine if the currently open file is dirty.
        /// If the file is dirty then the {#textEditorControl1.Undo()} function is performed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainEditor_TextChanging(object sender, TextChangingEventArgs e)
        {
            IS_FILE_DIRTY = true;
            undoToolStripMenuItem.Enabled = true;
            findButton.Enabled = !mainEditor.Text.Equals(EMPTY_STRING) ? true : false;
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.CSharp;
            syntaxLabel.Text = "C#";
        }

        private void sQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.SQL;
            syntaxLabel.Text = "SQL";
        }

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.HTML;
            syntaxLabel.Text = "HTML";
        }
        
        private void javaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.JS;
            syntaxLabel.Text = "JavaScript";
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.XML;
            syntaxLabel.Text = "XML";
        }

        private void pHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.PHP;
            syntaxLabel.Text = "PHP";
        }

        private void luaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.Lua;
            syntaxLabel.Text = "Lua";
        }

        private void visualBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.VB;
            syntaxLabel.Text = "Visual Basic";
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainEditor.Language = Language.Custom;
            syntaxLabel.Text = "None";
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = statusBarToolStripMenuItem.Checked ? true : false;
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            if (!FindAndSelect(searchTextCache, lastMatchCaseCache, lastSearchDownCache))
            {
                MessageBox.Show(this, "No Results Found...", "Warning");
            }
            mainEditor.Focus();
        }

        /// <summary>
        /// Opens a windows font dialog which allows the user to change or modify
        /// the current font. However, the FastColoredTextBox only supports monospaced fonts
        /// so not all fonts will work correctly.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.ShowColor = true;
            fontDialog.ShowApply = true;
            fontDialog.ShowEffects = true;
            fontDialog.ShowHelp = true;
            if (fontDialog.ShowDialog() != DialogResult.Cancel)
            {
                mainEditor.Font = fontDialog.Font;
            }
        }
    }
}
