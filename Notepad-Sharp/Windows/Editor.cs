using NotepadSharp.Utils;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NotepadSharp.Windows
{
    /// <summary>
    /// This class is a DockContent wrapper for the <see cref="FastColoredTextBoxNS.FastColoredTextBox"/>.
    /// This wrapper allows the <see cref="FastColoredTextBoxNS.FastColoredTextBox"/> to be dockable.
    /// </summary>
    public partial class Editor : DockContent
    {
        private MainForm _parent;
        private string _syntaxLabelText;
        private bool _isUntitled;

        #region Event Declarations

        [System.ComponentModel.Description("Occurs when a file is saved")]
        public event System.EventHandler<FastColoredTextBoxNS.FileSavedEventArgs> FileSaved;

        #endregion

        #region Public variables

        /// <summary>
        /// Public variable to allow modification of the visible title of the <see cref="Editor"/>.
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

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the <see cref="Editor"/>.
        /// </summary>
        public Editor(MainForm parent)
        {
            _parent = parent;
            InitializeComponent();
            ApplySettings();

            mainEditor.FileSaved += new System.EventHandler<FastColoredTextBoxNS.FileSavedEventArgs>(Editor_FileSaved);
        }

        /// <summary>
        /// Constructs the <see cref="Editor"/>.
        /// </summary>
        /// <param name="parent">Parent Control</param>
        /// <param name="fn">Filename</param>
        /// <param name="title">Form Text</param>
        public Editor(MainForm parent, string fn, string title)
        {
            _parent = parent;
            InitializeComponent();
            ApplySettings();
            Tag = fn;
            mainEditor.Tag = fn;
            this.Title = title;

            mainEditor.FileSaved += new System.EventHandler<FastColoredTextBoxNS.FileSavedEventArgs>(Editor_FileSaved);
        }

        #endregion

        #region Editor Form Behavior

        /// <summary>
        /// Attempts to save the current file.
        /// Sets tab text to filename if save is successful.
        /// </summary>
        /// <returns>A boolean value based on whether or not a successfull save was performed.</returns>
        public bool Save()
        {
            if (mainEditor.Save(mainEditor.Text))
            {
                Tag = mainEditor.Tag;
                Title = Path.GetFileName((string)Tag);
            }

            DetectSyntax(Path.GetExtension((string)Tag));

            return true;
        }

        /// <summary>
        /// Changes the language of the given FastColoredTextBox instance
        /// and clears all styles.
        /// </summary>
        /// <param name="tb">FastColoredTextBox</param>
        /// <param name="language">Language</param>
        public void ChangeSyntax(FastColoredTextBoxNS.Language language)
        {
            mainEditor.Range.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            mainEditor.Language = language;
            FastColoredTextBoxNS.Range r = new FastColoredTextBoxNS.Range(mainEditor);
            r.SelectAll();
            mainEditor.OnSyntaxHighlight(new FastColoredTextBoxNS.TextChangedEventArgs(r));
        }

        /// <summary>
        /// This method detects the language to be used in the given instance of <see cref="Editor"/> 
        /// by the file extension.
        /// </summary>
        /// <param name="ext">File Extension</param>
        public void DetectSyntax(string ext)
        {
            switch (ext)
            {
                case GlobalConstants.HTML_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.HTML);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_HTML;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_HTML;
                    break;
                case GlobalConstants.XML_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.XML);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_XML;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_XML;
                    break;
                case GlobalConstants.JS_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.JS);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_JS;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_JS;
                    break;
                case GlobalConstants.LUA_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.Lua);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_LUA;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_LUA;
                    break;
                case GlobalConstants.CS_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.CSharp);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_CS;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_CS;
                    break;
                case GlobalConstants.SQL_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.SQL);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_SQL;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_SQL;
                    break;
                case GlobalConstants.VB_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.VB);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_VB;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_VB;
                    break;
                case GlobalConstants.VBS_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.VB);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_VBS;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_VBS;
                    break;
                case GlobalConstants.PHP_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.PHP);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_PHP;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_PHP;
                    break;
                case GlobalConstants.JSON_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.JSON);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_JSON;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_JSON;
                    break;
                case GlobalConstants.BATCH_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.Batch);
                    this._syntaxLabelText = GlobalConstants.STX_TXT_BAT;
                    _parent.SyntaxStatusBarLabelText= GlobalConstants.STX_TXT_BAT;
                    break;
                case GlobalConstants.ASM_EXT:
                    ChangeSyntax(FastColoredTextBoxNS.Language.Assembly);
                    this._syntaxLabelText= GlobalConstants.STX_TXT_ASM;
                    _parent.SyntaxStatusBarLabelText = GlobalConstants.STX_TXT_ASM;
                    break;
                default:
                    ChangeSyntax(FastColoredTextBoxNS.Language.Custom);
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

        #endregion

        #region Event Handlers

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.Editor_TabClosing(e, this);
        }

        private void Editor_FileSaved(object sender, FastColoredTextBoxNS.FileSavedEventArgs e)
        {
            if (!e.IsSaveSuccessful)
            {
                return;
            }

            Tag = mainEditor.Tag;
            Title = Path.GetFileName((string)Tag);

            DetectSyntax(Path.GetExtension((string)Tag));

            _parent.MainForm_FileSaved(sender, e);
        }

        #endregion
    }
}
