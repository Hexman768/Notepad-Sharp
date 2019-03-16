using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Essay_Analysis_Tool
{
    public partial class mainForm : Form
    {
        // Declaring and Initializing useful variables
        OpenFileDialog file_open = new OpenFileDialog();
        SaveFileDialog sfdMain = new SaveFileDialog();
        FontDialog fontDialog = new FontDialog();
        FindForm findForm;
        ContextMenuStrip cmMain;
        Color currentLineColor = Color.FromArgb(100, 210, 210, 255);
        Color changedLineColor = Color.FromArgb(255, 230, 230, 255);
        private readonly Range selection;

        public bool UNDO_AVAIALBE = false;
        public bool FIND_FORM_CLOSED = true;
        public bool IS_FILE_DIRTY = false;
        public bool NEW_FILE = true;
        public bool BATCH_HIGHLIGHTING = false;
        public bool WRAP_SEARCH = false;
        public String EMPTY_STRING = "";
        public int tabCount;
        
        private const string _HTML_EXT = "html";
        private const string _XML_EXT = "xml";
        private const string _JAVASCRIPT_EXT = "js";
        private const string _CSHARP_EXT = "cs";
        private const string _LUA_EXT = "lua";
        private const string _SQL_EXT = "sql";
        private const string _JAVA_EXT = "java";
        private const string _BAT_EXT = "bat";

        //styles
        TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle LightBlueStyle = new TextStyle(Brushes.RoyalBlue, null, FontStyle.Regular);
        TextStyle YellowStyle = new TextStyle(Brushes.YellowGreen, null, FontStyle.Regular);
        TextStyle RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle GreenStyleItalic = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        TextStyle BrownStyleItalic = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

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
            if (file_open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        private Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

        /// <summary>
        /// Creates a new tab as well as a new instance of FastColoredTextBox for
        /// the given filename.
        /// </summary>
        /// <param name="fileName">File Name</param>
        private void CreateTab(string fileName)
        {
            try
            {
                var tb = new FastColoredTextBox();
                tb.Font = new Font("Consolas", 9.75f);
                tb.ContextMenuStrip = cmMain;
                tb.Dock = DockStyle.Fill;
                tb.BorderStyle = BorderStyle.Fixed3D;
                tb.LeftPadding = 17;
                tb.AddStyle(sameWordsStyle);//same words style
                var tab = new FATabStripItem(fileName != null ? Path.GetFileName(fileName) : "[new]", tb);
                tab.Tag = fileName;
                tb.HighlightingRangeType = HighlightingRangeType.VisibleRange;
                if (fileName != null)
                {
                    if (!setCurrentEditorSyntaxHighlight(fileName, tb))
                    {
                        tb.OpenFile(fileName);
                        batchSyntaxHighlight(tb);
                    }
                    else
                    {
                        tb.OpenFile(fileName);
                    }
                }
                tsFiles.AddTab(tab);
                tsFiles.SelectedItem = tab;
                tb.Focus();
                tb.TextChanged += new EventHandler<TextChangedEventArgs>(tb_TextChanged);
                tb.DelayedTextChangedInterval = 1000;
                tb.DelayedEventsInterval = 500;
                tb.ChangedLineColor = changedLineColor;
                AutocompleteMenu popupMenu = new AutocompleteMenu(tb);
                documentMap.Target = CurrentTB;
                tabCount++;
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                {
                    CreateTab(fileName);
                }
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
            CreateTab(null);
            NEW_FILE = true;
        }

        /// <summary>
        /// Detects the current syntax via the string argument that gets passed in.
        /// </summary>
        /// <param name="fName">Name of the file currently open in the editor</param>
        private bool setCurrentEditorSyntaxHighlight(string fName, FastColoredTextBox mainEditor)
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
                case _JAVA_EXT:
                    mainEditor.Language = Language.CSharp;
                    syntaxLabel.Text = "Java";
                    break;
                case _BAT_EXT:
                    mainEditor.Language = Language.Custom;
                    batchToolStripMenuItem.Checked = true;
                    syntaxLabel.Text = "Batch";
                    return false;
                default:
                    mainEditor.Language = Language.Custom;
                    syntaxLabel.Text = "None";
                    break;
            }
            return true;
        }

        public void changeSyntax(FastColoredTextBox tb, Language language)
        {
            try
            {
                tb.Language = language;
            }
            catch (Exception e)
            {
                if (MessageBox.Show(e.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                {
                    changeSyntax(tb, language);
                }
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
            if (tsFiles.SelectedItem != null)
            {
                Save(tsFiles.SelectedItem);
            }
        }
        
        /// <summary>
        /// Takes the current file and saves it as a new one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                string oldFile = tsFiles.SelectedItem.Tag as string;
                tsFiles.SelectedItem.Tag = null;
                if (!Save(tsFiles.SelectedItem))
                {
                    if (oldFile != null)
                    {
                        tsFiles.SelectedItem.Tag = oldFile;
                        tsFiles.SelectedItem.Title = Path.GetFileName(oldFile);
                    }
                }
            }
        }
        
        /// <summary>
        /// Shows find dialog
        /// </summary>
        public virtual void ShowFindDialog()
        {
            ShowFindDialog(null);
        }

        /// <summary>
        /// Shows find dialog
        /// </summary>
        public virtual void ShowFindDialog(string findText)
        {
            if (findForm == null)
            {
                findForm = new FindForm(CurrentTB);
                findForm.Show();
            }
            
            findForm.tbFind.SelectAll();
            findForm.Show();
            findForm.Focus();
        }
        
        FastColoredTextBox CurrentTB
        {
            get
            {
                if (tsFiles.SelectedItem == null)
                {
                    return null;
                }
                return (tsFiles.SelectedItem.Controls[0] as FastColoredTextBox);
            }

            set
            {
                tsFiles.SelectedItem = (value.Parent as FATabStripItem);
                value.Focus();
            }
        }
        
        /// <summary>
        /// Current selection range
        /// </summary>
        public Range Selection
        {
            get { return selection; }
            set
            {
                if (value == selection)
                {
                    return;
                }

                selection.BeginUpdate();
                selection.Start = value.Start;
                selection.End = value.End;
                selection.EndUpdate();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Attempts to save the current file upon clicking the "Save" Option
        /// available in the "Edit" drop-down menu.
        /// </summary>
        private bool Save(FATabStripItem tab)
        {
            var tb = (tab.Controls[0] as FastColoredTextBox);
            if (tab.Tag == null)
            {
                if (sfdMain.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return false;
                }
                tab.Title = Path.GetFileName(sfdMain.FileName);
                tab.Tag = sfdMain.FileName;
            }

            try
            {
                File.WriteAllText(tab.Tag as string, tb.Text);
                tb.IsChanged = false;
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                {
                    return Save(tab);
                }
                else
                {
                    return false;
                }
            }
            
            return true;
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
            CurrentTB.Undo();
        }
        
        /// <summary>
        /// Calls the create tab method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            CreateTab(null);
        }

        /// <summary>
        /// Calls the method to show the built-in find dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findButton_Click(object sender, EventArgs e)
        {
            ShowFindDialog();
        }

        /// <summary>
        /// Checks if the user clicks ok on the open file dialog, if so
        /// this method calls the method to create a new tab with the file
        /// name pulled from the open file dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        private void updateDocumentMap()
        {
            documentMap.Target = tabCount > 0 ? CurrentTB : null;
        }

        /// <summary>
        /// Removes the tab that is currently selected by the user then
        /// decreases the tab count by 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripButton_Click(object sender, EventArgs e)
        {
            tsFiles.RemoveTab(tsFiles.SelectedItem);
            tabCount--;
            updateDocumentMap();
        }

        /// <summary>
        /// Displays the font dialog which allows the user to change the editor font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentTB.Font = fontDialog.Font;
            }
        }

        /// <summary>
        /// Saves the current file if the current tab is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                Save(tsFiles.SelectedItem);
            }
        }

        /// <summary>
        /// Calls the close all tabs function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeAllToolStripButton_Click(object sender, EventArgs e)
        {
            CloseAllTabs();
            updateDocumentMap();
        }

        /// <summary>
        /// Closes all tabs until the running count of tabs is 0.
        /// </summary>
        private void CloseAllTabs()
        {            
            while (tabCount > 0)
            {
                tsFiles.RemoveTab(tsFiles.SelectedItem);
                tabCount--;
            }
            documentMap.Target = null;
        }

        private void batchSyntaxHighlight(FastColoredTextBox fctb)
        {
            fctb.LeftBracket = '(';
            fctb.RightBracket = ')';
            Range e = fctb.Range;
            // Clear all styles
            e.ClearStyle(StyleIndex.All);
            //variable highlighting
            e.SetStyle(YellowStyle, "(\".+?\"|\'.+?\')", RegexOptions.Singleline);
            e.SetStyle(MagentaStyle, @"%.+?%", RegexOptions.Multiline);
            //attribute highlighting
            e.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            //class name highlighting
            e.SetStyle(BoldStyle, @"^:[a-zA-Z]+", RegexOptions.Multiline);
            //symbol highlighting
            e.SetStyle(MagentaStyle, @"^(@)(?=(?i)echo)", RegexOptions.Multiline);
            e.SetStyle(RedStyle, @"(\*)", RegexOptions.Singleline);
            //keyword highlighting
            e.SetStyle(BlueStyle, @"(?<!(^(?i)(rem|::|echo).*))(?i)goto", RegexOptions.Multiline);
            e.SetStyle(BlueStyle, @"(?<!(^(?i)(rem|::|echo).*))(?i)do", RegexOptions.Multiline);
            e.SetStyle(BlueStyle, @"^([ ]{1,}|@)?\b(?i)(set|echo|for|pushd|popd|pause|exit|cd|if|else|goto|del|cls)(?![a-zA-Z]|[0-9])", RegexOptions.Multiline);
            //outside keyword highlighting
            e.SetStyle(LightBlueStyle, @"^([ ]{1,}|@)?\b(?i)(git)(?![a-zA-Z]|[0-9])", RegexOptions.Multiline);
            //comment highlighting
            e.SetStyle(GreenStyleItalic, @"(REM.*)");
            e.SetStyle(GreenStyleItalic, @"::.*");
            //clear folding markers
            e.ClearFoldingMarkers();
            BATCH_HIGHLIGHTING = true;
        }

        /// <summary>
        /// Calls the cut function on the current instance of FastColoredTextBox assuming it is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Cut();
            }
        }

        /// <summary>
        /// Calls the paste function on the current instance of FastColoredTextBox assuming it is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Paste();
            }
        }

        /// <summary>
        /// Calls the copy function on the current instance of FastColoredTextBox assuming it is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Copy();
            }
        }

        /// <summary>
        /// Calls the undo function on the current instance of FastColoredTextBox assuming it is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Undo();
            }
        }

        /// <summary>
        /// Calls the redo function on the current instance of FastColoredTextBox assuming it is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Redo();
            }
        }

        /// <summary>
        /// Increases the text size by 2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(2);
            }
        }

        /// <summary>
        /// Reduces the text size by 2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(-2);
            }
        }

        /// <summary>
        /// Opens the find dialog if the current instance of FastColoredTextBox is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                ShowFindDialog();
            }
        }

        /// <summary>
        /// Checks for hotkeys by checking which keycodes have been sent to 
        /// this method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.F)
            {
                if (CurrentTB != null)
                {
                    ShowFindDialog();
                }
            }
            else if (e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.C)
            {
                if (CurrentTB != null)
                {
                    CurrentTB.Copy();
                }
            }
            else if (e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.V)
            {
                if (CurrentTB != null)
                {
                    CurrentTB.Paste();
                }
            }
            else if (e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.X)
            {
                if (CurrentTB != null)
                {
                    CurrentTB.Cut();
                }
            }
            else if (e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.Z)
            {
                if (CurrentTB != null)
                {
                    CurrentTB.Undo();
                }
            }
            else if (e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.R)
            {
                if (CurrentTB != null)
                {
                    CurrentTB.Redo();
                }
            }
        }

        /// <summary>
        /// Sets the document map target to the current instance of FastColoredTextBox if
        /// the document map option under "view" is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void documentMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentMap.Visible = documentMapToolStripMenuItem.Checked ? true : false;
        }

        /// <summary>
        /// Sets the document map target to the current instance of FastColoredTextBox if
        /// the current instance of FastColoredTextBox is not null and the document map
        /// option under "view" is selected.
        /// </summary>
        /// <param name="e"></param>
        private void tsFiles_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            updateDocumentMap();
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "C#";
                changeSyntax(CurrentTB, Language.CSharp);
            }
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "None";
                changeSyntax(CurrentTB, Language.Custom);
            }
        }

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "HTML";
                changeSyntax(CurrentTB, Language.HTML);
            }
        }

        private void javaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "JavaScript";
                changeSyntax(CurrentTB, Language.JS);
            }
        }

        private void luaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Lua";
                changeSyntax(CurrentTB, Language.Lua);
            }
        }

        private void pHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "PHP";
                changeSyntax(CurrentTB, Language.PHP);
            }
        }

        private void sQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "SQL";
                changeSyntax(CurrentTB, Language.SQL);
            }
        }

        private void visualBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Visual Basic";
                changeSyntax(CurrentTB, Language.VB);
            }
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "XML";
                changeSyntax(CurrentTB, Language.XML);
            }
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statusBarToolStripMenuItem.Checked)
            {
                statusStrip1.Show();
            }
            else
            {
                statusStrip1.Hide();
            }
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentTB.Language == Language.Custom)
            {
                batchSyntaxHighlight(CurrentTB);
            }
        }

        private void batchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "Batch";
            CurrentTB.Language = Language.Custom;
            batchSyntaxHighlight(CurrentTB);
        }
    }
}
