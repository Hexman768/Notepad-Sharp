using Essay_Analysis_Tool.Business;
using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System.Collections.Generic;

namespace Essay_Analysis_Tool.Interface
{
    public interface IMainView
    {
        void SetController(MainViewController controller);

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
    }
}
