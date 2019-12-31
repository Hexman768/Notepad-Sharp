using Essay_Analysis_Tool.Business;
using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System.Collections.Generic;

namespace Essay_Analysis_Tool.Interface
{
    public interface IMainView
    {
        void SetController(MainViewController mainViewController, LoggerViewController loggerViewController);

        void UpdateChangedFlag(bool isChanged);

        bool IsFileAlreadyOpen(string name);

        void UpdateDocumentMap();

        void SanitizeTabStrip();

        void UpdateAutocompleteItems(List<AutocompleteItem> items);

        void UpdatePopupMenu(FastColoredTextBox fctb);

        void AddTab(FATabStripItem tab);

        void UpdateSelectedItem(FATabStripItem tab);

        void HighlightCurrentLine();

        List<FATabStripItem> GetTabList();

        FastColoredTextBox UpdateUIPropertiesForFCTB(FastColoredTextBox fctb);

        FATabStripItem CallSaveFileDialog(FATabStripItem tab);
        
        /// <summary>
        /// Calls the <see cref="LoggerViewController"/> to log
        /// an informational message to the <see cref="LoggerView"/>.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="style">Style of the message.</param>
        void LogInfo(string value);

        /// <summary>
        /// Calls the <see cref="LoggerViewController"/> to log
        /// a warning message to the <see cref="LoggerView"/>.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="style">Style of the message.</param>
        void LogWarning(string value);

        /// <summary>
        /// Calls the <see cref="LoggerViewController"/> to log
        /// an error message to the <see cref="LoggerView"/>.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="style">Style of the message.</param>
        void LogError(string value);
    }
}
