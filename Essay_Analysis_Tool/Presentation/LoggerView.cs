using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Windows.Forms;
using Essay_Analysis_Tool.Controllers;

namespace Essay_Analysis_Tool.Presentation
{
    /// <summary>
    /// Defines the <see cref="LoggerView" />.
    /// </summary>
    public partial class LoggerView : Form
    {

        #region Variables

        TextStyle infoStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        TextStyle warningStyle = new TextStyle(Brushes.BurlyWood, null, FontStyle.Regular);
        TextStyle errorStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);

        private LoggerViewController _controller;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the <see cref="LoggerView"/>.
        /// </summary>
        public LoggerView(LoggerViewController loggerViewController)
        {
            InitializeComponent();

            _controller = loggerViewController;
        }

        #endregion

        #region UI functionality

        public void Warn(string text)
        {
            console.AppendText(DateTime.Now + ": " + text, warningStyle);
        }

        public void Info(string text)
        {
            console.AppendText(DateTime.Now + ": " + text, infoStyle);
        }

        public void Error(string text)
        {
            console.AppendText(DateTime.Now + ": " + text, errorStyle);
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
