using FastColoredTextBoxNS;
using NotepadSharp.Utils;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NotepadSharp.Windows
{
    public partial class Editor : DockContent
    {
        private MainForm _parent;
        private string _syntaxLabelText;
        private bool _isUntitled;

        /// <summary>
        /// Public variable to allow other classes to modify
        /// the title of the <see cref="Editor"/>.
        /// </summary>
        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public string SyntaxText
        {
            get
            {
                return _syntaxLabelText;
            }
            set
            {
                this._syntaxLabelText = value ?? this._syntaxLabelText;
            }
        }

        public bool IsUntitled
        {
            get
            {
                return _isUntitled;
            }
            set
            {
                _isUntitled = value;
            }
        }

        /// <summary>
        /// Constructs the <see cref="Editor"/>.
        /// </summary>
        public Editor(MainForm parent)
        {
            _parent = parent;
            InitializeComponent();
            ApplySettings();
        }

        /// <summary>
        /// Constructs the <see cref="Editor"/>.
        /// </summary>
        /// <param name="parent">
        /// Parent Control
        /// </param>
        /// <param name="fn">
        /// File Name
        /// </param>
        /// <param name="title">
        /// Title
        /// </param>
        public Editor(MainForm parent, string fn, string title)
        {
            _parent = parent;
            InitializeComponent();
            ApplySettings();
            Tag = fn;
            this.Title = title;
        }

        /// <summary>
        /// Attempts to save the current file.
        /// </summary>
        /// <returns>A boolean value based on whether or not a successfull save was performed.</returns>
        public bool Save()
        {
            if (Tag == null)
            {
                SaveFileDialog sfdMain = Utility.CreateSaveDialog();
                if (sfdMain.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                Title = Path.GetFileName(sfdMain.FileName);
                Tag = sfdMain.FileName;
            }

            string filePath = (string)Tag;

            File.WriteAllText(filePath, mainEditor.Text);

            return true;
        }

        /// <summary>
        /// Changes the language of the given FastColoredTextBox instance
        /// and clears all styles.
        /// </summary>
        /// <param name="tb">FastColoredTextBox</param>
        /// <param name="language">Language</param>
        public void ChangeSyntax(Language language)
        {
            mainEditor.Range.ClearStyle(StyleIndex.All);
            mainEditor.Language = language;
            Range r = new Range(mainEditor);
            r.SelectAll();
            mainEditor.OnSyntaxHighlight(new TextChangedEventArgs(r));
        }

        /// <summary>
        /// This method detects the language to be used in the given instance of <see cref="Editor"/> 
        /// by the file extension.
        /// </summary>
        /// <param name="ext">File Extension</param>
        /// <param name="tab">Tab to Update</param>
        public void DetectSyntax(string ext, Editor tab)
        {
            switch (ext)
            {
                case GlobalConstants.HTML_EXT:
                    tab.ChangeSyntax(Language.HTML);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_HTML;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_HTML;
                    break;
                case GlobalConstants.XML_EXT:
                    tab.ChangeSyntax(Language.XML);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_XML;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_XML;
                    break;
                case GlobalConstants.JS_EXT:
                    tab.ChangeSyntax(Language.JS);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_JS;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_JS;
                    break;
                case GlobalConstants.LUA_EXT:
                    tab.ChangeSyntax(Language.Lua);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_LUA;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_LUA;
                    break;
                case GlobalConstants.CS_EXT:
                    tab.ChangeSyntax(Language.CSharp);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_CS;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_CS;
                    break;
                case GlobalConstants.SQL_EXT:
                    tab.ChangeSyntax(Language.SQL);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_SQL;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_SQL;
                    break;
                case GlobalConstants.VB_EXT:
                    tab.ChangeSyntax(Language.VB);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_VB;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_VB;
                    break;
                case GlobalConstants.VBS_EXT:
                    tab.ChangeSyntax(Language.VB);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_VBS;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_VBS;
                    break;
                case GlobalConstants.PHP_EXT:
                    tab.ChangeSyntax(Language.PHP);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_PHP;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_PHP;
                    break;
                case GlobalConstants.JSON_EXT:
                    tab.ChangeSyntax(Language.JSON);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_JSON;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_JSON;
                    break;
                default:
                    tab.ChangeSyntax(Language.Custom);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_TXT;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_TXT;
                    break;
            }
        }

        public void ApplySettings()
        {
            this.mainEditor.Font = new Font(EditorSettings.Font.FontFamily.Name, EditorSettings.Font.Size);
            this.mainEditor.Dock = EditorSettings.DockStyle;
            this.mainEditor.BorderStyle = EditorSettings.BorderStyle;
            this.mainEditor.LeftPadding = EditorSettings.LeftPadding;
            this.mainEditor.HighlightingRangeType = EditorSettings.HighlightingRangeType;
            this.mainEditor.ChangedLineColor = EditorSettings.ChangedLineColor;
            this.mainEditor.AddStyle(EditorSettings.SameWordsStyle);
        }

        public void HighlightCurrentLine(bool enabled)
        {
            this.mainEditor.CurrentLineColor = enabled ? EditorSettings.CurrentLineColor : Color.Transparent;
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.Editor_TabClosing(e, this);
        }
    }
}
