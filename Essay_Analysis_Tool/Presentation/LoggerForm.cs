using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Essay_Analysis_Tool
{
    public partial class LoggerForm : Form
    {

        #region Variable Declarations

        TextStyle infoStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        TextStyle warningStyle = new TextStyle(Brushes.BurlyWood, null, FontStyle.Regular);
        TextStyle errorStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the LoggerForm.
        /// </summary>
        public LoggerForm()
        {
            InitializeComponent();

            console.ReadOnly = true;
        }

        #endregion

        #region Logging functionality

        /// <summary>
        /// This function appends the given text to the logger form based on the specified
        /// message type.
        /// </summary>
        /// <param name="value">Text to be appended to the form.</param>
        /// <param name="type">The logger message type.</param>
        public void Log(string value, LoggerMessageType type)
        {
            Style style;

            switch (type)
            {
                case LoggerMessageType.Warning:
                    style = warningStyle;
                    break;
                case LoggerMessageType.Error:
                    style = errorStyle;
                    break;
                default:
                    style = infoStyle;
                    break;
            }

            console.AppendText(DateTime.Now + ": " + value + "\n", style);
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
