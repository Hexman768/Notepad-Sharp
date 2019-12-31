using Essay_Analysis_Tool.Business;
using FastColoredTextBoxNS;

namespace Essay_Analysis_Tool.Interface
{
    public interface ILoggerView
    {
        /// <summary>
        /// Logs text to the logger using the informationStyle.
        /// </summary>
        /// <param name="text">Message.</param>
        void Info(string text);

        /// <summary>
        /// Logs text to the logger using the warningStyle.
        /// </summary>
        /// <param name="text">Message.</param>
        void Warn(string text);

        /// <summary>
        /// Logs text to the logger using the errorStyle.
        /// </summary>
        /// <param name="text">Message.</param>
        void Error(string text);

        /// <summary>
        /// Sets the controller for the <see cref="LoggerView"/>.
        /// </summary>
        /// <param name="controller">Controller object.</param>
        void SetController(LoggerViewController controller);

        /// <summary>
        /// Shows the <see cref="LoggerView"/>.
        /// </summary>
        void Show();
    }
}
