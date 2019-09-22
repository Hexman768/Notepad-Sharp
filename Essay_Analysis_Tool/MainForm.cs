//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  License: GNU Lesser General Public License (LGPLv3)
//
//  Email: Zachary.Pedigo1@gmail.com
//
//  Copyright (C) Zachary Pedigo, 2019.

namespace Essay_Analysis_Tool
{
    using Essay_Analysis_Tool.Properties;
    using FarsiLibrary.Win;
    using FastColoredTextBoxNS;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MainForm" />
    /// </summary>
    public partial class MainForm : Form
    {
        #region Variable declarations and definitions

        //Dialog definitions
        internal OpenFileDialog file_open = new OpenFileDialog();
        internal SaveFileDialog sfdMain = new SaveFileDialog();
        internal FontDialog fontDialog = new FontDialog();
        LoggerForm logger = new LoggerForm();

        //Form declarations
        internal FindForm findForm;

        //Line Colors
        internal Color currentLineColor = Color.FromArgb(100, 210, 210, 255);
        internal Color changedLineColor = Color.FromArgb(255, 230, 230, 255);

        //General variable declarations and definitions
        private readonly Range _selection;
        private bool _highlightCurrentLine = true;
        private bool _enableDocumentMap = true;

        //file extensions
        private const string _html = "html";
        private const string _xml = "xml";
        private const string _javascript = "js";
        private const string _csharp = "cs";
        private const string _lua = "lua";
        private const string _sql = "sql";
        private const string _vb = "vb";
        private const string _vbs = "vbs";

        /// <summary>
        /// Defines the Platform Type.
        /// </summary>
        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();

        //Styles
        private Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

        /// <summary>
        /// Gets or sets the Current instance of FastColoredTextBox.
        /// </summary>
        internal FastColoredTextBox CurrentTB
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
        /// Gets or sets Current selection range.
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

        /// <summary>
        /// Initializes a new instance of the <see cref="mainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            logger.Log("Form Initialized!", LoggerMessageType.Info);
            sfdMain.Filter = Resources.NormalTextFileType + "|*.txt|"
                + Resources.CSharpFileType + "|*.cs|"
                + Resources.HTMLFileType + "|*.html|"
                + Resources.JSFileType + "|*.js|"
                + Resources.JSONFileType + "|*.json|"
                + Resources.LuaFileType + "|*.lua|"
                + Resources.PHPFileType + "|*.php|"
                + Resources.SQLFileType + "|*.sql|"
                + Resources.VBFileType + "|*.vb|"
                + Resources.VBSFileType + "|*.vbs|"
                + Resources.AllFileTypes + "|*.*";

            CreateTab();
            BuildAutocompleteMenu();
        }

        #region Tab Functionality

        /// <summary>
        /// Creates a new tab with read and write access to a specified file
        /// directed at by the path contained in the fileName.
        /// </summary>
        /// <param name="fileName">Name and Path of the file.</param>
        private void CreateTab(string fileName)
        {
            var tb = new FastColoredTextBox
            {
                Font = new Font("Consolas", 9.75f),
                ContextMenuStrip = null,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.Fixed3D,
                LeftPadding = 17,
                HighlightingRangeType = HighlightingRangeType.VisibleRange
            };

            tb.AddStyle(sameWordsStyle);

            var tab = new FATabStripItem(fileName != null ? Path.GetFileName(fileName) : "[new]", tb)
            {
                Tag = fileName
            };

            if (fileName != null && !IsFileAlreadyOpen(fileName))
            {
                SetCurrentEditorSyntaxHighlight(fileName, tb);
                tb.OpenFile(fileName);
            }
            else if (fileName != null)
            {
                return;
            }

            //create autocomplete popup menu
            popupMenu = new AutocompleteMenu(tb);
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
            popupMenu.AllowTabKey = true;

            tsFiles.AddTab(tab);
            tsFiles.SelectedItem = tab;
            tb.Focus();
            tb.ChangedLineColor = changedLineColor;
            tb.KeyDown += new KeyEventHandler(MainForm_KeyDown);
            tb.TextChangedDelayed += new EventHandler<TextChangedEventArgs>(Tb_TextChangedDelayed);
            UpdateDocumentMap();
            HighlightCurrentLine();
        }

        private bool IsFileAlreadyOpen(string fileName)
        {
            foreach (FATabStripItem tab in tsFiles.Items)
            {
                if (tab.Tag as string == fileName)
                {
                    tsFiles.SelectedItem = tab;
                    return true;
                }
            }
            return false;
        }

