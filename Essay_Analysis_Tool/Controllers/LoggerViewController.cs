using Essay_Analysis_Tool.Business;
using Essay_Analysis_Tool.Models;
using Essay_Analysis_Tool.Presentation;

namespace Essay_Analysis_Tool.Controllers
{
    /// <summary>
    /// Class to hold the business logic for the <see cref="LoggerView"/>.
    /// </summary>
    public class LoggerViewController
    {
        #region Variable Declarations

        private readonly LoggingService _loggingService;
        private LoggerView _view;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the <see cref="LoggerViewController"/>.
        /// </summary>
        /// <param name="view">Interface to the view.</param>
        public LoggerViewController(LoggingService loggingService)
        {
            _loggingService = loggingService;
            _loggingService.InfoAdded += UpdateView;
            _loggingService.WarningAdded += UpdateView;
            _loggingService.ErrorAdded += UpdateView;
            _view = new LoggerView(this);
        }

        private void UpdateView(object sender, InfoLogEntry entry)
        {
            _view.Add(entry);
        }

        private void UpdateView(object sender, WarnLogEntry entry)
        {
            _view.Add(entry);
        }

        private void UpdateView(object sender, ErrorLogEntry entry)
        {
            _view.Add(entry);
        }

        #endregion

        #region Functionality

        /// <summary>
        /// Shows the <see cref="LoggerView"/>.
        /// </summary>
        public void Show()
        {
            _view.Show();
        }

        #endregion

        ~LoggerViewController()
        {
            _loggingService.InfoAdded -= UpdateView;
            _loggingService.WarningAdded -= UpdateView;
            _loggingService.ErrorAdded -= UpdateView;
        }
    }
}
