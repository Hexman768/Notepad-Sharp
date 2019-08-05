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
        private bool _javaHighlighting = false;
        private bool _highlightCurrentLine = true;
        private bool _enableDocumentMap = true;

        //file extensions
        private const string _html = "html";
        private const string _xml = "xml";
        private const string _javascript = "js";
        private const string _csharp = "cs";
        private const string _lua = "lua";
        private const string _sql = "sql";
        private const string _java = "java";
        private const string _bat = "bat";

        /// <summary>
        /// Defines the Platform Type.
        /// </summary>
        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();

        //Styles
        private readonly TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private readonly TextStyle LightBlueStyle = new TextStyle(Brushes.RoyalBlue, null, FontStyle.Regular);
        private readonly TextStyle YellowStyle = new TextStyle(Brushes.YellowGreen, null, FontStyle.Regular);
        private readonly TextStyle RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        private readonly TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        private readonly TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        private readonly TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        private readonly TextStyle GreenStyleItalic = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly TextStyle BrownStyleItalic = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        private readonly TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        private readonly MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        private readonly Style BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        private Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

        /// <summary>
        /// Defines the JavaAttributeRegex and JavaClassNameRegex.
        /// </summary>
        protected Regex JavaAttributeRegex,
                      JavaClassNameRegex;

        /// <summary>
        /// Defines the JavaCommentRegex1, JavaCommentRegex2 and JavaCommentRegex3.
        /// </summary>
        protected Regex JavaCommentRegex1,
                      JavaCommentRegex2,
                      JavaCommentRegex3;

        /// <summary>
        /// Defines the JavaKeywordRegex.
        /// </summary>
        protected Regex JavaKeywordRegex;

        /// <summary>
        /// Defines the JavaNumberRegex.
        /// </summary>
        protected Regex JavaNumberRegex;

        /// <summary>
        /// Defines the JavaStringRegex.
        /// </summary>
        protected Regex JavaStringRegex;

        /// <summary>
        /// Defines the HTMLAttrRegex, HTMLAttrValRegex, HTMLCommentRegex1 and HTMLCommentRegex2.
        /// </summary>
        protected Regex HTMLAttrRegex,
                      HTMLAttrValRegex,
                      HTMLCommentRegex1,
                      HTMLCommentRegex2;

        /// <summary>
        /// Defines the HTMLEndTagRegex.
        /// </summary>
        protected Regex HTMLEndTagRegex;

        /// <summary>
        /// Defines the HTMLEntityRegex and HTMLTagContentRegex.
        /// </summary>
        protected Regex HTMLEntityRegex,
                      HTMLTagContentRegex;

        /// <summary>
        /// Defines the HTMLTagNameRegex.
        /// </summary>
        protected Regex HTMLTagNameRegex;

        /// <summary>
        /// Defines the HTMLTagRegex.
        /// </summary>
        protected Regex HTMLTagRegex;

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

        /// <summary>
        /// Determines whether or not to use the RegexOptions Compiled option based on the platform.
        /// </summary>
        public static RegexOptions RegexCompiledOption
        {
            get
            {
                if (platformType == Platform.X86)
                    return RegexOptions.Compiled;
                else
                    return RegexOptions.None;
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
                + Resources.BatchFileType + "|*.bat|"
                + Resources.CSharpFileType + "|*.cs|"
                + Resources.HTMLFileType + "|*.html|"
                + Resources.JavaFileType + "|*.java|"
                + Resources.JSFileType + "|*.js|"
                + Resources.JSONFileType + "|*.json|"
                + Resources.LuaFileType + "|*.lua|"
                + Resources.PHPFileType + "|*.php|"
                + Resources.SQLFileType + "|*.sql|"
                + Resources.VBFileType + "|*.vb|"
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

            if (fileName != null)
            {
                SetCurrentEditorSyntaxHighlight(fileName, tb);
                if (tb.Language == Language.Custom && _javaHighlighting)
                {
                    tb.OpenFile(fileName);
                    tb.TextChanged += new EventHandler<TextChangedEventArgs>(Tb_TextChanged);
                    JavaSyntaxHighlight(tb);
                }
                else
                {
                    tb.OpenFile(fileName);
                }
            }

            var tab = new FATabStripItem(fileName != null ? Path.GetFileName(fileName) : "[new]", tb)
            {
                Tag = fileName
            };

            //create autocomplete popup menu
            popupMenu = new AutocompleteMenu(tb);
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
            popupMenu.AllowTabKey = true;

            tsFiles.AddTab(tab);
            tsFiles.SelectedItem = tab;
            tb.Focus();
            tb.ChangedLineColor = changedLineColor;
            tb.KeyDown += new KeyEventHandler(MainForm_KeyDown);
            UpdateDocumentMap();
            HighlightCurrentLine();
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

            var tb = (tab.Controls[0] as FastColoredTextBox);

            if (tab.Tag == null)
            {
                if (sfdMain.ShowDialog() != DialogResult.OK)
                {
                    return true;
                }
                tab.Title = Path.GetFileName(sfdMain.FileName);
                tab.Tag = sfdMain.FileName;
            }

            File.WriteAllText(tab.Tag as string, tb.Text);
            tb.IsChanged = false;

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
        /// Handles the JavaToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void JavaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "Java";

            if (CurrentTB != null)
            {
                CurrentTB.Language = Language.Custom;

                JavaSyntaxHighlight(CurrentTB);
            }
        }

        /// <summary>
        /// Handles the BatchToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void BatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "Batch";

            if (CurrentTB != null)
            {
                ChangeSyntax(CurrentTB, Language.Batch);
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
                CurrentTB.OnTextChanged(CurrentTB.Range);
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
        /// The OpenToolStripMenuItem_Click
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTab(file_open.FileName);
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
                else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control && CurrentTB != null)
                {
                    if (tsFiles.SelectedItem != null)
                    {
                        Save(tsFiles.SelectedItem);
                    }
                }
                else if (e.Control && e.Shift && e.KeyCode == Keys.Back && CurrentTB != null)
                {
                    CurrentTB.ClearCurrentLine();
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
            }
        }

        /// <summary>
        /// Handles the Tb_TextChanged event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="TextChangedEventArgs"/></param>
        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentTB != null)
            {
                if (CurrentTB.Language == Language.Custom && _javaHighlighting)
                {
                    JavaSyntaxHighlight(CurrentTB);
                }
            }
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
        /// This function handles the syntax highlighting for the Java language.
        /// </summary>
        /// <param name="fctb">The fctb<see cref="FastColoredTextBox"/></param>
        private void JavaSyntaxHighlight(FastColoredTextBox fctb)
        {
            if (fctb == null)
            {
                logger.Log(Resources.NullFastColoredTextBox, LoggerMessageType.Error);
                return;
            }

            Range range = fctb.Range;
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns = @"^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);";
            //clear style of changed range
            range.ClearStyle(BrownStyle, GreenStyleItalic, MagentaStyle, BoldStyle, BlueStyle);
            //
            if (JavaStringRegex == null)
                InitJavaRegex();
            //string highlighting
            range.SetStyle(BrownStyle, JavaStringRegex);
            //comment highlighting
            range.SetStyle(GreenStyleItalic, JavaCommentRegex1);
            range.SetStyle(GreenStyleItalic, JavaCommentRegex2);
            range.SetStyle(GreenStyleItalic, JavaCommentRegex3);
            //number highlighting
            range.SetStyle(MagentaStyle, JavaStringRegex);
            //attribute highlighting
            range.SetStyle(GreenStyleItalic, JavaAttributeRegex);
            //class name highlighting
            range.SetStyle(BoldStyle, JavaClassNameRegex);
            //keyword highlighting
            range.SetStyle(BlueStyle, JavaKeywordRegex);

            //find document comments
            foreach (Range r in range.GetRanges(@"^\s*///.*$", RegexOptions.Multiline))
            {
                //remove C# highlighting from this fragment
                r.ClearStyle(StyleIndex.All);
                //do XML highlighting
                if (HTMLTagRegex == null)
                    InitHTMLRegex();
                //
                r.SetStyle(GreenStyleItalic);
                //tags
                foreach (Range rr in r.GetRanges(HTMLTagContentRegex))
                {
                    rr.ClearStyle(StyleIndex.All);
                    rr.SetStyle(GrayStyle);
                }
                //prefix '///'
                foreach (Range rr in r.GetRanges(@"^\s*///", RegexOptions.Multiline))
                {
                    rr.ClearStyle(StyleIndex.All);
                    rr.SetStyle(GrayStyle);
                }
            }

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"#region\b", @"#endregion\b"); //allow to collapse #region blocks
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
            _javaHighlighting = true;
        }

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
                case _java:
                    mainEditor.Language = Language.Custom;
                    _javaHighlighting = true;
                    syntaxLabel.Text = "Java";
                    break;
                case _bat:
                    ChangeSyntax(mainEditor, Language.Batch);
                    syntaxLabel.Text = "Batch";
                    break;
                default:
                    mainEditor.Language = Language.Custom;
                    _javaHighlighting = false;
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
            tb.OnTextChanged();
        }

        #endregion

        #region Regex Initializers

        /// <summary>
        /// The InitHTMLRegex
        /// </summary>
        protected void InitHTMLRegex()
        {
            HTMLCommentRegex1 = new Regex(@"(<!--.*?-->)|(<!--.*)", RegexOptions.Singleline | RegexCompiledOption);
            HTMLCommentRegex2 = new Regex(@"(<!--.*?-->)|(.*-->)",
                                          RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            HTMLTagRegex = new Regex(@"<|/>|</|>", RegexCompiledOption);
            HTMLTagNameRegex = new Regex(@"<(?<range>[!\w:]+)", RegexCompiledOption);
            HTMLEndTagRegex = new Regex(@"</(?<range>[\w:]+)>", RegexCompiledOption);
            HTMLTagContentRegex = new Regex(@"<[^>]+>", RegexCompiledOption);
            HTMLAttrRegex =
                new Regex(
                    @"(?<range>[\w\d\-]{1,20}?)='[^']*'|(?<range>[\w\d\-]{1,20})=""[^""]*""|(?<range>[\w\d\-]{1,20})=[\w\d\-]{1,20}",
                    RegexCompiledOption);
            HTMLAttrValRegex =
                new Regex(
                    @"[\w\d\-]{1,20}?=(?<range>'[^']*')|[\w\d\-]{1,20}=(?<range>""[^""]*"")|[\w\d\-]{1,20}=(?<range>[\w\d\-]{1,20})",
                    RegexCompiledOption);
            HTMLEntityRegex = new Regex(@"\&(amp|gt|lt|nbsp|quot|apos|copy|reg|#[0-9]{1,8}|#x[0-9a-f]{1,8});",
                                        RegexCompiledOption | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// The InitJavaRegex
        /// </summary>
        protected void InitJavaRegex()
        {
            JavaStringRegex =
                new Regex(
                    @"
                            # Character definitions:
                            '
                            (?> # disable backtracking
                              (?:
                                \\[^\r\n]|    # escaped meta char
                                [^'\r\n]      # any character except '
                              )*
                            )
                            '?
                            |
                            # Normal string & verbatim strings definitions:
                            (?<verbatimIdentifier>@)?         # this group matches if it is an verbatim string
                            ""
                            (?> # disable backtracking
                              (?:
                                # match and consume an escaped character including escaped double quote ("") char
                                (?(verbatimIdentifier)        # if it is a verbatim string ...
                                  """"|                         #   then: only match an escaped double quote ("") char
                                  \\.                         #   else: match an escaped sequence
                                )
                                | # OR
            
                                # match any char except double quote char ("")
                                [^""]
                              )*
                            )
                            ""
                        ",
                    RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace |
                    RegexCompiledOption
                    ); //thanks to rittergig for this regex

            JavaCommentRegex1 = new Regex(@"//.*$", RegexOptions.Multiline | RegexCompiledOption);
            JavaCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            JavaCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)",
                                            RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            JavaNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b",
                                          RegexCompiledOption);
            JavaAttributeRegex = new Regex(@"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline | RegexCompiledOption);
            JavaClassNameRegex = new Regex(@"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b", RegexCompiledOption);
            JavaKeywordRegex =
                new Regex(
                    @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|super|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|import|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|package|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b",
                    RegexCompiledOption);
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
    }
}