        private void CreateTab()
        {
            CreateTab(null);
        }

        /// <summary>
        /// This function closes all tabs via the Count variable stored within the tab strip.
        /// </summary>
        private void CloseAllTabs()
        {
            List<FATabStripItem> list = GetTabList();
            foreach (FATabStripItem tab in list)
            {
                TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tab);
                TsFiles_TabStripItemClosing(args);
                if (args.Cancel)
                {
                    return;
                }
                tsFiles.RemoveTab(tab);
            }
            SanitizeTabStrip();
        }

        /// <summary>
        /// Gets the list of tabs open in the FATabStrip.
        /// </summary>
        /// <returns>List FATabStripItem Instances<see cref="List{FATabStripItem}"/></returns>
        private List<FATabStripItem> GetTabList()
        {
            List<FATabStripItem> list = new List<FATabStripItem>();

            foreach (FATabStripItem tab in tsFiles.Items)
            {
                list.Add(tab);
            }

            return list;
        }

        /// <summary>
        /// Attempts to save the current file upon clicking the "Save" Option
        /// available in the "Edit" drop-down menu.
        /// </summary>
        /// <param name="tab">Tab inside the tab list.</param>
        /// <returns>A boolean value based on whether or not a successfull save was performed.</returns>
        private bool Save(FATabStripItem tab)
        {
            if (tab == null)
            {
                logger.Log(Resources.NullTabStripItem, LoggerMessageType.Error);
                MessageBox.Show("Save Unsucessfull.", "Save not complete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            FastColoredTextBox tb = tab.Controls[0] as FastColoredTextBox;

            if (tab.Tag == null)
            {
                if (sfdMain.ShowDialog() != DialogResult.OK)
                {
                    return true;
                }
                tab.Title = Path.GetFileName(sfdMain.FileName);
                tab.Tag = sfdMain.FileName;
            }

            string filePath = (string)tab.Tag;

            SetCurrentEditorSyntaxHighlight(filePath, tb);

            File.WriteAllText(filePath, tb.Text);
            UpdateChangedFlag(false);

            return true;
        }

        /// <summary>
        /// Starts or stops highlighting the current line based on {@code _highlightCurrentLine}.
        /// </summary>
        private void HighlightCurrentLine()
        {
            foreach (FATabStripItem tab in tsFiles.Items)
            {
                if (_highlightCurrentLine)
                {
                    (tab.Controls[0] as FastColoredTextBox).CurrentLineColor = currentLineColor;
                }
                else
                {
                    (tab.Controls[0] as FastColoredTextBox).CurrentLineColor = Color.Transparent;
                }
            }
            if (CurrentTB != null)
            {
                CurrentTB.Invalidate();
            }
        }

        #endregion

        #region Button Click Event Handlers

        /// <summary>
        /// Handles Event ExitToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Handles the UndoToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Undo();
            }
        }

        /// <summary>
        /// Handles the NewToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            CreateTab();
        }

        /// <summary>
        /// Handles the FindButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void FindButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ShowFindDialog();
            }
        }

        /// <summary>
        /// Handles the OpenToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        /// <summary>
        /// Handles the CloseToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tsFiles.SelectedItem);
                TsFiles_TabStripItemClosing(args);
                if (args.Cancel)
                {
                    return;
                }
                tsFiles.RemoveTab(tsFiles.SelectedItem);
                UpdateDocumentMap();
            }
            SanitizeTabStrip();
        }

        /// <summary>
        /// Hanldes the FontToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null && fontDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentTB.Font = fontDialog.Font;
            }
        }

        /// <summary>
        /// Handles the SaveToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                Save(tsFiles.SelectedItem);
            }
        }

        /// <summary>
        /// The CloseAllToolStripButton_Click
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void CloseAllToolStripButton_Click(object sender, EventArgs e)
        {
            CloseAllTabs();
            UpdateDocumentMap();
        }

        /// <summary>
        /// Handles the CutToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void CutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Cut();
            }
        }

        /// <summary>
        /// Handles the PasteToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void PasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Paste();
            }
        }

        /// <summary>
        /// Handles the CopyToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void CopyToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Copy();
            }
        }

        /// <summary>
        /// Handles the UndoToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void UndoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Undo();
            }
        }

        /// <summary>
        /// Handles the RedoToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void RedoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Redo();
            }
        }

        /// <summary>
        /// Handles the ZoomInToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void ZoomInToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(2);
            }
        }

        /// <summary>
        /// Handles the ZoomOutToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void ZoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(-2);
            }
        }

        /// <summary>
        /// Handles the FindToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void FindToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ShowFindDialog();
            }
        }

        /// <summary>
        /// Handles the DocumentMapToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void DocumentMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enableDocumentMap = _enableDocumentMap ? false : true;

            UpdateDocumentMap();
        }

        /// <summary>
        /// Handles the CToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "C#";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.CSharp);
            }
        }

        /// <summary>
        /// Handles the NoneToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void NoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "None";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.Custom);
            }
        }

        /// <summary>
        /// Handles the HTMLToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void HTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "HTML";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.HTML);
            }
        }

        /// <summary>
        /// Handles the JavaScriptToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void JavaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "JavaScript";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.JS);
            }
        }

        /// <summary>
        /// Handles the LuaToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void LuaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "Lua";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.Lua);
            }
        }

        /// <summary>
        /// Handles the PHPToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void PHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "PHP";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.PHP);
            }
        }

        /// <summary>
        /// Handles the SQLToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void SQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "SQL";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.SQL);
            }
        }

        /// <summary>
        /// Handles the VisualBasicToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void VisualBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "Visual Basic";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.VB);
            }
        }

        /// <summary>
        /// Handles the XMLToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "XML";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.XML);
            }
        }

        /// <summary>
        /// Handles the StatusBarToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Handles the RefreshToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void RefreshToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Refresh();
                var r = new Range(CurrentTB);
                r.SelectAll();
                CurrentTB.OnSyntaxHighlight(new TextChangedEventArgs(r));
            }
        }

        /// <summary>
        /// The HlCurrentLineToolStripButton_Click
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void HlCurrentLineToolStripButton_Click(object sender, EventArgs e)
        {
            _highlightCurrentLine = _highlightCurrentLine ? false : true;

            HighlightCurrentLine();
        }

        /// <summary>
        /// Handle when the 'Open' submenu clicked in the 'File' menu.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        /// <summary>
        /// The SaveToolStripMenuItem_Click
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                Save(tsFiles.SelectedItem);
            }
        }

        /// <summary>
        /// The SaveAsToolStripMenuItem_Click
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
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
        /// The NewToolStripMenuItem_Click
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTab();
        }

        private void ReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ShowReplaceDialog();
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Copy();
            }
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Redo();
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Cut();
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Paste();
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

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the MainForm_KeyDown event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="KeyEventArgs"/></param>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentTB != null)
            {
                if (e.Control && e.KeyCode == Keys.O)
                {
                    if (file_open.ShowDialog() == DialogResult.OK)
                    {
                        CreateTab(file_open.FileName);
                    }
                }
                else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
                {
                    if (tsFiles.SelectedItem != null)
                    {
                        Save(tsFiles.SelectedItem);
                    }
                }
                else if (e.Control && e.Shift && e.KeyCode == Keys.L)
                {
                    CurrentTB.ClearCurrentLine();
                }
                else if (e.Control && e.Shift && e.KeyCode == Keys.Oem2 && CurrentTB.CommentPrefix != null)
                {
                    if (!CurrentTB.SelectedText.Contains(CurrentTB.CommentPrefix))
                    {
                        CurrentTB.InsertLinePrefix(CurrentTB.CommentPrefix);
                    }
                    else
                    {
                        CurrentTB.RemoveLinePrefix(CurrentTB.CommentPrefix);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the TsFiles_TabStripItemSelectionChanged event.
        /// </summary>
        /// <param name="e">Event Arguments<see cref="TabStripItemChangedEventArgs"/></param>
        private void TsFiles_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            UpdateDocumentMap();

            if (CurrentTB != null)
            {
                popupMenu = new AutocompleteMenu(CurrentTB);
                BuildAutocompleteMenu();
                UpdateChangedFlag(CurrentTB.IsChanged);
            }
        }

        private void Tb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            var tb = sender as FastColoredTextBox;
            UpdateChangedFlag(tb.IsChanged);
        }

        /// <summary>
        /// Handles the TsFiles_TabStripItemClosed event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void TsFiles_TabStripItemClosed(object sender, EventArgs e)
        {
            SanitizeTabStrip();
            UpdateDocumentMap();
        }

        /// <summary>
        /// The MainForm_FormClosing
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="FormClosingEventArgs"/></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<FATabStripItem> list = new List<FATabStripItem>();
            foreach (FATabStripItem tab in tsFiles.Items)
            {
                list.Add(tab);
            }
            foreach (FATabStripItem tab in list)
            {
                TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tab);
                TsFiles_TabStripItemClosing(args);
                if (args.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                tsFiles.RemoveTab(tab);
            }
        }

        /// <summary>
        /// The TsFiles_TabStripItemClosing
        /// </summary>
        /// <param name="e">Event Arguments<see cref="TabStripItemClosingEventArgs"/></param>
        private void TsFiles_TabStripItemClosing(TabStripItemClosingEventArgs e)
        {
            if ((e.Item.Controls[0] as FastColoredTextBox).IsChanged)
            {
                switch (MessageBox.Show("Do you want save " + e.Item.Title + " ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case DialogResult.Yes:
                        if (!Save(e.Item))
                        {
                            e.Cancel = true;
                        }
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void SanitizeTabStrip()
        {
            List<FATabStripItem> list = GetTabList();
            if (list.Count == 0)
            {
                CreateTab();
            }
        }

        private bool IsSavedTab(string tagName) => tagName != null;

        private void DiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> filePaths = new List<string>();

            foreach (FATabStripItem item in tsFiles.Items)
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

        #endregion

        #region Document Map Functionality

        /// <summary>
        /// Updates the target of the DocumentMap.
        /// </summary>
        private void UpdateDocumentMap()
        {
            if (CurrentTB != null)
            {
                List<FATabStripItem> list = GetTabList();
                documentMap.Target = list.Count > 0 ? CurrentTB : null;
                documentMap.Visible = _enableDocumentMap ? true : false;
                if (!_enableDocumentMap || documentMap.Target == null)
                {
                    tsFiles.Width = this.Width - 40;
                    documentMap.Visible = false;
                    return;
                }
                tsFiles.Width = documentMap.Left - 23;
            }
        }

        #endregion

        #region Custom Syntax Highlighting

        /// <summary>
        /// Sets the current syntax highlighting setting.
        /// </summary>
        /// <param name="fName">Filename<see cref="string"/></param>
        /// <param name="mainEditor">FastColoredTextBox<see cref="FastColoredTextBox"/></param>
        private void SetCurrentEditorSyntaxHighlight(string fName, FastColoredTextBox mainEditor)
        {
            char[] name = fName.ToCharArray();
            string ext = "";
            int token = fName.Length - 1;

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

            switch (ext)
            {
                case _html:
                    ChangeSyntax(mainEditor, Language.HTML);
                    syntaxLabel.Text = "HTML";
                    break;
                case _xml:
                    ChangeSyntax(mainEditor, Language.XML);
                    syntaxLabel.Text = "XML";
                    break;
                case _javascript:
                    ChangeSyntax(mainEditor, Language.JS);
                    syntaxLabel.Text = "JavaScript";
                    break;
                case _lua:
                    ChangeSyntax(mainEditor, Language.Lua);
                    syntaxLabel.Text = "Lua";
                    break;
                case _csharp:
                    ChangeSyntax(mainEditor, Language.CSharp);
                    syntaxLabel.Text = "C#";
                    break;
                case _sql:
                    ChangeSyntax(mainEditor, Language.SQL);
                    syntaxLabel.Text = "SQL";
                    break;
                case _vb:
                    ChangeSyntax(mainEditor, Language.VB);
                    syntaxLabel.Text = "Visual Basic";
                    break;
                case _vbs:
                    ChangeSyntax(mainEditor, Language.VB);
                    syntaxLabel.Text = "Visual Basic Script";
                    break;
                default:
                    mainEditor.Language = Language.Custom;
                    syntaxLabel.Text = "None";
                    break;
            }
        }

        /// <summary>
        /// Changes the language of the given FastColoredTextBox instance
        /// and clears all styles.
        /// </summary>
        /// <param name="tb">FastColoredTextBox</param>
        /// <param name="language">Language</param>
        public void ChangeSyntax(FastColoredTextBox tb, Language language)
        {
            if (tb == null)
            {
                logger.Log(Resources.InvalidArgument, LoggerMessageType.Error);
                return;
            }

            tb.Range.ClearStyle(StyleIndex.All);
            tb.Language = language;
            Range r = new Range(tb);
            r.SelectAll();
            tb.OnSyntaxHighlight(new TextChangedEventArgs(r));
        }

        #endregion

        #region AutoCompleteMenu Functionality

        private void BuildAutocompleteMenu()
        {
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
                CurrentTB.IsChanged = isChanged;
                saveToolStripButton.Enabled = isChanged;
            }
        }

        #endregion
    }
}
