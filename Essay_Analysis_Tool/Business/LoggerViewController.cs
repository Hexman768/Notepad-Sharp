using Essay_Analysis_Tool.Interface;
using FastColoredTextBoxNS;
using System;
using System.Drawing;

namespace Essay_Analysis_Tool.Business
{
    /// <summary>
    /// Class to hold the business logic for the <see cref="LoggerView"/>.
    /// </summary>
    public class LoggerViewController
    {
        #region Variable Declarations

        TextStyle infoStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        TextStyle warningStyle = new TextStyle(Brushes.BurlyWood, null, FontStyle.Regular);
        TextStyle errorStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);

        private ILoggerView _view;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the <see cref="LoggerViewController"/>.
        /// </summary>
        /// <param name="view">Interface to the view.</param>
        public LoggerViewController(ILoggerView view)
        {
            _view = view;
        }

        #endregion

        #region Functionality

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

            _view.AppendText(DateTime.Now + ": " + value + "\n", style);
        }

        /// <summary>
        /// Shows the <see cref="LoggerView"/>.
        /// </summary>
        public void ShowView()
        {
            _view.ShowView();
        }

        #endregion
    }
}
