using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Essay_Analysis_Tool.Windows
{
    public partial class Editor : DockContent
    {
        //file extensions
        private const string _html = "html";
        private const string _xml = "xml";
        private const string _javascript = "js";
        private const string _csharp = "cs";
        private const string _lua = "lua";
        private const string _sql = "sql";
        private const string _vb = "vb";
        private const string _vbs = "vbs";

        private bool _highlightCurrentLine = true;

        //Styles
        private Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

        //Line Colors
        internal Color currentLineColor = Color.FromArgb(100, 210, 210, 255);
        internal Color changedLineColor = Color.FromArgb(255, 230, 230, 255);

        private MainForm _parent;

        /// <summary>
        /// Public variable to allow other classes to modify
        /// the title of the <see cref="Editor"/>.
        /// </summary>
        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public bool CurrentLineHighlight
        {
            get { return _highlightCurrentLine; }
            set
            {
                _highlightCurrentLine = value;

                if (!value)
                {
                    mainEditor.CurrentLineColor = Color.Transparent;
                }

                mainEditor.CurrentLineColor = currentLineColor;
            }
        }

        /// <summary>
        /// Constructs the <see cref="Editor"/> with default settings.
        /// </summary>
        public Editor(MainForm parent, string filename, Font font)
        {
            _parent = parent;
            InitializeComponent();
            mainEditor.Font = font;
            mainEditor.Dock = DockStyle.Fill;
            mainEditor.BorderStyle = BorderStyle.Fixed3D;
            mainEditor.LeftPadding = 17;
            mainEditor.HighlightingRangeType = HighlightingRangeType.VisibleRange;
            mainEditor.ChangedLineColor = changedLineColor;
            mainEditor.AddStyle(sameWordsStyle);
        }

        /// <summary>
        /// Attempts to save the current file.
        /// </summary>
        /// <returns>A boolean value based on whether or not a successfull save was performed.</returns>
        public bool Save()
        {
            if (Tag == null)
            {
                SaveFileDialog sfdMain = CreateSaveDialog();
                if (sfdMain.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                Title = Path.GetFileName(sfdMain.FileName);
                Tag = sfdMain.FileName;
            }

            string filePath = (string) Tag;

            File.WriteAllText(filePath, mainEditor.Text);
            
            return true;
        }

        public void SetCurrentEditorSyntaxHighlight(string fName)
        {
            if (!fName.Contains("."))
            {
                DetectSyntax("");
                return;
            }
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
            DetectSyntax(ext);
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
        /// Calls the open file function in the FCTB instance.
        /// </summary>
        /// <param name="fileName">filename</param>
        public void OpenFile(string fileName)
        {
            mainEditor.OpenFile(fileName);
        }

        public void ClearCurrentLine()
        {
            mainEditor.ClearCurrentLine();
        }

        private void DetectSyntax(string ext)
        {
            switch (ext)
            {
                case _html:
                    ChangeSyntax(Language.HTML);
                    break;
                case _xml:
                    ChangeSyntax(Language.XML);
                    break;
                case _javascript:
                    ChangeSyntax(Language.JS);
                    break;
                case _lua:
                    ChangeSyntax(Language.Lua);
                    break;
                case _csharp:
                    ChangeSyntax(Language.CSharp);
                    break;
                case _sql:
                    ChangeSyntax(Language.SQL);
                    break;
                case _vb:
                    ChangeSyntax(Language.VB);
                    break;
                case _vbs:
                    ChangeSyntax(Language.VB);
                    break;
                default:
                    mainEditor.Language = Language.Custom;
                    break;
            }
        }

        private SaveFileDialog CreateSaveDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Normal text file (*.txt)|*.txt|"
            + "C# source file (*.cs)" + "|*.cs|"
            + "Hyper Text Markup Language File (*.html)" + "|*.html|"
            + "Javascript source file (*.js)" + "|*.js|"
            + "JSON file (*.json)" + "|*.json|"
            + "Lua source file (*.lua)" + "|*.lua|"
            + "PHP file (*.php)" + "|*.php|"
            + "Structured Query Language file (*.sql)" + "|*.sql|"
            + "Visual Basic file (*.vb)" + "|*.vb|"
            + "VBScript file (*.vbs)" + "|*.vbs|"
            + "All files (*.*)" + "|*.*";
            return dialog;
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.CloseTab(this);
        }
    }
}
