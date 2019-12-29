using Essay_Analysis_Tool.Business;
using FastColoredTextBoxNS;

namespace Essay_Analysis_Tool.Interface
{
    public interface ILoggerView
    {
        /// <summary>
        /// Appends the given text to the FastColoredTextBox instance.
        /// </summary>
        /// <param name="text">Message text.</param>
        /// <param name="style">Style of message.</param>
        void AppendText(string text, Style style);

        /// <summary>
        /// Sets the controller for the <see cref="LoggerView"/>.
        /// </summary>
        /// <param name="controller">Controller object.</param>
        void SetController(LoggerViewController controller);

        /// <summary>
        /// Shows the <see cref="LoggerView"/>.
        /// </summary>
        void ShowView();
    }
}
