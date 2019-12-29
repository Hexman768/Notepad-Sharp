using Essay_Analysis_Tool.Business;
using Essay_Analysis_Tool.Interface;
using FastColoredTextBoxNS;
using System;
using System.Windows.Forms;

namespace Essay_Analysis_Tool
{
    /// <summary>
    /// Defines the <see cref="LoggerView" />
    /// </summary>
    public partial class LoggerView : Form, ILoggerView
    {

        #region Variables

        private LoggerViewController _controller;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the LoggerForm.
        /// </summary>
        public LoggerView()
        {
            InitializeComponent();

            console.ReadOnly = true;
        }

        #endregion

        #region UI functionality

        public void AppendText(string text, Style style)
        {
            console.AppendText(text, style);
        }

        public void SetController(LoggerViewController controller)
        {
            _controller = controller;
        }

        public void ShowView()
        {
            Show();
        }

        #endregion

        #region Event Handlers

        private void LoggerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            console.SelectAll();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            console.Copy();
        }

        #endregion
    }
}
