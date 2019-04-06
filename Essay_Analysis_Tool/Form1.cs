using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private readonly Range _selection;

        public bool UNDO_AVAIALBE = false;
        public bool FIND_FORM_CLOSED = true;
        public bool NEW_FILE = true;
        public bool BATCH_HIGHLIGHTING = false;
        public bool WRAP_SEARCH = false;
        public bool HIGHLIGHT_CURRENT_LINE = true;
        public bool ENABLE_DOCUMENT_MAP = true;
        public String EMPTY_STRING = "";
        
        private const string _html = "html";
        private const string _xml = "xml";
        private const string _javascript = "js";
        private const string _csharp = "cs";
        private const string _lua = "lua";
        private const string _sql = "sql";
        private const string _java = "java";
        private const string _bat = "bat";

        //styles
        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();
        public readonly TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public readonly TextStyle LightBlueStyle = new TextStyle(Brushes.RoyalBlue, null, FontStyle.Regular);
        public readonly TextStyle YellowStyle = new TextStyle(Brushes.YellowGreen, null, FontStyle.Regular);
        public readonly TextStyle RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        public readonly TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        public readonly TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        public readonly TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        public readonly TextStyle GreenStyleItalic = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        public readonly TextStyle BrownStyleItalic = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        public readonly TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        public readonly MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        public readonly Style BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);

        protected Regex JavaAttributeRegex,
                      JavaClassNameRegex;

        protected Regex JavaCommentRegex1,
                      JavaCommentRegex2,
                      JavaCommentRegex3;

        protected Regex JavaKeywordRegex;
        protected Regex JavaNumberRegex;
        protected Regex JavaStringRegex;
        protected Regex HTMLAttrRegex,
                      HTMLAttrValRegex,
                      HTMLCommentRegex1,
                      HTMLCommentRegex2;

        protected Regex HTMLEndTagRegex;

        protected Regex HTMLEntityRegex,
                      HTMLTagContentRegex;

        protected Regex HTMLTagNameRegex;
        protected Regex HTMLTagRegex;

        /// <summary>
        /// Initialzes the Form.
        /// </summary>
        public mainForm()
        {
            InitializeComponent();
        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        private Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));
        
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
                    SetCurrentEditorSyntaxHighlight(fileName, tb);
                    if (tb.Language == Language.Custom && BATCH_HIGHLIGHTING)
                    {
                        tb.OpenFile(fileName);
                        BatchSyntaxHighlight(tb);
                    }
                    else if (tb.Language == Language.Custom && !BATCH_HIGHLIGHTING)
                    {
                        tb.OpenFile(fileName);
                        JavaSyntaxHighlight(tb);
                    }
                    else
                    {
                        tb.OpenFile(fileName);
                    }
                }
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
        
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTab(null);
            NEW_FILE = true;
        }
        
        private void SetCurrentEditorSyntaxHighlight(string fName, FastColoredTextBox mainEditor)
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
                    BATCH_HIGHLIGHTING = true;
                    batchToolStripMenuItem.Checked = true;
                    syntaxLabel.Text = "Batch";
                    break;
                default:
                    mainEditor.Language = Language.Custom;
                    BATCH_HIGHLIGHTING = false;
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
            BATCH_HIGHLIGHTING = false;
            try
            {
                tb.Range.ClearStyle(StyleIndex.All);
                tb.Language = language;
                tb.TextChanged -= tb_TextChanged;
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
        
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                Save(tsFiles.SelectedItem);
            }
        }
        
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
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTB.Undo();
        }
        
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            CreateTab(null);
        }
        
        private void findButton_Click(object sender, EventArgs e)
        {
            ShowFindDialog();
        }
        
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CreateTab(file_open.FileName);
            }
        }

        private void UpdateDocumentMap()
        {
            List<FATabStripItem> list = GetTabList();
            documentMap.Target = list.Count > 0 ? CurrentTB : null;
            documentMap.Visible = ENABLE_DOCUMENT_MAP ? true : false;
            if (!ENABLE_DOCUMENT_MAP || documentMap.Target == null)
            {
                tsFiles.Width = this.Width - 40;
                documentMap.Visible = false;
                return;
            }
            tsFiles.Width = documentMap.Left - 23;
        }
        
        private void closeToolStripButton_Click(object sender, EventArgs e)
        {
            TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tsFiles.SelectedItem);
            tsFiles_TabStripItemClosing(args);
            if (args.Cancel)
            {
                return;
            }
            tsFiles.RemoveTab(tsFiles.SelectedItem);
            UpdateDocumentMap();
        }
        
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentTB.Font = fontDialog.Font;
            }
        }
        
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                Save(tsFiles.SelectedItem);
            }
        }
        
        private void closeAllToolStripButton_Click(object sender, EventArgs e)
        {
            CloseAllTabs();
            UpdateDocumentMap();
        }
        
        private void CloseAllTabs()
        {
            List<FATabStripItem> list = GetTabList();
            foreach (FATabStripItem tab in list)
            {
                TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tab);
                tsFiles_TabStripItemClosing(args);
                if (args.Cancel)
                {
                    return;
                }
                tsFiles.RemoveTab(tab);
            }
        }

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
            BATCH_HIGHLIGHTING = true;
            fctb.TextChanged += new EventHandler<TextChangedEventArgs>(tb_TextChanged);
        }

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

        private void JavaSyntaxHighlight(FastColoredTextBox fctb)
        {
            Range range = fctb.Range;
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);
^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);
";
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
            fctb.TextChanged += new EventHandler<TextChangedEventArgs>(tb_TextChanged);
        }
        
        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Cut();
            }
        }
        
        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Paste();
            }
        }
        
        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Copy();
            }
        }
        
        private void undoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Undo();
            }
        }
        
        private void redoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Redo();
            }
        }
        
        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(2);
            }
        }
        
        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(-2);
            }
        }
        
        private void findToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                ShowFindDialog();
            }
        }
        
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
        
        private void documentMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ENABLE_DOCUMENT_MAP = ENABLE_DOCUMENT_MAP ? false : true;
            UpdateDocumentMap();
        }
        
        private void tsFiles_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            UpdateDocumentMap();
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "C#";
                ChangeSyntax(CurrentTB, Language.CSharp);
            }
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "None";
                ChangeSyntax(CurrentTB, Language.Custom);
            }
        }

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "HTML";
                ChangeSyntax(CurrentTB, Language.HTML);
            }
        }

        private void javaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "JavaScript";
                ChangeSyntax(CurrentTB, Language.JS);
            }
        }

        private void luaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Lua";
                ChangeSyntax(CurrentTB, Language.Lua);
            }
        }

        private void pHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "PHP";
                ChangeSyntax(CurrentTB, Language.PHP);
            }
        }

        private void sQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "SQL";
                ChangeSyntax(CurrentTB, Language.SQL);
            }
        }

        private void visualBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Visual Basic";
                ChangeSyntax(CurrentTB, Language.VB);
            }
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "XML";
                ChangeSyntax(CurrentTB, Language.XML);
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
            if (CurrentTB.Language == Language.Custom && BATCH_HIGHLIGHTING)
            {
                BatchSyntaxHighlight(CurrentTB);
            }
            else if (CurrentTB.Language == Language.Custom)
            {
                JavaSyntaxHighlight(CurrentTB);
            }
        }

        private void javaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Java";
                CurrentTB.Language = Language.Custom;
                JavaSyntaxHighlight(CurrentTB);
            }
        }

        private void batchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                syntaxLabel.Text = "Batch";
                CurrentTB.Language = Language.Custom;
                BatchSyntaxHighlight(CurrentTB);
            }
        }

        private void tsFiles_TabStripItemClosed(object sender, EventArgs e)
        {
            UpdateDocumentMap();
        }

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Refresh();
                CurrentTB.OnTextChanged(CurrentTB.Range);
            }
        }
        
        private void HighlightCurrentLine()
        {
            foreach (FATabStripItem tab in tsFiles.Items)
            {
                if (HIGHLIGHT_CURRENT_LINE)
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
        
        private void hlCurrentLineToolStripButton_Click(object sender, EventArgs e)
        {
            HIGHLIGHT_CURRENT_LINE = HIGHLIGHT_CURRENT_LINE ? false : true;
            HighlightCurrentLine();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<FATabStripItem> list = new List<FATabStripItem>();
            foreach (FATabStripItem tab in tsFiles.Items)
            {
                list.Add(tab);
            }
            foreach (FATabStripItem tab in list)
            {
                TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tab);
                tsFiles_TabStripItemClosing(args);
                if (args.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                tsFiles.RemoveTab(tab);
            }
        }

        private void tsFiles_TabStripItemClosing(TabStripItemClosingEventArgs e)
        {
            if ((e.Item.Controls[0] as FastColoredTextBox).IsChanged)
            {
                switch (MessageBox.Show("Do you want save " + e.Item.Title + " ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case System.Windows.Forms.DialogResult.Yes:
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
    }
}
