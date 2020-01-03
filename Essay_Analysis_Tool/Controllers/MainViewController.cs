using Essay_Analysis_Tool.Business;
using Essay_Analysis_Tool.Models;
using Essay_Analysis_Tool.Properties;
using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Essay_Analysis_Tool.Controllers
{
    public class MainViewController
    {
        #region Variables

        readonly string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
        readonly string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
        readonly string[] snippets = { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(^;;)\n{\n;\n}", "while(^)\n{\n;\n}", "do${\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
        readonly string[] declarationSnippets = {
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
               };

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
        private readonly Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));
        private readonly LoggerViewController _loggerController;
        private readonly LoggingService _loggingService;
        private readonly MainView _view;

        #endregion

        #region Constructor

        public MainViewController(LoggerViewController loggerController, LoggingService loggingService)
        {
            _loggerController = loggerController;
            _loggingService = loggingService;
            _view = new MainView(this);
            _view.DefaultBehavior();
        }

        #endregion

        #region Controller Functionality

        /// <summary>
        /// Creates a new tab with read and write access to a specified file
        /// directed at by the path contained in the fileName.
        /// </summary>
        /// <param name="fileName">Name and Path of the file.</param>
        public void CreateTab(string fileName)
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
            tb = _view.UpdateUIPropertiesForFCTB(tb);

            var tab = new FATabStripItem(fileName != null ? Path.GetFileName(fileName) : "[new]", tb)
            {
                Tag = fileName
            };

            if (fileName != null && !_view.IsFileAlreadyOpen(fileName))
            {
                SetCurrentEditorSyntaxHighlight(fileName, tb);
                tb.OpenFile(fileName);
            }
            else if (fileName != null)
            {
                return;
            }

            //create autocomplete popup menu
            _view.UpdatePopupMenu(tb);

            _view.AddTab(tab);
            _view.UpdateSelectedItem(tab);
            tb.Focus();
            _view.UpdateDocumentMap();
            _view.HighlightCurrentLine();
        }

        public bool Save(FATabStripItem tab)
        {
            if (tab == null)
            {
                //_view.LogError(Resources.NullTabStripItem);
                MessageBox.Show("Save Unsucessfull.", "Save not complete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            FastColoredTextBox tb = tab.Controls[0] as FastColoredTextBox;

            if (tab.Tag == null)
            {
                tab = _view.CallSaveFileDialog(tab);
                if (tab == null)
                {
                    return true;
                }
            }

            string filePath = (string)tab.Tag;

            SetCurrentEditorSyntaxHighlight(filePath, tb);

            File.WriteAllText(filePath, tb.Text);
            _view.UpdateChangedFlag(false);

            return true;
        }

        public void SetCurrentEditorSyntaxHighlight(string fName, FastColoredTextBox mainEditor)
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
                    break;
                case _xml:
                    ChangeSyntax(mainEditor, Language.XML);
                    break;
                case _javascript:
                    ChangeSyntax(mainEditor, Language.JS);
                    break;
                case _lua:
                    ChangeSyntax(mainEditor, Language.Lua);
                    break;
                case _csharp:
                    ChangeSyntax(mainEditor, Language.CSharp);
                    break;
                case _sql:
                    ChangeSyntax(mainEditor, Language.SQL);
                    break;
                case _vb:
                    ChangeSyntax(mainEditor, Language.VB);
                    break;
                case _vbs:
                    ChangeSyntax(mainEditor, Language.VB);
                    break;
                default:
                    mainEditor.Language = Language.Custom;
                    break;
            }
        }

        public void ChangeSyntax(FastColoredTextBox tb, Language language)
        {
            if (tb == null)
            {
                _loggingService.Add(new ErrorLogEntry(Resources.InvalidArgument));
                return;
            }

            tb.Range.ClearStyle(StyleIndex.All);
            tb.Language = language;
            Range r = new Range(tb);
            r.SelectAll();
            tb.OnSyntaxHighlight(new TextChangedEventArgs(r));
        }

        public void BuildAutocompleteMenu()
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
            _view.UpdateAutocompleteItems(items);
        }

        public void Show()
        {
            _view.ShowDialog();
        }

        public void ShowLogs()
        {
            _loggerController.Show();
        }

        #endregion
    }
}
