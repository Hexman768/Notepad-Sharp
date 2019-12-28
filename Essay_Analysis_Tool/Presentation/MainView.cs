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

using Essay_Analysis_Tool.Business;
using Essay_Analysis_Tool.Interface;
using Essay_Analysis_Tool.Properties;
using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Essay_Analysis_Tool
{
    /// <summary>
    /// Defines the <see cref="MainView" />
    /// </summary>
    public partial class MainView : Form, IMainView
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
        private bool _highlightCurrentLine = true;
        private bool _enableDocumentMap = true;

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

        

        AutocompleteMenu popupMenu;

        private MainViewController _controller;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="mainForm"/> class.
        /// </summary>
        public MainView()
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
                CurrentTB.Undo();
            }
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            _controller.CreateTab(null);
        }

        private void FindButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ShowFindDialog();
            }
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == DialogResult.OK)
            {
                _controller.CreateTab(file_open.FileName);
            }
        }

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

        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null && fontDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentTB.Font = fontDialog.Font;
            }
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                _controller.Save(tsFiles.SelectedItem);
            }
        }

        private void CloseAllToolStripButton_Click(object sender, EventArgs e)
        {
            CloseAllTabs();
            UpdateDocumentMap();
        }

        private void CutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Cut();
            }
        }

        private void PasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Paste();
            }
        }

        private void CopyToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Copy();
            }
        }

        private void UndoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Undo();
            }
        }

        private void RedoToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.Redo();
            }
        }

        private void ZoomInToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(2);
            }
        }

        private void ZoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ChangeFontSize(-2);
            }
        }

        private void FindToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                CurrentTB.ShowFindDialog();
            }
        }

        private void DocumentMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enableDocumentMap = _enableDocumentMap ? false : true;

            UpdateDocumentMap();
        }

        private void CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "C#";

            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.CSharp);
            }
        }

        private void NoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.Custom);
            }
        }

        private void HTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.HTML);
            }
        }

        private void JavaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.JS);
            }
        }

        private void LuaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "Lua";

            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.Lua);
            }
        }

        private void PHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "PHP";

            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.PHP);
            }
        }

        private void SQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "SQL";

            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.SQL);
            }
        }

        private void VisualBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "Visual Basic";

            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.VB);
            }
        }

        private void XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            syntaxLabel.Text = "XML";

            if (CurrentTB != null)
            {
                _controller.ChangeSyntax(CurrentTB, Language.XML);
            }
        }

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

        private void HlCurrentLineToolStripButton_Click(object sender, EventArgs e)
        {
            _highlightCurrentLine = _highlightCurrentLine ? false : true;

            HighlightCurrentLine();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file_open.ShowDialog() == DialogResult.OK)
            {
                _controller.CreateTab(file_open.FileName);
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                _controller.Save(tsFiles.SelectedItem);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles.SelectedItem != null)
            {
                string oldFile = tsFiles.SelectedItem.Tag as string;
                tsFiles.SelectedItem.Tag = null;
                if (!_controller.Save(tsFiles.SelectedItem))
                {
                    if (oldFile != null)
                    {
                        tsFiles.SelectedItem.Tag = oldFile;
                        tsFiles.SelectedItem.Title = Path.GetFileName(oldFile);
                    }
                }
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.CreateTab(null);
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

            if (filePaths.Count == 1)
                new DiffViewerForm(filePaths[0]).Show();
            else if (filePaths.Count == 2)
                new DiffViewerForm(filePaths[0], filePaths[1]).Show();
            else
                new DiffViewerForm().Show();
        }

        #endregion

        #region Event Handlers

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentTB != null)
            {
                if (e.Control && e.KeyCode == Keys.O)
                {
                    if (file_open.ShowDialog() == DialogResult.OK)
                    {
                        _controller.CreateTab(file_open.FileName);
                    }
                }
                else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
                {
                    if (tsFiles.SelectedItem != null)
                    {
                        _controller.Save(tsFiles.SelectedItem);
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

        private void TsFiles_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            UpdateDocumentMap();

            if (CurrentTB != null)
            {
                popupMenu = new AutocompleteMenu(CurrentTB);
                _controller.BuildAutocompleteMenu();
                UpdateChangedFlag(CurrentTB.IsChanged);
            }
        }

        private void Tb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            var tb = sender as FastColoredTextBox;
            UpdateChangedFlag(tb.IsChanged);
        }

        private void TsFiles_TabStripItemClosed(object sender, EventArgs e)
        {
            SanitizeTabStrip();
            UpdateDocumentMap();
        }

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

        private void TsFiles_TabStripItemClosing(TabStripItemClosingEventArgs e)
        {
            if ((e.Item.Controls[0] as FastColoredTextBox).IsChanged)
            {
                switch (MessageBox.Show("Do you want save " + e.Item.Title + " ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case DialogResult.Yes:
                        if (!_controller.Save(e.Item))
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

        private bool IsSavedTab(string tagName) => tagName != null;

        #endregion

        #region UI Functionality

        public void UpdateChangedFlag(bool isChanged)
        {
            if (CurrentTB != null)
            {
                CurrentTB.IsChanged = isChanged;
                saveToolStripButton.Enabled = isChanged;
            }
        }

        public void UpdateDocumentMap()
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

        public bool IsFileAlreadyOpen(string name)
        {
            foreach (FATabStripItem tab in tsFiles.Items)
            {
                if (tab.Tag as string == name)
                {
                    tsFiles.SelectedItem = tab;
                    return true;
                }
            }
            return false;
        }

        public void HighlightCurrentLine()
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

        public void SanitizeTabStrip()
        {
            List<FATabStripItem> list = GetTabList();
            if (list.Count == 0)
            {
                _controller.CreateTab(null);
            }
        }

        public void CloseAllTabs()
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

        public void UpdateAutocompleteItems(List<AutocompleteItem> items)
        {
            popupMenu.Items.SetAutocompleteItems(items);
        }

        public void UpdatePopupMenu(FastColoredTextBox fctb)
        {
            popupMenu = new AutocompleteMenu(fctb);
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
            popupMenu.AllowTabKey = true;
        }

        public void AddTab(FATabStripItem tab)
        {
            tsFiles.AddTab(tab);
        }

        public void UpdateSelectedItem(FATabStripItem tab)
        {
            tsFiles.SelectedItem = tab;
        }

        public List<FATabStripItem> GetTabList()
        {
            List<FATabStripItem> list = new List<FATabStripItem>();

            foreach (FATabStripItem tab in tsFiles.Items)
            {
                list.Add(tab);
            }

            return list;
        }

        public FastColoredTextBox UpdateUIPropertiesForFCTB(FastColoredTextBox fctb)
        {
            fctb.ChangedLineColor = changedLineColor;
            fctb.KeyDown += new KeyEventHandler(MainForm_KeyDown);
            fctb.TextChangedDelayed += new EventHandler<TextChangedEventArgs>(Tb_TextChangedDelayed);
            return fctb;
        }

        public FATabStripItem CallSaveFileDialog(FATabStripItem tab)
        {
            if (sfdMain.ShowDialog() != DialogResult.OK)
            {
                return null;
            }
            tab.Title = Path.GetFileName(sfdMain.FileName);
            tab.Tag = sfdMain.FileName;
            return tab;
        }

        public void SetController(MainViewController controller)
        {
            _controller = controller;
            _controller.CreateTab(null);
            _controller.BuildAutocompleteMenu();
        }

        #endregion
    }
}
