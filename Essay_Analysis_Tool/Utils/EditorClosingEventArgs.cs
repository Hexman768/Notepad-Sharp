using Essay_Analysis_Tool.Windows;
using System;

namespace Essay_Analysis_Tool.Utils
{
    public class EditorClosingEventArgs : EventArgs
    {
        public EditorClosingEventArgs(Editor item)
        {
            if (item != null)
            {
                Item = item;
            }
        }

        public Editor Item { get; set; }
        public bool Cancel { get; set; }
    }
}
