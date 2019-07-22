namespace Essay_Analysis_Tool
{
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

        //Form declarations
        internal FindForm findForm;

        //Line Colors
        internal Color currentLineColor = Color.FromArgb(100, 210, 210, 255);
        internal Color changedLineColor = Color.FromArgb(255, 230, 230, 255);

        //General variable declarations and definitions
        private readonly Range _selection;
        private bool _batchHighlighting = false;
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

        //Application Version
        private const string Version = "v1.0.0.SNAPSHOT";

        //file types
        private const string _fthtml = "Hyper Text Markup Language File (*.html)";
        private const string _ftxml = "Extensible Markup Language file (.xml)";
        private const string _ftjs = "Javascript source file (.js)";
        private const string _ftcsharp = "C# source file (.cs)";
        private const string _ftlua = "Lua source file (.lua)";
        private const string _ftsql = "Structured Query Language file (.sql)";
        private const string _ftjava = "Java source file (.java)";
        private const string _ftbatch = "Windows Batch file (.bat)";

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
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="mainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            sfdMain.Filter = "Normal text file (*.txt)|*.txt|C# source file (*.cs)|*.cs|Hyper Text Markup Language file (*.html)|" +
                "*.html|All files (*.*)|*.*|Java source file (*.java)|*.java|JavaScript file (*.js)|*.js|JSON file (*.json)|*.json|" +
                "Lua source file (*.lua)|*.lua|PHP file (*.php)|*.php|Structured Query Language file (*.sql)|*.sql|" +
                "Visual Basic file (*.vb)|*.vb";
        }

        #region Tab Functionality
        /// <summary>
        /// The CreateTab
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/></param>
        private void CreateTab(string fileName)
        {
            try
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
                    if (tb.Language == Language.Custom && _batchHighlighting)
                    {
                        tb.OpenFile(fileName);
                        BatchSyntaxHighlight(tb);
                    }
                    else if (tb.Language == Language.Custom && !_batchHighlighting)
                    {
                        tb.OpenFile(fileName);
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
                
                tsFiles.AddTab(tab);
                tsFiles.SelectedItem = tab;
                tb.Focus();
                tb.ChangedLineColor = changedLineColor;
                AutocompleteMenu popupMenu = new AutocompleteMenu(tb);
                UpdateDocumentMap();
                HighlightCurrentLine();
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
        /// The CloseAllTabs
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
        /// <param name="tab">The tab<see cref="FATabStripItem"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool Save(FATabStripItem tab)
        {
            var tb = (tab.Controls[0] as FastColoredTextBox);
            if (tab.Tag == null)
            {
                if (sfdMain.ShowDialog() != DialogResult.OK)
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

        #region Find Dialog Functionality
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
        /// <param name="findText">The findText<see cref="string"/></param>
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
            CurrentTB.Undo();
        }

        /// <summary>
        /// Handles the NewToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            CreateTab(null);
        }

        /// <summary>
        /// Handles the FindButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void FindButton_Click(object sender, EventArgs e)
        {
            ShowFindDialog();
        }

        /// <summary>
        /// Handles the OpenToolStripButton_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tsFiles.SelectedItem);
            TsFiles_TabStripItemClosing(args);
            if (args.Cancel)
            {
                return;
            }
            tsFiles.RemoveTab(tsFiles.SelectedItem);
            UpdateDocumentMap();
        }

        /// <summary>
        /// Hanldes the FontToolStripMenuItem_Click event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
                ShowFindDialog();
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "C#";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "None";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "HTML";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "JavaScript";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Lua";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "PHP";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "SQL";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Visual Basic";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "XML";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Java";
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
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Batch";
                CurrentTB.Language = Language.Custom;
                BatchSyntaxHighlight(CurrentTB);
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
            CreateTab(null);
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
        /// Handles the TsFiles_TabStripItemSelectionChanged event.
        /// </summary>
        /// <param name="e">Event Arguments<see cref="TabStripItemChangedEventArgs"/></param>
        private void TsFiles_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            UpdateDocumentMap();
        }

        /// <summary>
        /// Handles the Tb_TextChanged event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="TextChangedEventArgs"/></param>
        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentTB.Language == Language.Custom && _batchHighlighting)
            {
                BatchSyntaxHighlight(CurrentTB);
            }
            else if (CurrentTB.Language == Language.Custom)
            {
                JavaSyntaxHighlight(CurrentTB);
            }
        }

        /// <summary>
        /// Handles the TsFiles_TabStripItemClosed event.
        /// </summary>
        /// <param name="sender">Sender Object<see cref="object"/></param>
        /// <param name="e">Event Arguments<see cref="EventArgs"/></param>
        private void TsFiles_TabStripItemClosed(object sender, EventArgs e)
        {
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

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = "About Notepad#";
            string message = "Created by: Zachary Pedigo\nVersion: " + Version + "\n" + "Date: " + DateTime.Now + "\n" + "OS: "
                + Environment.OSVersion + "\nLicense: GNU General Public License v3.0";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Document Map Functionality
        /// <summary>
        /// Updates the target of the DocumentMap.
        /// </summary>
        private void UpdateDocumentMap()
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
        #endregion

        #region Custom Syntax Highlighting
        /// <summary>
        /// The BatchSyntaxHighlight
        /// </summary>
        /// <param name="fctb">The fctb<see cref="FastColoredTextBox"/></param>
        private void BatchSyntaxHighlight(FastColoredTextBox fctb)
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
            _batchHighlighting = true;
            fctb.TextChanged += new EventHandler<TextChangedEventArgs>(Tb_TextChanged);
        }

        /// <summary>
        /// The JavaSyntaxHighlight
        /// </summary>
        /// <param name="fctb">The fctb<see cref="FastColoredTextBox"/></param>
        private void JavaSyntaxHighlight(FastColoredTextBox fctb)
        {
            Range range = fctb.Range;
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns = @"^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);";
            //clear style of changed range
            range.ClearStyle(BrownStyle, GreenStyleItalic, MagentaStyle, BoldStyle, BlueStyle, GreenStyleItalic);
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
            fctb.TextChanged += new EventHandler<TextChangedEventArgs>(Tb_TextChanged);
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
                    syntaxLabel.Text = "Java";
                    break;
                case _bat:
                    mainEditor.Language = Language.Custom;
                    _batchHighlighting = true;
                    batchToolStripMenuItem.Checked = true;
                    syntaxLabel.Text = "Batch";
                    break;
                default:
                    mainEditor.Language = Language.Custom;
                    _batchHighlighting = false;
                    syntaxLabel.Text = "None";
                    break;
            }
        }

        /// <summary>
        /// Changes the language of the given FastColoredTextBox instacnce
        /// and clears all styles.
        /// </summary>
        /// <param name="tb">FastColoredTextBox</param>
        /// <param name="language">Language</param>
        public void ChangeSyntax(FastColoredTextBox tb, Language language)
        {
            _batchHighlighting = false;
            try
            {
                tb.Range.ClearStyle(StyleIndex.All);
                tb.Language = language;
                tb.TextChanged -= Tb_TextChanged;
                tb.OnTextChanged();
            }
            catch (Exception e)
            {
                if (MessageBox.Show(e.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                {
                    ChangeSyntax(tb, language);
                }
            }
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
    }
}
