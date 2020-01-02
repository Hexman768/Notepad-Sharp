using Essay_Analysis_Tool.Presentation;

namespace Essay_Analysis_Tool.Controllers
{
    /// <summary>
    /// Class to hold the business logic for the <see cref="LoggerView"/>.
    /// </summary>
    public class LoggerViewController
    {
        #region Variable Declarations

        private LoggerView _view;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the <see cref="LoggerViewController"/>.
        /// </summary>
        /// <param name="view">Interface to the view.</param>
        public LoggerViewController()
        {
            _view = new LoggerView(this);
        }

        #endregion

        #region Functionality

        /// <summary>
        /// This function appends the given text to the <see cref="LoggerView"/> based on the message type.
        /// </summary>
        /// <param name="value">Text to be appended to the view.</param>
        public void LogInfo(string value)
        {
            _view.Info(value);
        }

        /// <summary>
        /// This function appends the given text to the <see cref="LoggerView"/> based on the message type.
        /// </summary>
        /// <param name="value">Text to be appended to the view.</param>
        public void LogWarning(string value)
        {
            _view.Warn(value);
        }

        /// <summary>
        /// This function appends the given text to the <see cref="LoggerView"/> based on the message type.
        /// </summary>
        /// <param name="value">Text to be appended to the view.</param>
        public void LogError(string value)
        {
            _view.Error(value);
        }

        /// <summary>
        /// Shows the <see cref="LoggerView"/>.
        /// </summary>
        public void Show()
        {
            _view.Show();
        }

        #endregion
    }
}
