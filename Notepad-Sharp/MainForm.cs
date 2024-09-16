using NotepadSharp.Properties;
using NotepadSharp.Utils;
using NotepadSharp.Windows;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NotepadSharp
{
    /// <summary>
    /// Defines the <see cref="MainForm" />
    /// </summary>
    public partial class MainForm : Form
    {
        #region Variable declarations and definitions

        //Dialog definitions
        internal OpenFileDialog file_open;
        internal FontDialog fontDialog;
        internal LoggerForm logger;
        internal DocMap documentMap;
        List<string> closedFileNames;

        //General variable declarations and definitions
        private readonly Range _selection;
        private bool _highlightCurrentLine = true;
        private bool _enableDocumentMap = true;
        List<Editor> tablist = new List<Editor>();

        /// <summary>
        /// Gets the Current instance of <see cref="Editor"/>
        /// </summary>
        public Editor CurrentTB
        {
            get
            {
                return this.dockpanel.ActiveContent as Editor;
            }
        }

        /// <summary>
        /// Gets or sets Current selection <see cref="Range"/>.
        /// </summary>
        public Range Selection
        {
            get { return _selection; }
            set
            {
                if (value == _selection)
                {
                    return;
                }

                _selection.BeginUpdate();
                _selection.Start = value.Start;
                _selection.End = value.End;
                _selection.EndUpdate();
                Invalidate();
            }
        }

        /// <summary>
        /// Public variable to update the <see cref="syntaxLabel"/> text.
        /// </summary>
        public string SyntaxStatusBarLabelText
        {
            get
            {
                return syntaxLabel.Text;
            }
            set
            {
                if (null != value && "" != value)
                {
                    syntaxLabel.Text = value;
                }
            }
        }

        AutocompleteMenu popupMenu;
        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
        string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
        string[] snippets = { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(^;;)\n{\n;\n}", "while(^)\n{\n;\n}", "do${\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
        string[] declarationSnippets = {
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
               };

        #endregion

        #region Class Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            logger = new LoggerForm();

            dockpanel.Theme = new VS2005Theme();
            
            IsMdiContainer = true;

            logger.Log("Form Initialized!", LoggerMessageType.Info);
            
            CreateTab(null);
            UpdateDocumentMap();

            BuildAutocompleteMenu();
        }

        #endregion

        #region Tab Functionality

        void CreateTab(string fileName)
        {
            if (IsFileAlreadyOpen(fileName))
            {
                return;
            }

            string title = fileName != null ? Path.GetFileName(fileName) : "new " + NextUntitledNumber();
            Editor tab = new Editor(this, fileName, title);

            if (null == fileName)
            {
                tab.DetectSyntax("");
                tab.IsUntitled = true;
            }
            else
            {
                string ext = fileName != null ? Utility.GetExtension(fileName) : "";
                tab.DetectSyntax(ext);
                tab.mainEditor.OpenFile(fileName);
            }

            tab.mainEditor.KeyDown += new KeyEventHandler(MainForm_KeyDown);
            tab.mainEditor.TextChangedDelayed += new EventHandler<TextChangedEventArgs>(Tb_TextChangedDelayed);
            tab.mainEditor.MouseClick += new MouseEventHandler(MainForm_MouseClick);
            tab.Show(this.dockpanel, DockState.Document);
            tablist.Add(tab);
            tab.Focus();
            UpdateDocumentMap();
            HighlightCurrentLine();
        }

        int NextUntitledNumber()
        {
            List<byte> usedNumbers = new List<byte>();
            for (byte i = 0; i < tablist.Count; i++)
            {
                if (tablist[i].IsUntitled)
                {
                    string result = Regex.Match(tablist[i].Title, @"\d+").Value;
                    if ("" == result)
                    {
                        continue;
                    }
                    usedNumbers.Add(Byte.Parse(result));
                }
            }

            byte newNumber = 1;
            bool numberAvail = true;
            bool found = false;

            do
            {
                for (byte j = 0; j < usedNumbers.Count; j++)
                {
                    numberAvail = true;
                    found = false;
                    if (usedNumbers[j] == newNumber)
                    {
                        numberAvail = false;
                        found = true;
                        break;
                    }
                }
                if (!numberAvail)
                {
                    newNumber++;
                }
                if (!found)
                {
                    break;
                }
            }
            while (!numberAvail);

            return newNumber;
        }

        bool IsFileAlreadyOpen(string fileName)
        {
            if (null == fileName)
            {
                return false;
            }

            foreach (Editor tab in tablist)
            {
                if (tab.Tag as string == fileName)
                {
                    tab.Activate();
                    return true;
                }
            }
            return false;
        }

        void CloseAllTabs()
        {
            foreach (Editor tab in tablist.ToArray())
            {
                tab.Close();
            }
        }

        void ChangeDockPanelTheme(DockPanelThemeType type)
        {
            CloseAndSaveAllTabsInMemory();
            documentMap.Close();
            documentMap = null;

            switch (type)
            {
                case DockPanelThemeType.VS2003Theme:
                    dockpanel.Theme = new VS2003Theme();
                    break;
                case DockPanelThemeType.VS2005Theme:
                    dockpanel.Theme = new VS2005Theme();
                    break;
                case DockPanelThemeType.VS2012BlueTheme:
                    dockpanel.Theme = new VS2012BlueTheme();
                    break;
                case DockPanelThemeType.VS2012DarkTheme:
                    dockpanel.Theme = new VS2012DarkTheme();
                    break;
                case DockPanelThemeType.VS2012LightTheme:
                    dockpanel.Theme = new VS2012LightTheme();
                    break;
                case DockPanelThemeType.VS2013BlueTheme:
                    dockpanel.Theme = new VS2013BlueTheme();
                    break;
                case DockPanelThemeType.VS2013DarkTheme:
                    dockpanel.Theme = new VS2013DarkTheme();
                    break;
                case DockPanelThemeType.VS2013LightTheme:
                    dockpanel.Theme = new VS2013LightTheme();
                    break;
                case DockPanelThemeType.VS2015BlueTheme:
                    dockpanel.Theme = new VS2015BlueTheme();
                    break;
                case DockPanelThemeType.VS2015DarkTheme:
                    dockpanel.Theme = new VS2015DarkTheme();
                    break;
                case DockPanelThemeType.VS2015LightTheme:
                    dockpanel.Theme = new VS2015LightTheme();
                    break;
            }

            OpenTabsFromMemory();
        }

        void CloseAndSaveAllTabsInMemory()
        {
            if (null == closedFileNames)
            {
                closedFileNames = new List<string>();
            }
            foreach (Editor tab in tablist.ToArray())
            {
                closedFileNames.Add(tab.Tag as string);
                tab.Close();
            }
        }

        void OpenTabsFromMemory()
        {
            foreach(string fn in closedFileNames)
            {
                CreateTab(fn);
            }

            closedFileNames = null;
        }

        bool CallSave(Editor tab)
        {
            if (tab.Save())
            {
                UpdateChangedFlag(false);
                return true;
            }
            return false;
        }

        void HighlightCurrentLine()
        {
            foreach (Editor tab in tablist.ToArray())
            {
                tab.HighlightCurrentLine(_highlightCurrentLine);
            }
            if (CurrentTB != null)
            {
                CurrentTB.Invalidate();
            }
        }

        void ChangeFont(Font font)
        {
            if (font.Size <= 4)
                return;

            foreach (Editor tab in tablist)
            {
                tab.mainEditor.Font = font;
            }
        }

        #endregion

        #region Button Click Event Handlers

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Undo();
            }
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            CreateTab(null);
        }

        private void FindButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.ShowFindDialog();
            }
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            if (null == file_open)
            {
                file_open = new OpenFileDialog();
            }

            if (file_open.ShowDialog() == DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenFileDialog fd = (OpenFileDialog) sender;
            string fileName = fd.FileName;

            CreateTab(fileName);
        }

        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Close();
            }
        }

        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog == null)
            {
                fontDialog = new FontDialog();
                fontDialog.Font = EditorSettings.Font;
            }
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                ChangeFont(fontDialog.Font);
                EditorSettings.Font = fontDialog.Font;
            }
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CallSave(CurrentTB);
            }
        }

        private void CloseAllToolStripButton_Click(object sender, EventArgs e)
        {
            CloseAllTabs();
        }

        private void CutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Cut();
            }
        }

        private void PasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Paste();
            }
        }

        private void CopyToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Copy();
            }
        }

        private void UndoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Undo();
            }
        }

        private void RedoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Redo();
            }
        }

        private void ZoomInToolStripButton_Click(object sender, EventArgs e)
        {
            ChangeFont(new Font(EditorSettings.Font.Name, EditorSettings.Font.Size + 2));
        }

        private void ZoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            ChangeFont(new Font(EditorSettings.Font.Name, EditorSettings.Font.Size - 2));
        }

        private void FindToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.ShowFindDialog();
            }
        }

        private void DocumentMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enableDocumentMap = !_enableDocumentMap;
            UpdateDocumentMap();
        }

        private void CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.CS_EXT);
            }
        }

        private void NoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax("txt");
            }
        }

        private void HTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.HTML_EXT);
            }
        }

        private void JavaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.JS_EXT);
            }
        }

        private void LuaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.LUA_EXT);
            }
        }

        private void PHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.PHP_EXT);
            }
        }

        private void SQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.SQL_EXT);
            }
        }

        private void VisualBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.VB_EXT);
            }
        }

        private void XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.XML_EXT);
            }
        }

        private void JSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.JSON_EXT);
            }
        }

        private void batchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.DetectSyntax(GlobalConstants.BATCH_EXT);
            }
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statusBarToolStripMenuItem.Checked)
            {
                statusStrip1.Show();
                this.dockpanel.Height -= statusStrip1.Height;
            }
            else
            {
                statusStrip1.Hide();
                this.dockpanel.Height += statusStrip1.Height;
            }
        }

        private void RefreshToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Refresh();
                var r = new Range(CurrentTB.mainEditor);
                r.SelectAll();
                CurrentTB.mainEditor.OnSyntaxHighlight(new TextChangedEventArgs(r));
            }
        }

        private void HlCurrentLineToolStripButton_Click(object sender, EventArgs e)
        {
            _highlightCurrentLine = _highlightCurrentLine ? false : true;

            HighlightCurrentLine();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == file_open)
            {
                file_open = new OpenFileDialog();
            }

            if (file_open.ShowDialog() == DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CallSave(CurrentTB);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                string oldFile = CurrentTB.Tag as string;
                CurrentTB.Tag = null;
                if (!CallSave(CurrentTB))
                {
                    if (oldFile != null)
                    {
                        CurrentTB.Tag = oldFile;
                        CurrentTB.Title = Path.GetFileName(oldFile);
                    }
                }
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTab(null);
        }

        private void ReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.ShowReplaceDialog();
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Copy();
            }
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Redo();
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Cut();
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.Paste();
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = "About Notepad#";
            string message = "Created by: Zachary Pedigo\nVersion: " + Resources.ApplicationVersion + "\n" + "Date: " + DateTime.Now + "\n" + "OS: "
                + Environment.OSVersion + "\nLicense: GNU General Public License v3.0";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logger.Show();
        }

        private void CutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
                CurrentTB.mainEditor.Cut();
        }

        private void CopyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
                CurrentTB.mainEditor.Copy();
        }

        private void PasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
                CurrentTB.mainEditor.Paste();
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
                CurrentTB.mainEditor.SelectAll();
        }

        #endregion

        #region Event Handlers

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentTB != null)
            {
                if (e.Control && e.KeyCode == Keys.O)
                {
                    if (null == file_open)
                    {
                        file_open = new OpenFileDialog();
                    }

                    if (file_open.ShowDialog() == DialogResult.OK)
                    {
                        CreateTab(file_open.FileName);
                    }
                }
                else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
                {
                    if (CurrentTB != null)
                    {
                        CallSave(CurrentTB);
                    }
                }
                else if (e.Control && e.Shift && e.KeyCode == Keys.L)
                {
                    CurrentTB.mainEditor.ClearCurrentLine();
                }
                else if (e.Control && e.Shift && e.KeyCode == Keys.Oem2 && CurrentTB.mainEditor.CommentPrefix != null)
                {
                    if (!CurrentTB.mainEditor.SelectedText.Contains(CurrentTB.mainEditor.CommentPrefix))
                    {
                        CurrentTB.mainEditor.InsertLinePrefix(CurrentTB.mainEditor.CommentPrefix);
                    }
                    else
                    {
                        CurrentTB.mainEditor.RemoveLinePrefix(CurrentTB.mainEditor.CommentPrefix);
                    }
                }
            }
        }

        private void Tb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            var tb = sender as FastColoredTextBox;
            UpdateChangedFlag(tb.IsChanged);
        }

        private void TsFiles_TabStripItemClosed(object sender, EventArgs e)
        {
            UpdateDocumentMap();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Editor tab in tablist.ToArray())
            {
                Editor_TabClosing(e, tab);
            }
        }

        public void Editor_TabClosing(FormClosingEventArgs e, Editor item)
        {
            if (item.mainEditor.IsChanged)
            {
                switch (MessageBox.Show("Do you want to save " + item.Title + " ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case DialogResult.Yes:
                        if (!CallSave(item))
                        {
                            e.Cancel = true;
                            return;
                        }
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
            tablist.Remove(item);
            this.dockpanel.Controls.Remove(item);
        }

        private void dockpanel_ActiveContentChanged(object sender, EventArgs e)
        {
            UpdateDocumentMap();

            if (CurrentTB != null)
            {
                this.syntaxLabel.Text = CurrentTB.SyntaxText;
                BuildAutocompleteMenu();
                UpdateChangedFlag(CurrentTB.mainEditor.IsChanged);
            }
        }

        private bool IsSavedTab(string tagName) => tagName != null;

        private void DiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> filePaths = new List<string>();

            foreach (Editor item in tablist)
            {
                string filePath = (string)item.Tag;

                if (IsSavedTab(filePath))
                    filePaths.Add(filePath);

                    if (filePaths.Count == 2)
                        break;
            }

            if  (filePaths.Count == 1)
                new DiffViewerForm(filePaths[0]).Show();
            else if (filePaths.Count == 2)
                new DiffViewerForm(filePaths[0], filePaths[1]).Show();
            else
                new DiffViewerForm().Show();
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this, new Point(e.X, e.Y));
            }
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2005Theme);
        }

        private void legacyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2012BlueTheme);
        }

        private void retroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2012DarkTheme);
        }

        private void vS2012LightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2012LightTheme);
        }

        private void vS2013BLueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2013BlueTheme);
        }

        private void vS2013DarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2013DarkTheme);
        }

        private void vS2013LightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2013LightTheme);
        }

        private void vS2015BLueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2015BlueTheme);
        }

        private void vS2015DarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2015DarkTheme);
        }

        private void vS2015LightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2015LightTheme);
        }

        private void vS2003ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDockPanelTheme(DockPanelThemeType.VS2003Theme);
        }

        #endregion

        #region Document Map Functionality

        private void UpdateDocumentMap()
        {
            if (documentMap == null && _enableDocumentMap)
                BuildDocumentMap();

            if (CurrentTB != null && documentMap != null)
            {
                documentMap.Target = tablist.Count > 0 ? CurrentTB.mainEditor : null;
                documentMap.Visible = _enableDocumentMap;
                if (!_enableDocumentMap || documentMap.Target == null)
                {
                    documentMap.Close();
                    documentMap = null;
                    return;
                }
            }
        }

        private void BuildDocumentMap()
        {
            documentMap = new DocMap();
            documentMap.Show(this.dockpanel, DockState.DockRight);
        }

        #endregion

        #region AutoCompleteMenu Functionality

        private void BuildAutocompleteMenu()
        {
            if (CurrentTB == null)
                return;
            List<AutocompleteItem> items = new List<AutocompleteItem>();

            foreach (var item in snippets)
                items.Add(new SnippetAutocompleteItem(item) { ImageIndex = 1 });
            foreach (var item in declarationSnippets)
                items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
            foreach (var item in methods)
                items.Add(new MethodAutocompleteItem(item) { ImageIndex = 2 });
            foreach (var item in keywords)
                items.Add(new AutocompleteItem(item));

            items.Add(new InsertSpaceSnippet());
            items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
            items.Add(new InsertEnterSnippet());

            popupMenu = new AutocompleteMenu(CurrentTB.mainEditor);

            //set as autocomplete source
            popupMenu.Items.SetAutocompleteItems(items);
        }

        /// <summary>
        /// This item appears when any part of snippet text is typed
        /// </summary>
        class DeclarationSnippet : SnippetAutocompleteItem
        {
            public DeclarationSnippet(string snippet)
                : base(snippet)
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var pattern = Regex.Escape(fragmentText);
                if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
                    return CompareResult.Visible;
                return CompareResult.Hidden;
            }
        }

        /// <summary>
        /// Divides numbers and words: "123AND456" -> "123 AND 456"
        /// Or "i=2" -> "i = 2"
        /// </summary>
        class InsertSpaceSnippet : AutocompleteItem
        {
            string pattern;

            public InsertSpaceSnippet(string pattern) : base("")
            {
                this.pattern = pattern;
            }

            public InsertSpaceSnippet()
                : this(@"^(\d+)([a-zA-Z_]+)(\d*)$")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                if (Regex.IsMatch(fragmentText, pattern))
                {
                    Text = InsertSpaces(fragmentText);
                    if (Text != fragmentText)
                        return CompareResult.Visible;
                }
                return CompareResult.Hidden;
            }

            public string InsertSpaces(string fragment)
            {
                var m = Regex.Match(fragment, pattern);
                if (m == null)
                    return fragment;
                if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
                    return fragment;
                return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return Text;
                }
            }
        }

        /// <summary>
        /// Inerts line break after '}'
        /// </summary>
        class InsertEnterSnippet : AutocompleteItem
        {
            Place enterPlace = Place.Empty;

            public InsertEnterSnippet()
                : base("[Line break]")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var r = Parent.Fragment.Clone();
                while (r.Start.iChar > 0)
                {
                    if (r.CharBeforeStart == '}')
                    {
                        enterPlace = r.Start;
                        return CompareResult.Visible;
                    }

                    r.GoLeftThroughFolded();
                }

                return CompareResult.Hidden;
            }

            public override string GetTextForReplace()
            {
                //extend range
                Range r = Parent.Fragment;
                Place end = r.End;
                r.Start = enterPlace;
                r.End = r.End;
                //insert line break
                return Environment.NewLine + r.Text;
            }

            public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
            {
                base.OnSelected(popupMenu, e);
                if (Parent.Fragment.tb.AutoIndent)
                    Parent.Fragment.tb.DoAutoIndent();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return "Insert line break after '}'";
                }
            }
        }

        #endregion

        #region Tool Strip Functionality

        private void UpdateChangedFlag(bool isChanged)
        {
            if (CurrentTB != null)
            {
                CurrentTB.mainEditor.IsChanged = isChanged;
                saveToolStripButton.Enabled = isChanged;
            }
        }

        #endregion
    }
}
